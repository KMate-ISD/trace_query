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
    public String CommentPrefix { get; init => field = EnsureValidStringTypeKnob(value); } = DefaultCommentPrefix;

    /// <summary>Field delimiter used by the input format. Must not be null, whitespace, or empty.</summary>
    public String FieldDelimiter { get ; init => field = EnsureValidStringTypeKnob(value); } = DefaultFieldDelimiter;

    /// <summary>Ensure string-type knob proposal is valid. Throws an exception otherwise.</summary>
    /// <param name="knob">Raw string-type knob proposal.</param>
    /// <returns>Input knob or throws exception.</returns>
    private static String EnsureValidStringTypeKnob(String? knob)
    {
        if ( false == String.IsNullOrEmpty(knob) )
        {
            return knob;
        }
        else
        {
            throw new ArgumentException("Config is rejected: proposed knob is invalid.");
        }
    }
}
