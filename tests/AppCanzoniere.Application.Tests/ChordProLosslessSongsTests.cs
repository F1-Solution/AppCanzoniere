using AppCanzoniere.Application.Music;
using static AppCanzoniere.Application.Tests.LosslessAssert;

namespace AppCanzoniere.Application.Tests;

/// <summary>Whole-song inputs in the styles of SongbookPro, OnSong exports, chordpro.org examples and Italian liturgical sheets.</summary>
public class ChordProLosslessSongsTests
{
    public static IEnumerable<object[]> Songs() => new[]
    {
        // SongbookPro-like
        "{title: Amazing Grace}\n{artist: John Newton}\n{key: G}\n{tempo: 72}\n{time: 3/4}\n\n{start_of_verse: Verse 1}\n[G]Amazing [G7]grace, how [C]sweet the [G]sound\nThat [G]saved a [Em]wretch like [D]me\n{end_of_verse}\n\n{start_of_chorus}\n[C]Chorus line [G]here\n{end_of_chorus}\n",
        "{t: Simple}\n{st: Sub}\n{c: Intro}\n[Am]La la [F]la\n{c: Outro}\n[G]La [Am]la\n",
        "{title: With Capo}\n{key: A}\n{capo: 2}\n[G]Shapes for [C]capo two\n\n[D]Next [Em]line\n",
        "{title: Bridge Song}\n{sob: Bridge}\n[Dm]Bridge [G]line\n{eob}\n{sov: Verse 2}\n[C]V2 line\n{eov}\n",
        // OnSong-export-like
        "{title: OnSong Style}\n{artist: Someone}\n{meta: tag Worship}\n{key: D}\n{tempo: 120}\n{time: 4/4}\n{duration: 3:45}\n{copyright: 2001 Some Publisher}\n{year: 2001}\n\n{comment: Verse 1}\n[D]Line one [A]here\n[Bm]Line two [G]here\n\n{comment: Chorus}\n[G]Sing it [D]loud\n",
        "{title: OnSong Two}\n{subtitle: Alt}\n{composer: A. Writer}\n{lyricist: B. Writer}\n{album: The Album}\n{x_custom: value with: colon}\n[C]Text\n",
        "{title: Chords Directive}\n{define: Am base-fret 1 frets x 0 2 2 1 0}\n{chord: Am}\n[Am]Line\n",
        // chordpro.org examples style
        "{title: Swing Low Sweet Chariot}\n{subtitle: Traditional}\n\n{start_of_chorus}\nSwing [D]low, sweet [G]chari[D]ot,\nComin' for to carry me [A7]home.\nSwing [D7]low, sweet [G]chari[D]ot,\nComin' for to [A7]carry me [D]home.\n{end_of_chorus}\n\nI [D]looked over Jordan, and [G]what did I [D]see\n",
        "# ChordPro example\n{title: Comments}\n{comment: A comment}\n{comment_italic: An italic comment}\n{comment_box: A boxed comment}\n{highlight: Highlighted}\n{ci: short}\n{cb: short}\n",
        "{title: Layout}\n{columns: 2}\n{textsize: 12}\n{textfont: Times}\n{chordcolour: blue}\n{new_page}\n{np}\n{column_break}\n{colb}\n[C]Text\n",
        "{title: Recall}\n{soc}\n[C]Chorus\n{eoc}\n[G]Verse\n{chorus}\n{chorus: Again}\n",
        "{title: Grid}\n{start_of_grid}\n| Am . . . | C . . . |\n| F . . . | G . . . |\n{end_of_grid}\n",
        "{title: Tab}\n{start_of_tab}\ne|---0---|\nB|---1---|\n{end_of_tab}\n{sot: Riff 2}\ne|-3-|\n{eot}\n",
        "{comment-guitar: Capo 2}\n{comment-piano: Left hand}\n{define-guitar: Am}\n[Am]x\n",
        // Italian liturgical
        "{title: Resta con noi, Signore}\n{artist: Anonimo}\n{key: Lam}\n\n{start_of_verse: Strofa 1}\n[La-]Resta con [Mi]noi, Signo[La-]re\n{end_of_verse}\n\n{start_of_chorus: Rit.}\n[Re-]Resta con noi, [Mi7]resta con [La-]noi\n{end_of_chorus}\n",
        "{t: Ave Maria}\n{soc: Rit.}\n[Do]Ave, [Sol]Ave, [Fa]Ave Ma[Do]ria\n{eoc}\n{sov: Strofa}\n[Lam]Quando il [Fa]cielo si [Sol]fa buio\n{eov}\n",
        "{title: Rit. in mezzo}\n[C]Prima strofa\n{soc: Rit.}\n[F]Ritornello\n\n{c: due volte}\n[G]Ancora\n{eoc}\n[C]Seconda strofa\n{chorus: Rit.}\n",
        "{title: Magnificat}\n{key: Re}\n{capo: 3}\n{tempo: 80}\n  [Re]L'anima mia magni[Sol]fica il Si[Re]gnore  \n\n  [La]il mio spirito esulta in [Re]Dio\n",
        // Messy, real-world
        "﻿{title: With BOM}\r\n{key: C}\r\n[C]Line\r\n",
        "Title Line Without Directive\nArtist\n\n[C]Lyrics start\n{c: later directive}\n",
        "{title: Spaces}\n{ key : C }\n{ capo : 1 }\n{ soc : Rit. }\n [C] spaced [ G ] chords \n{ eoc }\n",
        "{SOC}\n[C]Upper\n{EOC}\n{Sot}\ntab\n{Eot}\n{T: Late Title}\n{KEY: D}\n",
        "{title: Odd Chords}\n[N.C.]Rest [%] repeat [Intro] label [x] [] [C\nUnclosed [ and ] reversed [G]ok\n",
        "{title:T}\n{title:U}\n{key:C}\n{key:D}\n{capo:1}\n{capo:2}\n[C]Duplicated header\n",
        "{title: Tabs and spaces}\n\t[C]\tindented\twith tabs\t\n    [G]four spaces   \n",
        "{start_of_chorus}\n{start_of_tab}\ne|--0--|\n{end_of_tab}\n{end_of_chorus}\n[C]After\n",
        "{title: Empty values}\n{subtitle:}\n{artist:}\n{key:}\n{capo:}\n{comment:}\n{define:}\n[C]x\n",
        "[C]Only lyrics, no header\n[G]Second line\n[Am]Third line",
        "{title: Directives only}\n{artist: A}\n{key: C}\n{capo: 3}\n{tempo: 100}\n{time: 6/8}\n",
        "{title: Blank runs}\n\n\n\n[C]a\n\n\n[G]b\n\n\n"
    }.Select(s => new object[] { s });

    [Theory]
    [MemberData(nameof(Songs))]
    public void Song_RoundTripsByteForByte(string song) => AssertLossless(song);

    [Theory]
    [MemberData(nameof(Songs))]
    public void Song_RenderIsIdempotent(string song)
    {
        var once = RoundTrip(song);

        Assert.Equal(once, RoundTrip(once));
        Assert.Equal(once, RoundTrip(RoundTrip(once)));
    }

    [Fact]
    public void TheSongList_HasAtLeastTwentyFiveEntries() => Assert.True(Songs().Count() >= 25);

    [Theory]
    [MemberData(nameof(Songs))]
    public void Song_TransposedByTwo_ChangesOnlyChordsKeyAndNothingElse(string song)
    {
        var original = Expected(song).Split('\n');
        var shifted = Render(song, new ChordProRenderOptions { TransposeSteps = 2 }).Split('\n');

        Assert.Equal(original.Length, shifted.Length);
        for (var i = 0; i < original.Length; i++)
        {
            if (original[i] == shifted[i])
                continue;

            Assert.True(
                MentionsAChordOrKey(original[i]),
                $"Line {i + 1} changed but holds no chord or key: '{original[i]}' became '{shifted[i]}'.");
            Assert.Equal(StripChordsAndKeyValue(original[i]), StripChordsAndKeyValue(shifted[i]));
        }
    }

    private static bool MentionsAChordOrKey(string line) =>
        line.Contains('[') || System.Text.RegularExpressions.Regex.IsMatch(line, @"^﻿?\s*\{\s*key\s*:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    private static string StripChordsAndKeyValue(string line)
    {
        var noChords = System.Text.RegularExpressions.Regex.Replace(line, @"\[[^\]]*\]", "[]");
        return System.Text.RegularExpressions.Regex.Replace(noChords, @"^(﻿?\s*\{\s*key\s*:).*$", "$1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}
