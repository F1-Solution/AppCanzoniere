namespace AppCanzoniere.Application.Layout;

/// <summary>Whether chords are laid out over the lyrics or the lyrics alone.</summary>
public enum LayoutDisplayMode { ChordsAndLyrics, LyricsOnly }

/// <summary>Whether a chord box is plain text or padded like a pill.</summary>
public enum ChordBoxStyle { Plain, Pill }

/// <summary>The kind of a laid-out section.</summary>
public enum SectionKind { Verse, Chorus, Bridge, Tab, Comment }

/// <summary>The kind of string a run draws.</summary>
public enum RunKind { Lyric, Chord, Annotation, SectionLabel, Comment, Tab, LineNumber }

/// <summary>How a two-page spread advances: whole pairs or one page at a time.</summary>
public enum SpreadMode { Full, HalfTurn }
