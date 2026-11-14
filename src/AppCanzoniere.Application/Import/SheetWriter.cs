namespace AppCanzoniere.Application.Import;

/// <summary>ChordPro text plus, per input line, the output line and the final warnings.</summary>
internal sealed record WrittenSheet(string ChordPro, IReadOnlyList<int> OutputLines, IReadOnlyList<IReadOnlyList<SheetWarning>> Warnings, int MergedCount);

/// <summary>Assembles the ChordPro output: header directives, sections, merged chord lines (spec 5.3, 6.2, 7).</summary>
internal sealed class SheetWriter
{
    private static readonly MetaKind[] HeaderOrder =
    {
        MetaKind.Title, MetaKind.Artist, MetaKind.Lyricist, MetaKind.Composer, MetaKind.Key, MetaKind.Capo, MetaKind.Tempo, MetaKind.Time
    };

    private readonly IReadOnlyList<AnalyzedLine> _lines;
    private readonly int _snapColumns;
    private readonly List<string> _header = new();
    private readonly List<string> _body = new();
    private readonly List<SheetWarning>[] _warnings;
    private readonly int[] _headerIndex;
    private readonly int[] _bodyIndex;
    private SectionKind? _open;
    private bool _chorusDefined;
    private int _pendingIndex = -1;
    private int _merged;

    private SheetWriter(IReadOnlyList<AnalyzedLine> lines, int snapColumns)
    {
        _lines = lines;
        _snapColumns = snapColumns;
        _warnings = lines.Select(l => l.Warnings.ToList()).ToArray();
        _headerIndex = Enumerable.Repeat(-1, lines.Count).ToArray();
        _bodyIndex = Enumerable.Repeat(-1, lines.Count).ToArray();
    }

    public static WrittenSheet Write(IReadOnlyList<AnalyzedLine> lines, int snapColumns)
    {
        var writer = new SheetWriter(lines, snapColumns);
        writer.BuildHeader();
        writer.BuildBody();
        return writer.Finish();
    }

    private void BuildHeader()
    {
        var directives = new Dictionary<MetaKind, (string Text, int Index)>();
        var comments = new List<(string Text, int Index)>();
        foreach (var line in _lines.Where(l => l.Meta is not null))
        {
            var meta = line.Meta!;
            if (meta.Kind == MetaKind.Comment)
                comments.Add(($"{{comment:{Safe(meta.Value, line.Index)}}}", line.Index));
            else if (directives.ContainsKey(meta.Kind))
                comments.Add(($"{{comment:{Safe(line.Source.Text.Trim(), line.Index)}}}", line.Index));
            else
                directives[meta.Kind] = ($"{{{meta.Kind.ToString().ToLowerInvariant()}:{Safe(meta.Value, line.Index)}}}", line.Index);
        }

        foreach (var kind in HeaderOrder.Where(directives.ContainsKey))
            AddHeader(directives[kind]);
        foreach (var comment in comments)
            AddHeader(comment);

        if (directives.TryGetValue(MetaKind.Key, out var key) && directives.ContainsKey(MetaKind.Capo))
            _warnings[key.Index].Add(SheetWarning.KeyAndCapo);
    }

    private string Safe(string value, int line)
    {
        var safe = TextSafety.Replace(value);
        if (safe != value)
            _warnings[line].Add(SheetWarning.BracketsReplaced);
        return safe;
    }

    private void AddHeader((string Text, int Index) entry)
    {
        _header.Add(entry.Text);
        _headerIndex[entry.Index] = _header.Count - 1;
    }

    private void BuildBody()
    {
        for (var i = 0; i < _lines.Count; i++)
        {
            switch (_lines[i].Kind)
            {
                case SheetLineKind.Blank:
                    CloseSection();
                    AddBlank(i);
                    break;
                case SheetLineKind.Tab:
                    EmitTab(i);
                    break;
                case SheetLineKind.Metadata or SheetLineKind.Title:
                    break;
                case SheetLineKind.SectionLabel:
                    EmitLabel(i);
                    EmitContent(i);
                    break;
                default:
                    EmitContent(i);
                    break;
            }
        }

        CloseSection();
        while (_body.Count > 0 && _body[^1].Length == 0)
            _body.RemoveAt(_body.Count - 1);
    }

    private int AddBody(int line, string text)
    {
        _body.Add(text);
        _bodyIndex[line] = _body.Count - 1;
        return _body.Count - 1;
    }

    private void AddBlank(int line)
    {
        if (_body.Count == 0 || _body[^1].Length == 0)
            _bodyIndex[line] = Math.Max(_body.Count - 1, 0);
        else
            AddBody(line, string.Empty);
    }

    private void CloseSection()
    {
        if (_open is { } section)
            _body.Add($"{{end_of_{section.ToString().ToLowerInvariant()}}}");
        _open = null;
    }

    private void EmitTab(int i)
    {
        if (i == 0 || _lines[i - 1].Kind != SheetLineKind.Tab)
        {
            CloseSection();
            _body.Add("{start_of_tab}");
        }

        AddBody(i, _lines[i].Source.Text);
        if (i == _lines.Count - 1 || _lines[i + 1].Kind != SheetLineKind.Tab)
            _body.Add("{end_of_tab}");
    }

    private void EmitLabel(int i)
    {
        var line = _lines[i];
        var label = line.Label!;
        var hasContent = line.Content is not null || NextHasContent(i);
        CloseSection();

        var suffix = label.Value is null ? string.Empty : ":" + label.Value;
        if (label.Kind == SectionKind.CommentOnly || (!hasContent && label.Kind != SectionKind.Chorus))
        {
            AddBody(i, $"{{comment:{label.Text.TrimEnd(':')}}}");
        }
        else if (!hasContent)
        {
            if (!_chorusDefined)
                _warnings[i].Add(SheetWarning.ChorusRecallWithoutChorus);
            AddBody(i, "{chorus" + suffix + "}");
        }
        else
        {
            var name = label.Kind.ToString().ToLowerInvariant();
            AddBody(i, $"{{start_of_{name}{suffix}}}");
            _open = label.Kind;
            _chorusDefined |= label.Kind == SectionKind.Chorus;
        }
    }

    private bool NextHasContent(int i) =>
        i + 1 < _lines.Count && _lines[i + 1].Kind is not (SheetLineKind.Blank or SheetLineKind.SectionLabel or SheetLineKind.Tab);

    // A lyric made only of marks, numbers or note names ("Re Re") takes the chords only when each one lands on a letter, else it would read as a chord line again.
    private bool NextIsLyric(int i) =>
        i + 1 < _lines.Count && _lines[i + 1].Content is { Kind: UnitKind.Lyric } next &&
        next.Class.Tokens.Any(t => t.Kind != TokenKind.Neutral) && next.Text.Any(char.IsLetterOrDigit) &&
        (HasWordInBothNotations(next) || EveryChordLandsOnALetter(_lines[i].Content!, next)) &&
        MetadataParser.TryParse(next.Text.Trim()) is null;

    private static bool HasWordInBothNotations(ContentUnit lyric) =>
        new[] { Music.Notation.Italian, Music.Notation.English }.All(n => SheetTokenizer.Tokenize(lyric.Text, lyric.Start, n).Any(t => t.Kind == TokenKind.Word));

    // A bracketed chord line may be the line the first conversion kept apart, so it stays apart again.
    private bool EveryChordLandsOnALetter(ContentUnit chords, ContentUnit lyric)
    {
        if (chords.Class.Tokens.Any(t => t.Bracketed))
            return false;

        var merged = ChordMerger.Merge(chords, lyric, _snapColumns).Line;
        return BracketedText.Matches(merged).All(m => m.Index + m.Length < merged.Length && char.IsLetter(merged[m.Index + m.Length]));
    }

    private bool NextIsInline(int i) =>
        i + 1 < _lines.Count && _lines[i + 1].Content is { Kind: UnitKind.Inline };

    private void EmitContent(int i)
    {
        if (_lines[i].Content is not { } unit)
            return;

        if (_pendingIndex >= 0)
            EmitMerged(i, unit);
        else if (unit.Kind != UnitKind.Chord)
            AddBody(i, Escape(i, unit.OutputText));
        else if (NextIsLyric(i))
            _pendingIndex = i;
        else
        {
            NoteParentheses(i, unit);
            if (NextIsInline(i))
                _warnings[i].Add(SheetWarning.ChordLineKeptAlone);
            AddBody(i, ChordMerger.Instrumental(unit));
        }
    }

    private void EmitMerged(int i, ContentUnit lyric)
    {
        var chordUnit = _lines[_pendingIndex].Content!;
        var merged = ChordMerger.Merge(chordUnit, lyric, _snapColumns);
        var index = AddBody(i, Escape(i, merged.Line));
        _bodyIndex[_pendingIndex] = index;
        _warnings[_pendingIndex].AddRange(merged.Warnings);
        NoteParentheses(_pendingIndex, chordUnit);
        _pendingIndex = -1;
        _merged++;
    }

    private void NoteParentheses(int line, ContentUnit unit)
    {
        if (unit.Class.Tokens.Any(t => t.Parenthesised))
            _warnings[line].Add(SheetWarning.ParenthesesRemoved);
    }

    private string Escape(int line, string text)
    {
        if (!text.StartsWith('#'))
            return text;

        _warnings[line].Add(SheetWarning.EscapedLyric);
        return " " + text;
    }

    private WrittenSheet Finish()
    {
        var separator = _header.Count > 0 && _body.Count > 0 ? 1 : 0;
        var output = _header.Concat(Enumerable.Repeat(string.Empty, separator)).Concat(_body).ToList();
        var last = Math.Max(output.Count - 1, 0);
        var lines = Enumerable.Range(0, _lines.Count)
            .Select(i => Math.Clamp(_headerIndex[i] >= 0 ? _headerIndex[i] : _bodyIndex[i] + _header.Count + separator, 0, last))
            .ToList();
        var text = output.Count == 0 ? string.Empty : string.Join("\n", output) + "\n";
        return new WrittenSheet(text, lines, _warnings.Select(w => (IReadOnlyList<SheetWarning>)w.Distinct().ToList()).ToList(), _merged);
    }
}
