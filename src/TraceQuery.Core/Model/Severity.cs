namespace TraceQuery.Core;

/// <summary>
/// Trace entry / Log entry severity levels enumeration.
/// Rationale for choosing `enum` type: Readability and enum being a value type, read is fast.
/// </summary>
public enum Severity
{
    /// <summary>Diagnostic / Trace severity level.</summary>
    Trace,

    /// <summary>Informational severity level.</summary>
    Info,

    /// <summary>Warning severity level.</summary>
    Warning,

    /// <summary>Error severity level.</summary>
    Error,

    /// <summary>Critical severity level.</summary>
    Critical
};
