namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>The TIFF and PNG predictors as PdfPig 0.1.16 applies them after Flate and LZW, writing at most a cap of bytes.</summary>
internal static class Predictor
{
    /// <summary>The predicted bytes; unreadable for rows of zero bytes or nonsensical samples, on which PdfPig's predictor never advances.</summary>
    public static Decoded Apply(ReadOnlySpan<byte> data, int predictor, int colors, int bits, int columns, int cap)
    {
        var rowBytes = ((columns * (long)colors * bits) + 7) / 8;
        if (colors < 1 || bits is not (1 or 2 or 4 or 8 or 16) || rowBytes <= 0)
            return Decoded.Unreadable;

        // Two row buffers and a padded last row come on top of the data, as in PdfPig.
        if (data.Length + (3 * rowBytes) > cap)
            return Decoded.OverCap;

        var rows = new Rows(predictor, colors, bits, columns, (int)rowBytes);
        var output = new BoundedOutput(cap, data.Length);
        for (var i = 0; i < data.Length;)
            i += rows.Take(data[i..], output);
        rows.Flush(output);
        return Decoded.Of(output);
    }

    private sealed class Rows(int predictor, int colors, int bits, int columns, int rowBytes)
    {
        private byte[] _current = new byte[rowBytes];
        private byte[] _last = new byte[rowBytes];
        private int _filled;
        private bool _typeRead;

        // Consumes a row's type byte (PNG) or as many bytes as the current row still needs, writing the row once full.
        public int Take(ReadOnlySpan<byte> data, BoundedOutput output)
        {
            if (predictor >= 10 && _filled == 0 && !_typeRead)
            {
                predictor = data[0] + 10;
                _typeRead = true;
                return 1;
            }

            var count = Math.Min(rowBytes - _filled, data.Length);
            data[..count].CopyTo(_current.AsSpan(_filled));
            _filled += count;
            if (_filled == rowBytes)
                Write(output);
            return count;
        }

        // A last partial row is padded with zeros and written, as PdfPig does when it flushes.
        public void Flush(BoundedOutput output)
        {
            if (_filled == 0)
                return;
            _current.AsSpan(_filled).Clear();
            Write(output);
        }

        private void Write(BoundedOutput output)
        {
            DecodeRow(predictor, colors, bits, columns, _current, _last);
            output.Write(_current);
            (_current, _last) = (_last, _current);
            _filled = 0;
            _typeRead = false;
        }
    }

    // PdfPig's row decoding, unchanged: TIFF predictor 2 and the PNG types None, Sub, Up, Average and Paeth.
    private static void DecodeRow(int predictor, int colors, int bits, int columns, byte[] row, byte[] last)
    {
        var pixel = ((colors * bits) + 7) / 8;
        var length = row.Length;
        switch (predictor)
        {
            case 2:
                Tiff(colors, bits, columns, row, pixel);
                break;
            case 11:
                for (var i = pixel; i < length; i++)
                    row[i] = (byte)(row[i] + row[i - pixel]);
                break;
            case 12:
                for (var i = 0; i < length; i++)
                    row[i] = (byte)(row[i] + last[i]);
                break;
            case 13:
                for (var i = 0; i < length; i++)
                    row[i] = (byte)(row[i] + (((i >= pixel ? row[i - pixel] : 0) + last[i]) / 2));
                break;
            case 14:
                for (var i = 0; i < length; i++)
                    row[i] = (byte)(row[i] + Paeth(i >= pixel ? row[i - pixel] : 0, last[i], i >= pixel ? last[i - pixel] : 0));
                break;
        }
    }

    private static void Tiff(int colors, int bits, int columns, byte[] row, int pixel)
    {
        switch (bits)
        {
            case 8:
                for (var i = pixel; i < row.Length; i++)
                    row[i] = (byte)(row[i] + row[i - pixel]);
                return;
            case 16:
                for (var i = pixel; i < row.Length - 1; i += 2)
                {
                    var sum = ((row[i] << 8) + row[i + 1]) + ((row[i - pixel] << 8) + row[i - pixel + 1]);
                    (row[i], row[i + 1]) = ((byte)(sum >> 8), (byte)sum);
                }

                return;
            case 1 when colors == 1:
                TiffSingleBits(row);
                return;
        }

        for (var m = colors; m < columns * colors; m++)
        {
            var at = m * bits / 8;
            var shift = 8 - (m * bits % 8) - bits;
            var before = (m - colors) * bits / 8;
            var beforeShift = 8 - ((m - colors) * bits % 8) - bits;
            var mask = (1 << bits) - 1;
            var sum = ((row[at] >> shift) & mask) + ((row[before] >> beforeShift) & mask);
            row[at] = (byte)((row[at] & ~(mask << shift)) | ((sum & mask) << shift));
        }
    }

    // One-bit grey: each bit is added (exclusive or) to the bit before it, across byte boundaries.
    private static void TiffSingleBits(byte[] row)
    {
        for (var j = 0; j < row.Length; j++)
        {
            for (var bit = 7; bit >= 0; bit--)
            {
                if (j == 0 && bit == 7)
                    continue;
                var value = (row[j] >> bit) & 1;
                var previous = bit == 7 ? row[j - 1] & 1 : (row[j] >> (bit + 1)) & 1;
                row[j] = ((value + previous) & 1) == 0 ? (byte)(row[j] & ~(1 << bit)) : (byte)(row[j] | (1 << bit));
            }
        }
    }

    private static int Paeth(int left, int up, int upLeft)
    {
        var estimate = left + up - upLeft;
        var toLeft = Math.Abs(estimate - left);
        var toUp = Math.Abs(estimate - up);
        var toUpLeft = Math.Abs(estimate - upLeft);
        return toLeft <= toUp && toLeft <= toUpLeft ? left : toUp <= toUpLeft ? up : upLeft;
    }
}
