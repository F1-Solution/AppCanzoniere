using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

// Real content that repeats near the page edge must survive: numbered titles, refrains and chord rows.
public class RunningLinesContentTests
{
    private sealed class Fake(IReadOnlyList<PdfPageWords> pages) : IPdfWordExtractor
    {
        public Task<PdfWordsResult> ExtractAsync(Stream pdf, CancellationToken cancellationToken = default) =>
            Task.FromResult(PdfWordsResult.Success(pages));
    }

    private static async Task<IReadOnlyList<string>> Import(params PdfPageWords[] pages)
    {
        var result = await new PdfTextImporter(new Fake(pages)).ImportAsync(new MemoryStream());
        return result.Pages.Select(p => p.Text).ToList();
    }

    private static PdfPageWords Page(int number, params (string Text, double Baseline)[] rows)
    {
        var builder = new PageBuilder();
        foreach (var (text, baseline) in rows)
            builder.Text(text, 50, baseline);
        return builder.Page(number);
    }

    [Fact]
    public async Task Numbered_titles_at_the_top_of_pages_are_kept_when_only_the_song_number_differs()
    {
        var titles = new[] { "11. ALLELUIA", "12. ALLELUIA", "13. ALLELUIA", "14. SANTO" };
        var pages = titles.Select((t, i) => Page(i + 1, (t, 810), ("Cantate al Signore un canto nuovo", 790), ("perche ha fatto meraviglie", 778))).ToArray();

        var texts = await Import(pages);

        for (var i = 0; i < titles.Length; i++)
            Assert.Contains(titles[i], texts[i]);
    }

    [Fact]
    public async Task A_refrain_and_a_chord_row_at_the_bottom_of_three_pages_are_kept()
    {
        var pages = Enumerable.Range(1, 3).Select(n => Page(n,
            ($"strofa numero {n} della canzone", 100), ("Do Sol La- Fa", 76), ("Gloria a te Signore", 64))).ToArray();

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Contains("Gloria a te Signore", t));
        Assert.All(texts, t => Assert.Contains("La-", t));
    }

    [Fact]
    public async Task A_chord_row_alone_at_the_bottom_of_three_pages_is_kept_even_far_from_the_text()
    {
        var pages = Enumerable.Range(1, 3).Select(n => Page(n, ($"strofa numero {n} della canzone", 400), ("Do Sol La- Fa", 30))).ToArray();

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Contains("La-", t));
    }

    [Fact]
    public async Task A_chord_with_a_number_is_not_the_same_as_the_chord_without_it()
    {
        var pages = new[]
        {
            Page(1, ("strofa uno della canzone", 400), ("Do7 Sol", 30)),
            Page(2, ("strofa due della canzone", 400), ("Do Sol", 30)),
            Page(3, ("strofa tre della canzone", 400), ("Do9 Sol", 30)),
        };

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Contains("Sol", t));
    }

    [Theory]
    [InlineData("Canti per tutti - {0} -")]
    [InlineData("Canti per tutti pag. {0}")]
    [InlineData("Canti per tutti {0}")]
    [InlineData("{0}")]
    public async Task A_footer_with_a_page_number_shape_is_still_removed(string shape)
    {
        var pages = Enumerable.Range(1, 3).Select(n => Page(n, ("Alleluia cantate al Signore", 400), (string.Format(shape, n), 30))).ToArray();

        var texts = await Import(pages);

        Assert.All(texts, t => Assert.Equal("Alleluia cantate al Signore", t));
    }
}
