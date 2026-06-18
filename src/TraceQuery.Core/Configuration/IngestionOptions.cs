using System;
using TraceQuery.Core.Model;

namespace TraceQuery.Core.Configuration;

/// <summary>
/// Captures the option knobs ingestion / querying / reporting reads.
/// Immutable, validated configuration type.
/// </summary>
public class IngestionOptions
{
    private const String DefaultCommentPrefix = "#";
    private const String DefaultFieldDelimiter = " | ";
    
    /// <summary>Entries classified below this are out of scope for later filtering.</summary>
    public Severity SeverityThreshold { get; init; } = Severity.Trace;

    /// <summary>Comment / non-record prefix. Must not be null, whitespace, or empty.</summary>
    public String CommentPrefix { get; init => field = EnsureStringKnob(value); } = DefaultCommentPrefix;

    /// <summary>Field delimiter used by the input format. Must not be null or empty.</summary>
    public String FieldDelimiter { get ; init => field = EnsureStringKnob(value, true); } = DefaultFieldDelimiter;

    /// <summary>Ensure string-type knob proposal is valid. Throws an exception otherwise.</summary>
    /// <param name="knob">Raw string-type knob proposal.</param>
    /// <param name="allowWhitespace">When true, whitespace as <paramref name="knob"/> is valid.</param>
    /// <returns>True if valid; otherwise throws exception.</returns>
    private static String EnsureStringKnob(
        String? knob,
        Boolean allowWhitespace = false
    )
    {
        Boolean isKnobValid =
            ( false != allowWhitespace )
            ? !String.IsNullOrEmpty(knob)
            : !String.IsNullOrWhiteSpace(knob);

        if ( false == isKnobValid )
        {
            throw new ArgumentException("Configuration was rejected. Please refer to the XML documentation for allowed values.");
        }
        
        return knob!;
    }
}
