#!/usr/bin/env bash
# Runs the emulator checks against a debug APK: tools/android-emulator-tests.sh <apk> [output-dir]
# Needs a booted emulator reachable through adb; exits 1 when any check fails.
set -u

APK=${1:?usage: android-emulator-tests.sh <apk> [output-dir]}
OUT=${2:-emulator-artifacts}
PKG=com.companyname.appcanzoniere.maui
ACTIVITY=$PKG/$PKG.MainActivity
TAG=AppCanzoniereSelfTest
mkdir -p "$OUT"

FAILED=0
pass() { echo "PASS: $1"; }
fail() { echo "FAIL: $1"; FAILED=$((FAILED + 1)); }
check() { # check <name> <exit status of the condition>
  if [ "$2" -eq 0 ]; then pass "$1"; else fail "$1"; fi
}

log() { adb logcat -d -s "$TAG:I" | tr -d '\r'; }
shot() { adb exec-out screencap -p > "$OUT/$1.png"; }
key() { adb shell input keyevent "$@"; sleep 1.5; }
audio_state() { adb shell dumpsys audio | tr -d '\r' | grep -E '^- STREAM_|Current:'; }

# wait_log <regex> <seconds>: waits until a self-test log line matches.
wait_log() {
  local end=$((SECONDS + $2))
  while [ $SECONDS -lt $end ]; do
    log | grep -Eq "$1" && return 0
    sleep 2
  done
  return 1
}

# scroll_field <name>: the named number from the latest SCROLL line since the last log clear.
scroll_field() { log | grep 'SCROLL y=' | tail -1 | sed -n "s/.*[ ]$1=\([-0-9.]*\).*/\1/p"; }
scroll_lines() { log | grep -c 'SCROLL y='; }
near() { awk -v a="$1" -v b="$2" -v t="${3:-4}" 'BEGIN { d = a - b; if (d < 0) d = -d; exit !(d <= t) }'; }

CURRENT_Y=0
# turn <name> <expected sign +1|-1> <adb input args...>: sends a key and checks the offset moved by one page.
turn() {
  local name=$1 sign=$2; shift 2
  adb logcat -c
  key "$@"
  local y viewport row page
  y=$(scroll_field y); viewport=$(scroll_field viewport); row=$(scroll_field row)
  if [ -z "$y" ] || [ -z "$viewport" ] || [ -z "$row" ]; then
    log | tail -5
    fail "$name (no scroll reported)"
    return
  fi
  page=$(awk -v v="$viewport" -v r="$row" -v s="$sign" 'BEGIN { print s * (v - r) }')
  echo "  $name: y $CURRENT_Y -> $y, expected step $page (viewport $viewport, row $row)"
  near "$y" "$(awk -v c="$CURRENT_Y" -v p="$page" 'BEGIN { print c + p }')"
  check "$name" $?
  CURRENT_Y=$y
}

echo "== Setup"
adb wait-for-device
adb shell input keyevent 82 # dismisses the lock screen if shown
adb install -r -g "$APK" || { echo "FAIL: install"; exit 1; }
adb shell settings put global window_animation_scale 0
adb shell settings put global transition_animation_scale 0
adb shell settings put global animator_duration_scale 0

echo "== 1. OCR self-test"
adb shell am force-stop $PKG
adb logcat -c
adb shell am start -n "$ACTIVITY" --es selftest ocr > /dev/null
wait_log 'OCR (OK|FAIL)' 180
log | grep OCR
log | grep -q 'OCR INFO bundled=True'; check "OCR uses the bundled model" $?
log | grep 'OCR OK' | grep -qi alleluia && log | grep 'OCR OK' | grep -qi nuovo; check "OCR recognises the expected words" $?

echo "== 2. Song screen opens"
adb shell am force-stop $PKG
adb logcat -c
adb shell am start -n "$ACTIVITY" --es selftest song > /dev/null
wait_log 'SONG (OPENED|FAIL)' 180
log | grep -q 'SONG OPENED'; check "Song screen opens on the synthetic song" $?
sleep 5
shot 01-song-before

echo "== 6a. Screen stays on while the song is open"
log | grep 'KEEPSCREENON' | tail -1 | grep -q 'KEEPSCREENON True'; check "Keep-screen-on requested on the song screen" $?
adb shell dumpsys power | tr -d '\r' > "$OUT/power-song.txt"
grep -q "WAKE_LOCK.*'WindowManager/" "$OUT/power-song.txt"; check "dumpsys power shows a window wake lock on the song screen" $?

echo "== 3. Page keys"
# The first key sent after the screen opens never reaches the app on the emulator, so a neutral key goes first.
key KEYCODE_SHIFT_LEFT
log | grep "KEY " | tail -3
adb logcat -c
turn "PAGE_DOWN forward" 1 KEYCODE_PAGE_DOWN
shot 02-song-after-page-down
turn "SPACE forward" 1 KEYCODE_SPACE
turn "DPAD_DOWN forward" 1 KEYCODE_DPAD_DOWN
turn "DPAD_RIGHT forward" 1 KEYCODE_DPAD_RIGHT
turn "PAGE_UP back" -1 KEYCODE_PAGE_UP
turn "DPAD_UP back" -1 KEYCODE_DPAD_UP
turn "DPAD_LEFT back" -1 KEYCODE_DPAD_LEFT

echo "== 4a. Volume keys turn pages on the song screen"
AUDIO_BEFORE=$(audio_state)
turn "VOLUME_DOWN forward" 1 KEYCODE_VOLUME_DOWN
turn "VOLUME_UP back" -1 KEYCODE_VOLUME_UP
AUDIO_AFTER=$(audio_state)
[ "$AUDIO_BEFORE" = "$AUDIO_AFTER" ]; check "Volume unchanged on the song screen" $?

echo "== 5. A long press turns one page"
adb logcat -c
BEFORE_Y=$CURRENT_Y
adb shell input keyevent --longpress KEYCODE_PAGE_DOWN
sleep 2
Y=$(scroll_field y); VIEWPORT=$(scroll_field viewport); ROW=$(scroll_field row)
echo "  long press: y $BEFORE_Y -> $Y (viewport $VIEWPORT, row $ROW)"
if [ -n "$Y" ] && near "$Y" "$(awk -v c="$BEFORE_Y" -v v="$VIEWPORT" -v r="$ROW" 'BEGIN { print c + v - r }')"; then
  pass "Long press moved exactly one page"
else
  fail "Long press moved exactly one page"
fi
shot 03-song-after-long-press
adb shell uiautomator dump /sdcard/ui.xml > /dev/null 2>&1 && adb pull /sdcard/ui.xml "$OUT/ui-song.xml" > /dev/null 2>&1

echo "== 4b/6b. Leaving the song screen"
adb logcat -c
key KEYCODE_BACK
shot 04-after-back
log | grep -q 'KEEPSCREENON False'; check "Keep-screen-on released after leaving the song screen" $?
adb shell dumpsys power | tr -d '\r' > "$OUT/power-home.txt"
grep -q "WAKE_LOCK.*'WindowManager/" "$OUT/power-home.txt" && fail "dumpsys power shows no window wake lock after leaving" || pass "dumpsys power shows no window wake lock after leaving"

adb logcat -c
AUDIO_HOME0=$(audio_state)
key KEYCODE_VOLUME_UP
AUDIO_HOME1=$(audio_state)
key KEYCODE_VOLUME_DOWN
AUDIO_HOME2=$(audio_state)
{ echo "$AUDIO_HOME0"; echo ---; echo "$AUDIO_HOME1"; echo ---; echo "$AUDIO_HOME2"; } > "$OUT/audio-home.txt"
{ [ "$AUDIO_HOME0" != "$AUDIO_HOME1" ] || [ "$AUDIO_HOME1" != "$AUDIO_HOME2" ]; }; check "Volume keys change the volume on another screen" $?
[ "$(scroll_lines)" -eq 0 ]; check "Volume keys do not scroll on another screen" $?

adb logcat -d > "$OUT/logcat.txt"
echo "== Done: $FAILED failed check(s)"
[ "$FAILED" -eq 0 ]
