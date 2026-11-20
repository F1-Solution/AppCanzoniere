using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.ImportReview;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.Tests.ImportReview;

public sealed class ImportMessagesTests
{
    [Fact]
    public void Every_warning_has_an_italian_label_except_the_informative_one()
    {
        foreach (var w in Enum.GetValues<SheetWarning>().Where(w => w != SheetWarning.ChordInsideWord))
            Assert.False(string.IsNullOrWhiteSpace(ImportMessages.WarningLabel(w)), w.ToString());

        Assert.Null(ImportMessages.WarningLabel(SheetWarning.ChordInsideWord));
    }

    [Fact]
    public void Ocr_unavailable_text_depends_on_platform()
    {
        var win = ImportMessages.ErrorFor(SongbookImportFailure.OcrUnavailable, ImportPlatform.Windows);
        var and = ImportMessages.ErrorFor(SongbookImportFailure.OcrUnavailable, ImportPlatform.Android);

        Assert.Contains("lingua italiana", win.Explanation);
        Assert.Contains("Riavvia", and.Explanation);
    }

    [Theory]
    [InlineData(1, 1, "p. 1")]
    [InlineData(4, 5, "p. 4–5")]
    public void Page_range_text(int first, int last, string expected)
    {
        Assert.Equal(expected, ImportMessages.PageRange(first, last));
    }

    [Fact]
    public void Count_phrases_use_singular_and_plural()
    {
        Assert.Equal("1 canto", ImportMessages.Songs(1));
        Assert.Equal("2 canti", ImportMessages.Songs(2));
    }

    [Fact]
    public void Duration_is_formatted_m_ss()
    {
        Assert.Equal("0:48", ImportMessages.Duration(TimeSpan.FromSeconds(48)));
        Assert.Equal("12:05", ImportMessages.Duration(TimeSpan.FromSeconds(725)));
    }
}
