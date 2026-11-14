namespace AppCanzoniere.Application.Layout;

/// <summary>Fails a layout whose measurer or options were so large that a coordinate overflowed.</summary>
internal static class FiniteCoordinates
{
    public static void Require(double height, IEnumerable<LayoutSection> sections)
    {
        Check(height);
        foreach (var section in sections)
        {
            Check(section.Y, section.Height, section.TabScale);
            foreach (var run in section.LabelRuns)
                Check(run);

            foreach (var line in section.Lines)
            {
                Check(line.Y, line.Height);
                foreach (var row in line.Rows)
                {
                    Check(row.Y, row.Height);
                    foreach (var run in row.Runs)
                        Check(run);
                }
            }
        }
    }

    private static void Check(LayoutRun run) =>
        Check(run.X, run.Y, run.Width, run.Height, run.TextX, run.BaselineY);

    private static void Check(params double[] values)
    {
        if (values.Any(v => !double.IsFinite(v)))
            throw new InvalidOperationException("The layout has a coordinate that is not finite: the measurer or an option value is too large.");
    }
}
