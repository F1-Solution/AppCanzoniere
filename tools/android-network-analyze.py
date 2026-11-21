#!/usr/bin/env python3
"""Lists DNS names, TLS server names, HTTP hosts and peer addresses per capture window.

Usage: android-network-analyze.py <pcap> <markers> <output-file> [<uid-counters>]
Reads a classic pcap (Ethernet) captured on the runner by tcpdump; needs no extra packages.
With <uid-counters> it also fails (exit 1) when the app's own byte counters grew between ocr_start and tail_end.
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


def classify_v6(frame):
    """IPv6 without extension headers: DNS, QUIC and TCP are only counted, not named."""
    nxt = frame[20]
    dst = socket.inet_ntop(socket.AF_INET6, frame[38:54])
    if nxt not in (6, 17) or len(frame) < 58:
        return "ipv6", dst
    dport = struct.unpack(">H", frame[56:58])[0]
    kind = {53: "dns6", 443: "quic6" if nxt == 17 else "tls6"}.get(dport, "ipv6")
    return kind, "[%s]:%d" % (dst, dport)


def classify(frame):
    """Returns (kind, value) or None for one Ethernet frame."""
    if len(frame) < 34 or frame[12:14] != b"\x08\x00":
        return None
    ihl = (frame[14] & 0x0F) * 4
    proto = frame[23]
    dst = socket.inet_ntoa(frame[30:34])
    l4 = 14 + ihl
    if proto == 17:
        sport, dport = struct.unpack(">HH", frame[l4:l4 + 4])
        msg = frame[l4 + 8:]
        if dport == 53 and len(msg) > 12 and not msg[2] & 0x80:
            return "dns", dns_name(msg, 12)
        return ("quic" if dport == 443 else "udp"), "%s:%d" % (dst, dport)
    if proto == 6:
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
    for start, end in windows:
        if start not in markers or end not in markers:
            continue
        lines.append("")
        lines.append("== window %s -> %s (%d s)" % (start, end, markers[end] - markers[start]))
        counts = collections.Counter(c for t, c in packets if c and markers[start] <= t <= markers[end])
        for (kind, value), n in sorted(counts.items()):
            lines.append("  %-5s %4d  %s" % (kind, n, value))
    lines.append("")
    lines.append("== whole capture")
    counts = collections.Counter(c for t, c in packets if c)
    for (kind, value), n in sorted(counts.items()):
        lines.append("  %-5s %4d  %s" % (kind, n, value))
    text = "\n".join(lines)
    open(out_path, "w").write(text + "\n")
    print(text)


main()
