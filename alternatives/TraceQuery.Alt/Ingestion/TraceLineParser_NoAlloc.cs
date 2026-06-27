using System;
using TraceQuery.Core.Configuration;
using TraceQuery.Core.Model;

namespace TraceQuery.Alt.Ingestion;

/// <summary>Attempts to turns a raw line handed over by <see cref="TraceFileSource"/> into a <see cref="TraceEntry"/>.</summary>
public static class TraceLineParser_NoAlloc
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
        traceEntry = null; // Initialize out TraceEntry.
        
        return (
            TryCreateValidSpan(traceLine, out ReadOnlySpan<Char> traceSpan) // Store as span if not null or whitespace.
            && !traceSpan.StartsWith(options.CommentPrefix.AsSpan()) // Ensure not comment.
            && TryBuildEntry(traceSpan, options.FieldDelimiter, out traceEntry) // Try build trace entry.
        );

        // ------ Local functions ------

        static Boolean TryCreateValidSpan(String? traceLine, out ReadOnlySpan<Char> traceSpan)
        {
            Boolean isValid = !String.IsNullOrWhiteSpace(traceLine);
            traceSpan = isValid ? traceLine.AsSpan() : new();

            return isValid;
        }

        static Boolean TryBuildEntry(ReadOnlySpan<Char> traceSpan, String fieldDelimiter, out TraceEntry? traceEntry)
        {
            traceEntry = null; // Initialize out TraceEntry.
            
            // Get trace segments:
            Span<Range> segmentsMutable = stackalloc Range[ValidSegmentCount];

            int segmentCount = traceSpan.Split(segmentsMutable, fieldDelimiter.AsSpan());
            if ( ValidSegmentCount != segmentCount ) { return false; } // Early return if not valid.
            
            ReadOnlySpan<Range> segments = segmentsMutable; // Make span immutable.
            
            // Extract TraceEntry items:
            ReadOnlySpan<Char> timestampSpan = traceSpan[segments[0]].Trim();
            ReadOnlySpan<Char> severitySpan  = traceSpan[segments[1]].Trim();
            ReadOnlySpan<Char> componentSpan = traceSpan[segments[2]].Trim();
            ReadOnlySpan<Char> messageSpan   = traceSpan[segments[3]].Trim();
            
            // Validate timestamp:
            Boolean isValidTimestamp = DateTimeOffset.TryParseExact(
                timestampSpan,
                TimestampFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTimeOffset timestamp
            );

            if ( !isValidTimestamp ) { return false; } // Early return if not valid.

            // Validate severity:
            Boolean isValidSeverity = Severity.TryParse(severitySpan, IgnoreCase, out Severity severity);

            if ( !isValidSeverity ) { return false; } // Early return if not valid.
            
            // Instantiate TraceEntry:
            traceEntry = new(
                timestamp,
                severity,
                componentSpan.ToString(),
                messageSpan.ToString()
            );

            return true;
        }
    }
}
