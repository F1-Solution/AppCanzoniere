using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.PdfImport;

public sealed class PdfDocumentTitleFilterTests
{
    [Theory]
    [InlineData("Messa di S.Rita")]
    [InlineData("Canti vol.12")]
    [InlineData("Ave Maria - J.S.Bach")]
    [InlineData("Lui/Lei")]
    [InlineData("Dio e amore / Inno")]
    public void A_real_title_with_dots_or_slashes_is_kept(string raw) =>
        Assert.Equal(raw, PdfDocumentTitle.Clean(raw));

    [Theory]
    [InlineData("canto.pdf")]
    [InlineData("canto.DOCX")]
    [InlineData("canto.mscz")]
    [InlineData("canto.tiff")]
    [InlineData("C:\\temp\\canto")]
    [InlineData("/home/utente/canto")]
    [InlineData("cartella/sotto/canto.txt")]
    [InlineData("\\\\server\\share\\canto")]
    public void A_file_name_or_a_path_is_ignored(string raw) =>
        Assert.Null(PdfDocumentTitle.Clean(raw));

    [Fact]
    public void Cutting_a_long_title_never_splits_a_surrogate_pair()
    {
        var raw = new string('a', PdfDocumentTitle.MaxLength - 1) + "\U0001D11E" + "bcd";

        var title = PdfDocumentTitle.Clean(raw)!;

        Assert.False(char.IsHighSurrogate(title[^1]));
        Assert.Equal(new string('a', PdfDocumentTitle.MaxLength - 1), title);
    }
}
