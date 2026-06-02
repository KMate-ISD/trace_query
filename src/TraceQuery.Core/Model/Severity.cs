namespace TraceQuery.Core.Model;

/// <summary>
/// Trace entry / Log entry severity levels enumeration.
/// Rationale of `enum` as type choice: modeling;
/// type-safe constants for a finite set of categories.
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
