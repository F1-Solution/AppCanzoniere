using AppCanzoniere.Application.Import;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 9.1 cases 39-43 and the metadata rules of section 7.
public class MetadataTests
{
    [Fact]
    public void Case39_MetadataBlock_BecomesDirectivesBeforeTheBody()
    {
        var result = Run("Titolo: Canto della sera", "Tonalità: Re", "Capo 2", "Tempo: 4/4", "", "Do Sol", "Ti lodiamo");

        Assert.Equal(
            Out("{title:Canto della sera}", "{key:Re}", "{capo:2}", "{time:4/4}", "", "[Do]Ti [Sol]lodiamo"),
            result.ChordPro);
        Assert.Contains(SheetWarning.KeyAndCapo, result.Diag(2).Warnings);
        Assert.Equal(SheetLineKind.Metadata, result.Diag(2).Kind);
    }

    [Theory]
    [InlineData("Capotasto: II", "{capo:2}")]
    [InlineData("capo III", "{capo:3}")]
    [InlineData("Capotasto al 3° tasto", "{capo:3}")]
    public void Case40_CapoForms(string line, string directive) =>
        Assert.Equal(Out(directive), Run(line).ChordPro);

    [Fact]
    public void Case40_CapoOutOfRange_IsAnUnreadableComment()
    {
        var result = Run("Capo 12");

        Assert.Equal(Out("{comment:Capo 12}"), result.ChordPro);
        Assert.Contains(SheetWarning.UnreadableMetadata, result.Diag(1).Warnings);
    }

    [Theory]
    [InlineData("Tempo: 72", "{tempo:72}")]
    [InlineData("Key: D", "{key:D}")]
    [InlineData("Tonalità: La minore", "{key:Lam}")]
    [InlineData("Tonalità: Re maggiore", "{key:Re}")]
    [InlineData("Titolo: Santo", "{title:Santo}")]
    [InlineData("Autore: Mario Rossi", "{artist:Mario Rossi}")]
    [InlineData("Testo e musica di Mario Rossi", "{artist:Mario Rossi}")]
    [InlineData("Testo: Anna Verdi", "{lyricist:Anna Verdi}")]
    [InlineData("Musica: Carlo Neri", "{composer:Carlo Neri}")]
    [InlineData("Time: 3/4", "{time:3/4}")]
    public void Case41_TempoAndKeyForms(string line, string directive) =>
        Assert.Equal(Out(directive), Run(line).ChordPro);

    [Fact]
    public void Case41_UnreadableKey_IsAComment()
    {
        var result = Run("Tonalità: boh");

        Assert.Equal(Out("{comment:Tonalità: boh}"), result.ChordPro);
        Assert.Contains(SheetWarning.UnreadableMetadata, result.Diag(1).Warnings);
    }

    [Fact]
    public void Case42_TitleGuess_UsesTheFirstLine()
    {
        var result = Run("CANTO DELLA SERA", "", "Do Sol", "Ti lodiamo");

        Assert.Equal(Out("{title:CANTO DELLA SERA}", "", "[Do]Ti [Sol]lodiamo"), result.ChordPro);
        Assert.Equal(SheetLineKind.Title, result.Diag(1).Kind);
        Assert.Equal(0.7, result.Diag(1).Confidence);
    }

    [Fact]
    public void Case43_NoTitleGuess_WhenTheNextLineIsALyric() =>
        Assert.Equal(Out("Ti lodiamo", "Signore"), Run("Ti lodiamo", "Signore").ChordPro);

    [Fact]
    public void NoTitleGuess_WhenAnExplicitTitleExists() =>
        Assert.Equal(Out("{title:Santo}", "", "Prima riga", "", "Seconda"), Run("Titolo: Santo", "", "Prima riga", "", "Seconda").ChordPro);

    [Fact]
    public void RepeatedMetadata_KeepsTheFirstAndCommentsTheRest() =>
        Assert.Equal(Out("{key:Re}", "{comment:Key: Mi}"), Run("Key: Re", "Key: Mi").ChordPro);

    [Fact]
    public void MetadataAfterTheHeader_IsPlainText() =>
        Assert.Equal(
            Out("[Do]Ti [Sol]lodiamo", "Key: Re"),
            Run("Do Sol", "Ti lodiamo", "Key: Re").ChordPro);

    [Fact]
    public void KeywordWithoutColonAndNoValue_IsNotMetadata() =>
        Assert.Equal(Out("Tono di lode", "Signore"), Run("Tono di lode", "Signore").ChordPro);
}
