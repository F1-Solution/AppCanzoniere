#!/usr/bin/env bash
# Marks capture windows for the runner's tcpdump file while ML Kit runs: tools/android-network-capture.sh <apk> <output-dir>
# Writes <output-dir>/net-markers.txt, jobscheduler dumps and per-uid netstats; tools/android-network-analyze.py reads them.
set -u

APK=${1:?usage: android-network-capture.sh <apk> <output-dir>}
OUT=${2:?usage: android-network-capture.sh <apk> <output-dir>}
PKG=com.companyname.appcanzoniere.maui
TAG=AppCanzoniereSelfTest
mkdir -p "$OUT"
MARK="$OUT/net-markers.txt"
: > "$MARK"
# uid_history <uid>: the netstats history lines (st=...) of one uid.
uid_history() {
  adb shell dumpsys netstats detail | tr -d '\r' | awk -v uid="$1" '
    /uid=-?[0-9]+ set=/ { show = ($0 ~ ("uid=" uid " set=")) }
    show && (/uid=-?[0-9]+ set=/ || /^[ \t]*st=/) { print }'
}

# mark <label>: records the host time, the app's byte history and the shell's (uid 2000, proves the collection works) from netstats.
mark() {
  echo "$1 $(date +%s)" >> "$MARK"; echo "MARK $1"
  adb shell cmd netstats poll --force > /dev/null 2>&1 || true
  { echo "## $1"; uid_history "$UID_APP"; } >> "$OUT/net-uid-counters.txt"
  { echo "## $1"; uid_history 2000; } >> "$OUT/net-shell-counters.txt"
}

adb wait-for-device
adb shell input keyevent 82
adb install -r -g "$APK" || { echo "FAIL: install"; exit 1; }
UID_APP=$(adb shell pm list packages -U "$PKG" | tr -d '\r' | sed -n 's/.*uid:\([0-9]*\).*/\1/p' | head -1)
echo "app uid: $UID_APP" | tee "$OUT/net-uid.txt"
adb shell dumpsys connectivity | tr -d '\r' | grep -iE 'validated' | head -5

# Control: a lookup from the shell must show up in the capture, or the capture is not seeing guest traffic.
mark control_start
adb shell ping -c 2 -W 2 example.com > "$OUT/net-control.txt" 2>&1
mark control_end

# Window 1: emulator and system traffic with the app installed but never started.
adb shell am force-stop "$PKG"
mark baseline_start
sleep "${BASELINE_SECONDS:-60}"
mark baseline_end

# Window 2: OCR self-test.
adb logcat -c
component=$(adb shell cmd package resolve-activity --brief -a android.intent.action.MAIN -c android.intent.category.LAUNCHER "$PKG" | tr -d '\r' | tail -1)
mark ocr_start
adb shell am start -n "$component" --es selftest ocr > /dev/null
end=$((SECONDS + 180))
while [ $SECONDS -lt $end ]; do
  adb logcat -d -s "$TAG:I" | tr -d '\r' | grep -Eq 'OCR (OK|FAIL)' && break
  sleep 2
done
adb logcat -d -s "$TAG:I" | tr -d '\r' | grep OCR | tee "$OUT/net-ocr.log"
mark ocr_end

# Window 3: wait, then force every job and alarm the app registered.
sleep "${WAIT_SECONDS:-60}"
adb shell dumpsys jobscheduler | tr -d '\r' > "$OUT/net-jobscheduler-before.txt"
adb shell dumpsys alarm | tr -d '\r' | grep -B1 -A3 "$PKG" > "$OUT/net-alarms.txt" || true
grep -E "JOB #.*$PKG" "$OUT/net-jobscheduler-before.txt" | tee "$OUT/net-jobs.txt"
mark jobs_start
sed -n "s/.*JOB #[^/]*\/\(-\{0,1\}[0-9][0-9]*\):.*$PKG.*/\1/p" "$OUT/net-jobscheduler-before.txt" | sort -u | while read -r id; do
  echo "run job $id"
  adb shell cmd jobscheduler run -f "$PKG" "$id"
done
sleep "${JOBS_SECONDS:-90}"
mark jobs_end

# The app may keep uploading after the jobs: one more quiet window with the app in the background.
adb shell input keyevent 3
sleep "${TAIL_SECONDS:-60}"
mark tail_end

adb shell dumpsys netstats detail | tr -d '\r' > "$OUT/net-netstats.txt"
adb shell dumpsys jobscheduler | tr -d '\r' > "$OUT/net-jobscheduler-after.txt"
adb shell am force-stop "$PKG"
echo "capture done"
