using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Tests;

public class ChordNotationTests
{
    private static string ItToEn(string chord) =>
        ChordNotation.Convert(chord, NotationOptions.Italian, NotationOptions.English);

    private static string EnToIt(string chord) =>
        ChordNotation.Convert(chord, NotationOptions.English, NotationOptions.Italian);

    [Theory]
    [InlineData("Do", "C")]
    [InlineData("Re", "D")]
    [InlineData("Mi", "E")]
    [InlineData("Fa", "F")]
    [InlineData("Sol", "G")]
    [InlineData("La", "A")]
    [InlineData("Si", "B")]
    public void PlainRoots_ConvertBothWays(string italian, string english)
    {
        Assert.Equal(english, ItToEn(italian));
        Assert.Equal(italian, EnToIt(english));
    }

    [Theory]
    [InlineData("Do#", "C#")]
    [InlineData("Reb", "Db")]
    [InlineData("Mib", "Eb")]
    [InlineData("Fa#", "F#")]
    [InlineData("Solb", "Gb")]
    [InlineData("Sol#", "G#")]
    [InlineData("Lab", "Ab")]
    [InlineData("Sib", "Bb")]
    [InlineData("Do♯", "C#")]
    [InlineData("Si♭", "Bb")]
    [InlineData("Mi♭", "Eb")]
    public void Italian_Accidentals_AreKeptAndUnicodeSignsAreAccepted(string italian, string english)
    {
        Assert.Equal(english, ItToEn(italian));
    }

    [Theory]
    [InlineData("C#", "Do#")]
    [InlineData("Bb", "Sib")]
    [InlineData("Eb", "Mib")]
    [InlineData("F#", "Fa#")]
    [InlineData("Ab", "Lab")]
    [InlineData("B", "Si")]
    [InlineData("C♯", "Do#")]
    [InlineData("B♭", "Sib")]
    public void English_Accidentals_AreWrittenInItalian(string english, string italian)
    {
        Assert.Equal(italian, EnToIt(english));
    }

    [Fact]
    public void Si_IsBNatural_AndSib_IsBFlat_NotSiWithAStraySuffix()
    {
        Assert.Equal("B", ItToEn("Si"));
        Assert.Equal("Bb", ItToEn("Sib"));
        Assert.Equal("Bbm", ItToEn("Sibm"));
        Assert.Equal("Bm", ItToEn("Sim"));
        Assert.Equal("Bb7", ItToEn("Sib7"));
        Assert.Equal("B7", ItToEn("Si7"));
    }

    [Fact]
    public void Sol_IsGNotSiNorSoPlusL()
    {
        Assert.Equal("G", ItToEn("Sol"));
        Assert.Equal("G7", ItToEn("Sol7"));
        Assert.Equal("Gm", ItToEn("Solm"));
        Assert.Equal("Gb", ItToEn("Solb"));
        Assert.Equal("G/B", ItToEn("Sol/Si"));
    }

    [Theory]
    [InlineData("Lam", "Am")]
    [InlineData("Lam7", "Am7")]
    [InlineData("La-", "Am")]
    [InlineData("Re-7", "Dm7")]
    [InlineData("Mi-", "Em")]
    [InlineData("Fa#-", "F#m")]
    [InlineData("Sib-7", "Bbm7")]
    [InlineData("Dom", "Cm")]
    [InlineData("Rem", "Dm")]
    [InlineData("Mim", "Em")]
    [InlineData("Fa#m7", "F#m7")]
    [InlineData("Sibm", "Bbm")]
    public void Italian_Minors_AcceptLetterMAndDash(string italian, string english)
    {
        Assert.Equal(english, ItToEn(italian));
    }

    [Theory]
    [InlineData("Sol7", "G7")]
    [InlineData("Re4", "D4")]
    [InlineData("Dosus4", "Csus4")]
    [InlineData("Sibmaj7", "Bbmaj7")]
    [InlineData("Dodim", "Cdim")]
    [InlineData("Miaug", "Eaug")]
    [InlineData("Fa#dim7", "F#dim7")]
    [InlineData("Lasus2", "Asus2")]
    [InlineData("Redim", "Ddim")]
    [InlineData("Do6/9", "C6/9")]
    [InlineData("Rem7b5", "Dm7b5")]
    [InlineData("Doadd9", "Cadd9")]
    [InlineData("Domaj7", "Cmaj7")]
    public void Italian_Suffixes_AreKeptVerbatim(string italian, string english)
    {
        Assert.Equal(english, ItToEn(italian));
    }

    [Theory]
    [InlineData("Do/Mi", "C/E")]
    [InlineData("Lam/Sol", "Am/G")]
    [InlineData("Sib/Re", "Bb/D")]
    [InlineData("Sol7/Fa", "G7/F")]
    [InlineData("Fa#m/Do#", "F#m/C#")]
    [InlineData("La-/Sol", "Am/G")]
    public void Italian_SlashChords_ConvertTheBassToo(string italian, string english)
    {
        Assert.Equal(english, ItToEn(italian));
    }

    [Theory]
    [InlineData("D/F#", "Re/Fa#")]
    [InlineData("Am/G", "Lam/Sol")]
    [InlineData("Bb/D", "Sib/Re")]
    [InlineData("G7/F", "Sol7/Fa")]
    [InlineData("C6/9", "Do6/9")]
    public void English_SlashChords_ConvertTheBassToo(string english, string italian)
    {
        Assert.Equal(italian, EnToIt(english));
    }

    [Theory]
    [InlineData("LA", "A")]
    [InlineData("la", "A")]
    [InlineData("lA", "A")]
    [InlineData("SOL", "G")]
    [InlineData("sol7", "G7")]
    [InlineData("Lam", "Am")]
    [InlineData("lam", "Am")]
    [InlineData("mi", "E")]
    [InlineData("re-", "Dm")]
    [InlineData("do/MI", "C/E")]
    public void Italian_RootIsCaseInsensitive(string italian, string english)
    {
        Assert.Equal(english, ItToEn(italian));
    }

    [Theory]
    [InlineData("C", "Do")]
    [InlineData("Am", "Lam")]
    [InlineData("Dm7", "Rem7")]
    [InlineData("Em", "Mim")]
    [InlineData("F#m7", "Fa#m7")]
    [InlineData("Bbmaj7", "Sibmaj7")]
    [InlineData("Cdim", "Dodim")]
    [InlineData("Eaug", "Miaug")]
    [InlineData("Csus4", "Dosus4")]
    [InlineData("G7", "Sol7")]
    [InlineData("Ab/C", "Lab/Do")]
    [InlineData("am", "Lam")]
    [InlineData("Bm7b5", "Sim7b5")]
    public void English_Chords_ConvertToItalianWithLam(string english, string italian)
    {
        Assert.Equal(italian, EnToIt(english));
    }

    [Theory]
    [InlineData("Am", "La-")]
    [InlineData("Dm7", "Re-7")]
    [InlineData("Em/B", "Mi-/Si")]
    [InlineData("F#m", "Fa#-")]
    [InlineData("C", "Do")]
    [InlineData("G7", "Sol7")]
    [InlineData("Cmaj7", "Domaj7")]
    public void ItalianDash_WritesMinorsWithAHyphen(string english, string italian)
    {
        var result = ChordNotation.Convert(english, NotationOptions.English, NotationOptions.ItalianDash);

        Assert.Equal(italian, result);
    }

    [Theory]
    [InlineData("La-", "Lam")]
    [InlineData("Re-7", "Rem7")]
    [InlineData("Lam", "Lam")]
    public void Italian_DashToLetterM_WithinTheSameNotation(string input, string expected)
    {
        var result = ChordNotation.Convert(input, NotationOptions.ItalianDash, NotationOptions.Italian);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Lam", "La-")]
    [InlineData("Rem7", "Re-7")]
    public void Italian_LetterMToDash_WithinTheSameNotation(string input, string expected)
    {
        var result = ChordNotation.Convert(input, NotationOptions.Italian, NotationOptions.ItalianDash);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Am7")]
    [InlineData("F#m/C#")]
    [InlineData("Bbmaj7")]
    public void English_ToEnglish_IsIdentity(string chord)
    {
        Assert.Equal(chord, ChordNotation.Convert(chord, NotationOptions.English, NotationOptions.English));
    }

    [Theory]
    [InlineData("N.C.")]
    [InlineData("%")]
    [InlineData("|")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Bridge")]
    [InlineData("x")]
    [InlineData("C/H")]
    [InlineData("Am/")]
    [InlineData("/Do")]
    public void Unparseable_InEnglish_IsReturnedUnchanged(string text)
    {
        Assert.Equal(text, EnToIt(text));
    }

    [Theory]
    [InlineData("N.C.")]
    [InlineData("%")]
    [InlineData("|")]
    [InlineData("")]
    [InlineData("Dog")]
    [InlineData("Dod")]
    [InlineData("Dm")]
    [InlineData("Am7")]
    [InlineData("Lam/")]
    [InlineData("Sol/Xi")]
    public void Unparseable_InItalian_IsReturnedUnchanged(string text)
    {
        Assert.Equal(text, ItToEn(text));
    }

    [Fact]
    public void Convert_NullChord_DoesNotThrow()
    {
        Assert.Null(ChordNotation.Convert(null!, NotationOptions.Italian, NotationOptions.English));
    }

    [Fact]
    public void HyphenInLyrics_IsNeverTouched_WhileTheChordTokenIsConverted()
    {
        // Only chord tokens are passed to Convert; a lyric syllable like "la-la" is not a chord.
        Assert.Equal("Am", ItToEn("La-"));
        Assert.Equal("la-la", ItToEn("la-la"));
    }

    [Theory]
    [InlineData("Do")]
    [InlineData("Re7")]
    [InlineData("Sib/Re")]
    public void Italian_ToItalian_IsIdentityForCanonicalSpelling(string chord)
    {
        Assert.Equal(chord, ChordNotation.Convert(chord, NotationOptions.Italian, NotationOptions.Italian));
    }
}
