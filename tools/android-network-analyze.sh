#!/usr/bin/env bash
# Lists DNS names and TLS server names per capture window: tools/android-network-analyze.sh <pcap> <markers> <output-file>
set -u
PCAP=${1:?usage: android-network-analyze.sh <pcap> <markers> <output-file>}
MARKERS=${2:?markers file}
OUTFILE=${3:?output file}
mark_of() { awk -v k="$1" '$1==k { print $2 }' "$MARKERS"; }
{
  echo "pcap: $PCAP"; ls -l "$PCAP"
  echo "markers:"; cat "$MARKERS"
  for w in "baseline_start baseline_end" "ocr_start ocr_end" "ocr_end jobs_start" "jobs_start jobs_end" "jobs_end tail_end"; do
    from=${w% *}; to=${w#* }
    s=$(mark_of "$from"); e=$(mark_of "$to")
    if [ -z "$s" ] || [ -z "$e" ]; then continue; fi
    echo; echo "== window $from -> $to ($((e - s)) s)"
    echo "-- DNS queries"
    tshark -r "$PCAP" -Y "dns.flags.response==0 && frame.time_epoch>=$s && frame.time_epoch<=$e" -T fields -e dns.qry.name 2>/dev/null | sort | uniq -c | sort -rn
    echo "-- TLS server names"
    tshark -r "$PCAP" -Y "tls.handshake.type==1 && frame.time_epoch>=$s && frame.time_epoch<=$e" -T fields -e tls.handshake.extensions_server_name 2>/dev/null | sort | uniq -c | sort -rn
  done
  echo; echo "== whole capture, all DNS names"
  tshark -r "$PCAP" -Y "dns.flags.response==0" -T fields -e dns.qry.name 2>/dev/null | sort | uniq -c | sort -rn
} > "$OUTFILE" 2>&1
cat "$OUTFILE"
