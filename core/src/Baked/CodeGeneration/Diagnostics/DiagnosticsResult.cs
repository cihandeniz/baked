namespace Baked.CodeGeneration.Diagnostics;

public record DiagnosticsResult(
    IReadOnlyCollection<Exception> Errors,
    IReadOnlyCollection<DiagnosticMessage> Messages
);