#!/usr/bin/env bash
# Runs the emulator checks against a debug APK: tools/android-emulator-tests.sh <apk> [output-dir]
# Needs a booted emulator reachable through adb; exits 1 when any check fails.
set -u

APK=${1:?usage: android-emulator-tests.sh <apk> [output-dir]}
OUT=${2:-emulator-artifacts}
PKG=com.companyname.appcanzoniere.maui
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
# launch <selftest> [extra --es args...]: starts the launcher activity the package manager reports, so no activity name is hard-coded.
launch() {
  local component
  component=$(adb shell cmd package resolve-activity --brief -a android.intent.action.MAIN -c android.intent.category.LAUNCHER "$PKG" | tr -d '' | tail -1)
  case "$component" in
    */*) local test=$1; shift; adb shell am start -n "$component" --es selftest "$test" "$@" > /dev/null ;;
    *) echo "FAIL: no launcher activity found for $PKG (got '$component')"; FAILED=$((FAILED + 1)) ;;
  esac
}
# tap_song: taps inside the song area (horizontal centre, two thirds down the screen).
tap_song() {
  local size w h
  size=$(adb shell wm size | tr -d '' | grep -Eo '[0-9]+x[0-9]+' | tail -1)
  w=${size%x*}; h=${size#*x}
  adb shell input tap $((w / 2)) $((h * 2 / 3))
  sleep 1
}
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
# tap_then_turn <name> <sign> <key>: taps the song, takes the offset the tap itself left (focus can scroll the view), then sends the key.
tap_then_turn() {
  adb logcat -c
  tap_song
  local y
  y=$(scroll_field y)
  [ -n "$y" ] && CURRENT_Y=$y
  turn "$@"
}

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
launch ocr
wait_log 'OCR (OK|FAIL)' 180
log | grep OCR
log | grep -q 'OCR INFO bundled=True'; check "OCR uses the bundled model" $?
log | grep 'OCR OK' | grep -qi alleluia && log | grep 'OCR OK' | grep -qi nuovo; check "OCR recognises the expected words" $?

echo "== 2. Song screen opens"
adb shell am force-stop $PKG
adb logcat -c
launch song
wait_log 'SONG (OPENED|FAIL)' 180
log | grep -q 'SONG OPENED'; check "Song screen opens on the synthetic song" $?
sleep 5
log | grep 'FOCUS ' | tail -1
log | grep 'FOCUS ' | tail -1 | grep -qv 'FOCUS none'; check "The song screen holds focus when the page appears" $?
shot 01-song-before

echo "== 6a. Screen stays on while the song is open"
log | grep 'KEEPSCREENON' | tail -1 | grep -q 'KEEPSCREENON True'; check "Keep-screen-on requested on the song screen" $?
adb shell dumpsys power | tr -d '\r' > "$OUT/power-song.txt"
grep -q "WAKE_LOCK.*'WindowManager/" "$OUT/power-song.txt"; check "dumpsys power shows a window wake lock on the song screen" $?

echo "== 3. Page keys"
# The very first key after a touch must turn the page: a tap goes before each of these.
adb logcat -c
tap_then_turn "PAGE_DOWN forward (first key after a tap)" 1 KEYCODE_PAGE_DOWN
shot 02-song-after-page-down
tap_then_turn "SPACE forward (first key after a tap)" 1 KEYCODE_SPACE
tap_then_turn "DPAD_DOWN forward (first key after a tap)" 1 KEYCODE_DPAD_DOWN
# Tab, tap, key: a navigation key before the touch must not cost the key after it.
key KEYCODE_TAB
tap_then_turn "PAGE_DOWN forward (after Tab and a tap)" 1 KEYCODE_PAGE_DOWN
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

# push_fixture <name>: copies a committed import PDF into the app's private files folder (the debug APK is debuggable, so run-as works).
push_fixture() {
  adb exec-in run-as "$PKG" sh -c "mkdir -p files/import-fixtures && cat > files/import-fixtures/$1" < "tests/fixtures/import/$1"
}
# run_import <fixture> <scenario> <timeout seconds>: fresh app process, imports the fixture and waits for the END line.
run_import() {
  adb shell am force-stop $PKG
  adb logcat -c
  launch import --es importfile "$1" --es importscenario "$2"
  wait_log 'IMPORT (END|FAIL)' "$3"
  log | grep 'IMPORT '
}

echo "== 7. Import screen: single-song text PDF"
push_fixture single-song.pdf
run_import single-song.pdf single 180
log | grep -q 'IMPORT DONE phase=Result;songs=1;.*reviewing=True'; check "Single song goes straight to review" $?
log | grep -q 'IMPORT CHECK reviewing=True accepted=False'; check "The declaration starts unticked" $?
log | grep -q 'IMPORT CHECK declaration=True accepted=False canSave=False'; check "Save is disabled until the declaration is ticked" $?
log | grep -q 'IMPORT CHECK accepted=True canSave=True'; check "Save is enabled once the declaration is ticked" $?
log | grep 'IMPORT END' | grep -q 'saved=1'; check "The single song is saved" $?
shot 05-import-single

echo "== 8. Import screen: mini songbook"
push_fixture mini-songbook.pdf
run_import mini-songbook.pdf songbook 180
log | grep -q 'IMPORT DONE phase=Result;songs=3;selected=3'; check "Songbook gives 3 songs, all selected" $?
log | grep -q 'IMPORT CHECK none-selected canSave=False'; check "Select none disables saving" $?
log | grep -q 'IMPORT CHECK declaration=True accepted=False canSave=False'; check "Saving the songbook asks for the declaration first" $?
log | grep 'IMPORT END' | grep -q 'saved=3'; check "All three songs are saved" $?
shot 06-import-songbook

echo "== 9. Import screen: scanned page read with on-device OCR"
push_fixture scanned-page.pdf
run_import scanned-page.pdf plain 300
log | grep 'IMPORT DONE' | grep -Eq 'phase=Result;songs=[1-9][0-9]*;.*origins=(Scan|TextAndScan)'; check "OCR finds at least one song with origin scan" $?
shot 07-import-scan

echo "== 10. Import screen: damaged file"
push_fixture damaged.pdf
run_import damaged.pdf plain 120
log | grep 'IMPORT DONE' | grep -Eq 'phase=Error;.*error=[^;]+;'; check "Damaged file ends in the error state with a title" $?
[ -n "$(adb shell pidof $PKG | tr -d '[:space:]')" ]; check "The app is still running after the damaged file" $?
shot 08-import-damaged

echo "== 11. Import screen: cancel while reading"
push_fixture mini-songbook.pdf
run_import mini-songbook.pdf cancel 120
log | grep -q 'IMPORT CANCELLING phase=Reading'; check "The import was in the reading state when it was cancelled" $?
log | grep 'IMPORT END' | grep -q 'phase=Cancelled;songs=0;'; check "Cancel leaves no partial list" $?

echo "== 12. Import screen: cancel during a real recognition"
push_fixture scanned-page.pdf
run_import scanned-page.pdf cancelreal 300
log | grep -q 'IMPORT CANCELLING stage=Recognizing phase=Reading'; check "The import was recognising the page when it was cancelled" $?
log | grep 'IMPORT END' | grep -q 'phase=Cancelled;songs=0;'; check "Cancelling during recognition leaves no list" $?
[ -n "$(adb shell pidof $PKG | tr -d '[:space:]')" ]; check "The app is still running after cancelling during recognition" $?

adb logcat -d > "$OUT/logcat.txt"
echo "== Done: $FAILED failed check(s)"
[ "$FAILED" -eq 0 ]
