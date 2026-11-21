using AppCanzoniere.Application.ImportReview;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.ImportReview;

public sealed class BoundedReaderTests
{
    [Fact]
    public async Task Reads_the_whole_stream_when_it_fits()
    {
        var bytes = Enumerable.Range(0, 5000).Select(i => (byte)i).ToArray();

        var read = await BoundedReader.ReadAsync(new MemoryStream(bytes), 6000);

        Assert.Equal(bytes, read);
    }

    [Fact]
    public async Task Returns_null_when_a_seekable_stream_is_over_the_limit_without_reading_it()
    {
        var stream = new MemoryStream(new byte[100]);

        Assert.Null(await BoundedReader.ReadAsync(stream, 99));
        Assert.Equal(0, stream.Position);
    }

    [Fact]
    public async Task Returns_null_when_a_forward_only_stream_passes_the_limit()
    {
        var stream = new ForwardOnly(new byte[300]);

        Assert.Null(await BoundedReader.ReadAsync(stream, 200));
    }

    [Fact]
    public async Task Accepts_a_stream_of_exactly_the_limit()
    {
        var read = await BoundedReader.ReadAsync(new ForwardOnly(new byte[200]), 200);

        Assert.Equal(200, read!.Length);
    }

    [Fact]
    public async Task Stops_when_cancelled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => BoundedReader.ReadAsync(new ForwardOnly(new byte[10]), 100, cts.Token));
    }

    [Fact]
    public async Task An_unavailable_reader_fails_with_its_reason()
    {
        var result = await new UnavailablePdfWordExtractor("The PDF reader is missing.").ExtractAsync(Stream.Null);

        Assert.Equal(PdfExtractionStatus.Unreadable, result.Status);
        Assert.Contains("missing", result.Reason);
    }

    private sealed class ForwardOnly(byte[] data) : MemoryStream(data)
    {
        public override bool CanSeek => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    }
}
