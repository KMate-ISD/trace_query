using System;
using TraceQuery.Core.Configuration;
using TraceQuery.Core.Model;

namespace TraceQuery.Alt.Ingestion;

/// <summary>Attempts to turns a raw line handed over by <see cref="TraceFileSource"/> into a <see cref="TraceEntry"/>.</summary>
public static class TraceLineParser_Simple
{
    const int ValidSegmentCount = 4; // A well formed trace line has 4 segments per TRACE_FORMAT.md.
    const Boolean IgnoreCase = true;
    const String TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

    /// <summary>Tries parsing a line provided by <see cref="TraceFileSource"/>.</summary>
    /// <param name="traceLine">The line to be parsed.</param>
    /// <param name="options">The configuration options for ingestion.</param>
    /// <param name="traceEntry">The parsed <see cref="TraceEntry"/> instance. Null on failure.</param>
    /// <returns>True on success.</returns>
    public static Boolean TryParseTraceLine(this String? traceLine, IngestionOptions options, out TraceEntry? traceEntry)
    {
        traceEntry = null;

        return(
            !String.IsNullOrWhiteSpace(traceLine) // Null-check.
            && !traceLine!.StartsWith(options.CommentPrefix) // Check if comment.
            && TryGetSegments(traceLine, options, out String[] tracelineSegments) // Check if well formed.
            && TryBuildEntry(tracelineSegments, out traceEntry) // Instantiate TraceEntry, if traceLine source is valid.
        );

        // ------ Local functions ------

        static Boolean TryGetSegments(String traceLine, IngestionOptions options, out String[] traceLineSegments)
        {
            traceLineSegments = traceLine.Split(options.FieldDelimiter, ValidSegmentCount);
            return ( ValidSegmentCount == traceLineSegments.Length ); // True if segment count is of a well formed trace entry.
        }

        static Boolean TryBuildEntry(String[] traceLineSegments, out TraceEntry? traceEntry)
        {
            traceEntry = null; // Initialize out parameter to null.

            ReadOnlySpan<Char> timestamp = traceLineSegments[(int)TraceLineSegmentIndices.Timestamp].AsSpan().Trim();
            ReadOnlySpan<Char> severity  = traceLineSegments[(int)TraceLineSegmentIndices.Severity].AsSpan().Trim();
            ReadOnlySpan<Char> component = traceLineSegments[(int)TraceLineSegmentIndices.Component].AsSpan().Trim();
            ReadOnlySpan<Char> message   = traceLineSegments[(int)TraceLineSegmentIndices.Message].AsSpan().Trim();
            
            Boolean isValidTimestamp = DateTimeOffset.TryParseExact(
                timestamp,
                TimestampFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTimeOffset validatedTimestamp
            ); // Validate timestamp.
            
            Boolean isValidSeverity = Severity.TryParse(severity, IgnoreCase, out Severity validatedSeverity); // Validate severity.

            Boolean isValidTraceLine = ( isValidTimestamp && isValidSeverity ); // True if TraceEntry instance can be built.

            if ( false != isValidTraceLine )
            {
                traceEntry = new TraceEntry(
                    validatedTimestamp,
                    validatedSeverity,
                    component.ToString(),
                    message.ToString()
                );
            }

            return isValidTraceLine;
        }
    }

    private enum TraceLineSegmentIndices
    {
        Timestamp,
        Severity,
        Component,
        Message,
    } // Segment indices of intermediary String array during parsing a raw trace line.
}
