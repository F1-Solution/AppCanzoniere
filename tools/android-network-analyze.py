#!/usr/bin/env python3
"""Lists DNS names, TLS server names, HTTP hosts and peer addresses per capture window.

Usage: android-network-analyze.py <pcap> <markers> <output-file> [<uid-counters>]
Reads a classic pcap (Ethernet) captured on the runner by tcpdump; needs no extra packages.
Exits 1 when a telemetry host shows up in a DNS query or TLS server name, when the control window shows no DNS,
or (with <uid-counters>) when the app's own byte counters grew between ocr_start and tail_end.
"""
import collections
import socket
import struct
import sys


def read_packets(path):
    data = open(path, "rb").read()
    endian = "<" if data[:4] in (b"\xd4\xc3\xb2\xa1", b"\x4d\x3c\xb2\xa1") else ">"
    offset = 24
    while offset + 16 <= len(data):
        sec, usec, caplen, _ = struct.unpack(endian + "IIII", data[offset:offset + 16])
        offset += 16
        yield sec + usec / 1e6, data[offset:offset + caplen]
        offset += caplen


def dns_name(msg, pos):
    labels = []
    while pos < len(msg):
        n = msg[pos]
        if n == 0:
            break
        if n & 0xC0 == 0xC0:
            break
        labels.append(msg[pos + 1:pos + 1 + n].decode("ascii", "replace"))
        pos += 1 + n
    return ".".join(labels)


def sni(payload):
    # TLS record 0x16, handshake 0x01 (ClientHello); walk to the server_name extension.
    try:
        if payload[0] != 0x16 or payload[5] != 0x01:
            return None
        p = 43
        p += 1 + payload[p]
        p += 2 + struct.unpack(">H", payload[p:p + 2])[0]
        p += 1 + payload[p]
        end = p + 2 + struct.unpack(">H", payload[p:p + 2])[0]
        p += 2
        while p + 4 <= end:
            etype, elen = struct.unpack(">HH", payload[p:p + 4])
            if etype == 0:
                nlen = struct.unpack(">H", payload[p + 7:p + 9])[0]
                return payload[p + 9:p + 9 + nlen].decode("ascii", "replace")
            p += 4 + elen
    except (IndexError, struct.error):
        pass
    return None


def classify_l4(proto, frame, l4, dst):
    """Names one TCP or UDP segment starting at offset l4: DNS query, TLS server name, HTTP host, QUIC or a new connection."""
    if proto == 17:
        if len(frame) < l4 + 8:
            return None
        dport = struct.unpack(">H", frame[l4 + 2:l4 + 4])[0]
        msg = frame[l4 + 8:]
        if dport == 53 and len(msg) > 12 and not msg[2] & 0x80:
            return "dns", dns_name(msg, 12)
        return ("quic" if dport == 443 else "udp"), "%s:%d" % (dst, dport)
    if proto == 6:
        if len(frame) < l4 + 20:
            return None
        dport = struct.unpack(">H", frame[l4 + 2:l4 + 4])[0]
        offset = (frame[l4 + 12] >> 4) * 4
        payload = frame[l4 + offset:]
        name = sni(payload) if payload else None
        if name:
            return "tls", name
        if payload[:4] in (b"GET ", b"POST", b"HEAD", b"PUT "):
            for line in payload.split(b"\r\n"):
                if line.lower().startswith(b"host:"):
                    return "http", line[5:].strip().decode("ascii", "replace")
        if frame[l4 + 13] & 0x02 and not frame[l4 + 13] & 0x10:
            return "tcp", "%s:%d" % (dst, dport)
    return None


def classify(frame):
    """Returns (kind, value) or None for one Ethernet frame; IPv6 without extension headers is handled like IPv4."""
    if len(frame) >= 54 and frame[12:14] == b"\x86\xdd":
        dst = "[%s]" % socket.inet_ntop(socket.AF_INET6, frame[38:54])
        return classify_v6(frame, dst)
    if len(frame) < 34 or frame[12:14] != b"\x08\x00":
        return None
    return classify_l4(frame[23], frame, 14 + (frame[14] & 0x0F) * 4, socket.inet_ntoa(frame[30:34]))


def classify_v6(frame, dst):
    """IPv6: the same names as IPv4, with kinds suffixed 6 so they are counted apart."""
    found = classify_l4(frame[20], frame, 54, dst)
    if found is None:
        return ("ipv6", dst) if frame[20] not in (6, 17) else None
    return found[0] + "6", found[1]


def uid_totals(path):
    """Total received plus sent bytes of the app per marker, from the netstats lines saved at each mark."""
    totals, label = {}, None
    for line in open(path):
        if line.startswith("## "):
            label = line[3:].strip()
            totals[label] = 0
        elif label and line.strip().startswith("st="):
            fields = dict(p.split("=", 1) for p in line.split() if "=" in p)
            totals[label] += int(fields.get("rb", 0)) + int(fields.get("tb", 0))
    return totals


TELEMETRY = ("firebaselogging",)


def main():
    pcap, markers_path, out_path = sys.argv[1:4]
    counters_path = sys.argv[4] if len(sys.argv) > 4 else None
    markers = {}
    for line in open(markers_path):
        parts = line.split()
        if len(parts) == 2:
            markers[parts[0]] = int(parts[1])
    packets = [(t, classify(f)) for t, f in read_packets(pcap)]
    windows = [("control_start", "control_end"), ("baseline_start", "baseline_end"), ("ocr_start", "ocr_end"), ("ocr_end", "jobs_start"),
               ("jobs_start", "jobs_end"), ("jobs_end", "tail_end")]
    lines = ["pcap: %s, %d packets" % (pcap, len(packets)), "markers: %s" % markers]
    failures = []
    for start, end in windows:
        if start not in markers or end not in markers:
            continue
        lines.append("")
        lines.append("== window %s -> %s (%d s)" % (start, end, markers[end] - markers[start]))
        counts = collections.Counter(c for t, c in packets if c and markers[start] <= t <= markers[end])
        for (kind, value), n in sorted(counts.items()):
            lines.append("  %-5s %4d  %s" % (kind, n, value))
        if start == "control_start" and not any(kind.startswith("dns") for kind, _ in counts):
            failures.append("the control window shows no DNS query: the capture does not see the emulator's traffic")
    lines.append("")
    lines.append("== whole capture")
    counts = collections.Counter(c for t, c in packets if c)
    for (kind, value), n in sorted(counts.items()):
        lines.append("  %-5s %4d  %s" % (kind, n, value))
        if kind in ("dns", "dns6", "tls", "tls6") and any(host in value.lower() for host in TELEMETRY):
            failures.append("telemetry host in %s: %s" % (kind, value))
    lines.append("")
    lines.append("== app uid bytes (received + sent, from netstats)")
    if counters_path:
        totals = uid_totals(counters_path)
        for label, total in totals.items():
            lines.append("  %-15s %d" % (label, total))
        if "ocr_start" in totals and "tail_end" in totals:
            grown = totals["tail_end"] - totals["ocr_start"]
            lines.append("  growth ocr_start -> tail_end: %d" % grown)
            if grown > 0:
                failures.append("the app's own traffic grew by %d bytes between ocr_start and tail_end" % grown)
        else:
            failures.append("netstats counters missing for ocr_start or tail_end")
    else:
        lines.append("  no counters file given")
    lines.append("")
    lines.append("RESULT: " + ("FAIL" if failures else "PASS"))
    lines.extend("  FAIL: " + f for f in failures)
    text = "\n".join(lines)
    open(out_path, "w").write(text + "\n")
    print(text)
    return 1 if failures else 0


sys.exit(main())
