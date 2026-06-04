using System;
using System.Runtime.CompilerServices;
using TraceQuery.Core.Model;

namespace TraceQuery.Core.Querying;

/// <summary>
/// Captures the option knobs ingestion / querying / reporting reads.
/// Immutable, validated configuration type.
/// </summary>
public class Options
{
    private const String _defaultCommentPrefix = "#";
    private const String _defaultFieldDelimiter = " | ";

    /// <summary>One-param constructor for Options class.</summary>
    /// <param name="severityThreshold">Entries classified below this are out of scope for later filtering.</param>
    public Options(Severity? severityThreshold)
    {
        SeverityThreshold = severityThreshold ?? Severity.Trace;
    }
    
    /// <summary>Entries classified below this are out of scope for later filtering.</summary>
    public Severity SeverityThreshold { get; init; } = Severity.Trace;

    /// <summary>Comment / non-record prefix. Defaults to match TRACE_FORMAT.md v1.</summary>
    public String CommentPrefix { get; init => field = GetValidCommentPrefix(value); } = _defaultCommentPrefix;

    /// <summary>Field delimiter used by the input format. Defaults to match TRACE_FORMAT.md v1.</summary>
    public String FieldDelimiter { get ; init => field = GetValidFieldDelimiter(value); } = _defaultFieldDelimiter;

    /// <summary>Validate string-type knob proposal and produce a knob that is always valid.</summary>
    /// <param name="knob">Raw string-type knop proposal.</param>
    /// <param name="fallback">Knob fallback.</param>
    /// <returns>Either input knob, if valid, or fallback.</returns>
    protected internal static String GetValidStringTypeKnob(String? knob, String fallback)
    {
        String validKnob = fallback;

        if ( false != _validate(knob) )
        {
            validKnob = knob!;
        }

        return validKnob;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static String GetValidCommentPrefix(String? commentPrefix)
    {
        return GetValidStringTypeKnob(commentPrefix, _defaultCommentPrefix);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static String GetValidFieldDelimiter(String? fieldDelimiter)
    {
        return GetValidStringTypeKnob(fieldDelimiter, _defaultFieldDelimiter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Boolean _validate(String? input)
    {
        return (    ( false == String.IsNullOrEmpty(input) )
                 && ( false == input.IsWhiteSpace()        ) );
    }
}
