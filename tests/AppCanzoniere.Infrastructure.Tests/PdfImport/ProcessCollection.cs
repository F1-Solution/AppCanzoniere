namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

// Tests that start dotnet processes run one at a time, so a slow start under load cannot push one past its time limit.
[CollectionDefinition(nameof(ProcessCollection), DisableParallelization = true)]
public sealed class ProcessCollection;
