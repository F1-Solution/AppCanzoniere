using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfDocumentTitleTests
{
    [Theory]
    [InlineData("Il mio canto", "Il mio canto")]
    [InlineData("  Il   mio\tcanto  ", "Il mio canto")]
    [InlineData("Il mio\u0000 canto\u202E", "Il mio canto")]
    [InlineData("Canto 2", "Canto 2")]
    public void A_useful_title_is_kept_cleaned(string raw, string expected) =>
        Assert.Equal(expected, PdfDocumentTitle.Clean(raw));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\u0001\u0002")]
    [InlineData("12")]
    [InlineData("Microsoft Word - canto.docx")]
    [InlineData("Microsoft Word - Documento1")]
    [InlineData("Microsoft PowerPoint - canti")]
    [InlineData("canto.docx")]
    [InlineData("canto finale.PDF")]
    [InlineData("C:\\temp\\canto.rtf")]
    [InlineData("untitled")]
    [InlineData("Untitled 3")]
    [InlineData("Senza titolo")]
    [InlineData("Documento1")]
    [InlineData("Nuovo documento")]
    [InlineData("scan0034")]
    [InlineData("Credo_in_te_Frisina_Chitarra")]
    [InlineData("canto-di-lode-v2")]
    public void A_missing_or_useless_title_is_ignored(string? raw) =>
        Assert.Null(PdfDocumentTitle.Clean(raw));

    [Fact]
    public void A_very_long_title_is_cut_to_the_limit()
    {
        var title = PdfDocumentTitle.Clean(new string('a', 5000));

        Assert.Equal(PdfDocumentTitle.MaxLength, title!.Length);
    }

    [Fact]
    public void A_useless_metadata_title_does_not_name_an_untitled_sheet()
    {
        var page = SyntheticPages.Page(1, "   Do        Sol\nCanterò al nuovo giorno\n   La-       Fa\n   sopra il mare scuro");

        var result = PdfSongSplitter.Split(new[] { page }, "Microsoft Word - x.docx", "scan.pdf");

        Assert.Equal("Canterò al nuovo giorno", Assert.Single(result.Songs).Title);
    }
}
