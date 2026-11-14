namespace AppCanzoniere.Application.Layout;

/// <summary>The abstract text style a string is drawn in; the measurer maps it to a theme font and size.</summary>
public enum TextRole { Lyric, ChorusLyric, Chord, Annotation, SectionLabel, Comment, Tab, LineNumber }

/// <summary>Size of a string drawn in a role; LineHeight and Ascent do not depend on the text.</summary>
public readonly record struct TextMetrics(double Width, double LineHeight, double Ascent);

/// <summary>Resolves a role to the current theme font and size and measures text; bound to one font set.</summary>
public interface ITextMeasurer
{
    TextMetrics Measure(string text, TextRole role);
}
