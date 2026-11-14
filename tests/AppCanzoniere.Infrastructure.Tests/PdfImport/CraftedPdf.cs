using System.IO.Compression;
using System.Text;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

/// <summary>
/// Hand-written PDFs for the robustness tests: crafted filters, page trees and forms that the document
/// builder cannot produce. Every object is written verbatim, numbered from 1 in list order.
/// </summary>
internal static class CraftedPdf
{
    /// <summary>One line of text in Helvetica, the visible content of every crafted page.</summary>
    public const string Text = "BT /F1 11 Tf 50 700 Td (canto la mia gioia) Tj ET\n";

    public const string Font = "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>";

    /// <summary>A PDF object: a dictionary, or a stream when Data is set (Dict then holds the stream's own entries).</summary>
    public sealed record Obj(string Dict, byte[]? Data = null);

    /// <summary>Catalog (1), pages (2), font (3), page (4) whose content is object 5, then the extra objects from 6.</summary>
    public static byte[] OnePage(string streamEntries, byte[] content, string resources = "", IReadOnlyList<Obj>? extra = null, string pageEntries = "")
    {
        var objects = new List<Obj>
        {
            new("<< /Type /Catalog /Pages 2 0 R >>"),
            new("<< /Type /Pages /Kids [4 0 R] /Count 1 >>"),
            new(Font),
            new($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] {pageEntries} /Resources << /Font << /F1 3 0 R >> {resources} >> /Contents 5 0 R >>"),
            new(streamEntries, content),
        };
        if (extra is not null)
            objects.AddRange(extra);
        return Write(objects);
    }

    /// <summary>A page dictionary for hand-built trees: parent 2, Helvetica as /F1, the given content object.</summary>
    public static string Page(int contents, string entries = "") =>
        $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 3 0 R >> >> /Contents {contents} 0 R {entries}>>";

    public static byte[] Write(IReadOnlyList<Obj> objects)
    {
        using var pdf = new MemoryStream();
        void Put(string text) => pdf.Write(Encoding.Latin1.GetBytes(text));
        Put("%PDF-1.5\n");
        var offsets = new List<long>();
        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(pdf.Position);
            if (objects[i].Data is not { } data)
            {
                Put($"{i + 1} 0 obj\n{objects[i].Dict}\nendobj\n");
                continue;
            }

            Put($"{i + 1} 0 obj\n<< {objects[i].Dict} /Length {data.Length} >>\nstream\n");
            pdf.Write(data);
            Put("\nendstream\nendobj\n");
        }

        var xref = pdf.Position;
        Put($"xref\n0 {objects.Count + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets)
            Put($"{offset:D10} 00000 n \n");
        Put($"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
        return pdf.ToArray();
    }

    /// <summary>The line of text followed by comment lines up to about the given size: harmless content that packs to almost nothing.</summary>
    public static byte[] Padded(int bytes)
    {
        var content = new StringBuilder(Text, bytes + 100);
        while (content.Length < bytes)
            content.Append('%').Append('x', 98).Append('\n');
        return Ascii(content.ToString());
    }

    public static byte[] Ascii(string text) => Encoding.Latin1.GetBytes(text);

    public static byte[] Zlib(byte[] data)
    {
        using var packed = new MemoryStream();
        using (var zlib = new ZLibStream(packed, CompressionLevel.SmallestSize, leaveOpen: true))
            zlib.Write(data);
        return packed.ToArray();
    }

    /// <summary>Zlib data with its two header bytes zeroed: zlib rejects it, a decoder that skips the header does not.</summary>
    public static byte[] ZlibWithoutHeader(byte[] data)
    {
        var packed = Zlib(data);
        packed[0] = 0;
        packed[1] = 0;
        return packed;
    }

    public static byte[] Hex(byte[] data) => Ascii(Convert.ToHexString(data) + ">");

    /// <summary>ASCII base-85 with the end marker, four bytes per five characters.</summary>
    public static byte[] Ascii85(byte[] data)
    {
        var text = new StringBuilder();
        for (var i = 0; i < data.Length; i += 4)
        {
            var count = Math.Min(4, data.Length - i);
            uint value = 0;
            for (var k = 0; k < 4; k++)
                value = (value << 8) | (k < count ? data[i + k] : 0u);

            var group = new char[5];
            for (var k = 4; k >= 0; k--, value /= 85)
                group[k] = (char)('!' + (value % 85));
            text.Append(group, 0, count + 1);
        }

        return Ascii(text.Append("~>").ToString());
    }

    /// <summary>Run-length data: runs of up to 128 equal bytes as two bytes, everything else as literal blocks.</summary>
    public static byte[] RunLength(byte[] data)
    {
        var packed = new List<byte>();
        for (var i = 0; i < data.Length;)
        {
            var run = 1;
            while (i + run < data.Length && run < 128 && data[i + run] == data[i])
                run++;
            if (run > 1)
            {
                packed.Add((byte)(257 - run));
                packed.Add(data[i]);
                i += run;
                continue;
            }

            packed.Add(0);
            packed.Add(data[i++]);
        }

        packed.Add(128);
        return packed.ToArray();
    }

    /// <summary>PDF LZW (9 to 12 bit codes, early change), with a clear code whenever the table fills.</summary>
    public static byte[] Lzw(byte[] data)
    {
        var output = new List<byte>();
        int buffer = 0, bits = 0, width = 9, next = 258, current = -1;
        void Emit(int code)
        {
            buffer = (buffer << width) | code;
            bits += width;
            while (bits >= 8)
            {
                output.Add((byte)(buffer >> (bits - 8)));
                bits -= 8;
                buffer &= (1 << bits) - 1;
            }
        }

        var table = new Dictionary<(int, byte), int>();
        Emit(256);
        foreach (var b in data)
        {
            if (current < 0)
            {
                current = b;
                continue;
            }

            if (table.TryGetValue((current, b), out var code))
            {
                current = code;
                continue;
            }

            Emit(current);
            table[(current, b)] = next++;
            if (next >= 1 << width && width < 12)
                width++;
            if (next >= 4093)
            {
                Emit(256);
                table.Clear();
                (next, width) = (258, 9);
            }

            current = b;
        }

        if (current >= 0)
            Emit(current);
        Emit(257);
        if (bits > 0)
            output.Add((byte)(buffer << (8 - bits)));
        return output.ToArray();
    }
}
