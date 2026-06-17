using System;
using TraceQuery.Core.Configuration;
using TraceQuery.Core.Model;

namespace TraceQuery.Core.Ingestion;

/// <summary>Attempts to turns a raw line handed over by <see cref="TraceFileSource"/> into a <see cref="TraceEntry"/>.</summary>
public static class TraceLineParser
{
    private const int NoIndex = -1; // Marks that a certain index doesn't exist.
    private const int ValidDelimiterCount = 3; // A well formed trace line has 4 segments per TRACE_FORMAT.md.

    /// <summary>Tries parsing a line provided by <see cref="TraceFileSource"/>.</summary>
    /// <param name="traceLine">The line to be parsed.</param>
    /// <param name="options">The configuration options for ingestion.</param>
    /// <param name="traceEntry">The parsed <see cref="TraceEntry"/> instance. Null on failure.</param>
    /// <returns>True on success.</returns>
    public static Boolean TryParseTraceLine
    (
        this String? traceLine,
        IngestionOptions options,
        out TraceEntry? traceEntry
    )
    {
        int[] delimiterIndicesBuffer = new int[ValidDelimiterCount];
        Array.Fill(delimiterIndicesBuffer, NoIndex);

        traceEntry = null;
        Boolean isParsableTraceLine = ( false == String.IsNullOrWhiteSpace(traceLine) );

        if ( false != isParsableTraceLine )
        {
            isParsableTraceLine = ( false == IsCommentTraceLine(traceLine!, options) );
        }
        else
        {
            return false;
        }

        if ( false != isParsableTraceLine )
        {
            isParsableTraceLine = TryGetSegments(traceLine!, options, delimiterIndicesBuffer);

            return ( false != ( isParsableTraceLine && TryBuildEntry(traceLine!, options, delimiterIndicesBuffer, out traceEntry) ) );
        }
        else
        {
            return false;
        }

        // ------ Local functions ------

        static Boolean IsCommentTraceLine(String traceLine, IngestionOptions options)
        {
            String commentPrefix = options.CommentPrefix;
            int lenCommentPrefix = commentPrefix.Length;
            int lenTraceLine = traceLine.Length;

            int traceLineCursor = 0;

            while (    ( lenTraceLine > traceLineCursor )
                    && ( lenCommentPrefix > traceLineCursor )
                    && ( commentPrefix[traceLineCursor] == traceLine[traceLineCursor] ) )
            {
                ++traceLineCursor;
            }

            return ( lenCommentPrefix == traceLineCursor ); // True when traceLine is a comment.
        }

        static Boolean TryGetSegments(String traceLine, IngestionOptions options, int[] delimiterIndices)
        {
            String fieldDelimiter = options.FieldDelimiter;
            int lenTraceLine = traceLine.Length;
            int lenFieldDelimiter = fieldDelimiter.Length;

            int traceLineCursor = 0;
            int delimiterCount = 0;
            int delimiterCursor = 0;

            while (    ( ValidDelimiterCount > delimiterCount )
                    && ( lenTraceLine > traceLineCursor ) )
            {
                if ( fieldDelimiter[0] == traceLine[traceLineCursor] )
                {
                    while (    ( lenTraceLine > traceLineCursor )
                            && ( lenFieldDelimiter > delimiterCursor )
                            && ( fieldDelimiter[delimiterCursor] == traceLine[traceLineCursor] ) )
                    {
                        traceLineCursor++;
                        delimiterCursor++;
                    }

                    if ( lenFieldDelimiter == delimiterCursor )
                    { // Delimiter found.
                        delimiterIndices[delimiterCount] = traceLineCursor - lenFieldDelimiter;
                        delimiterCount += 1;
                    }

                    delimiterCursor = 0;
                }
                else
                {
                    traceLineCursor++;   
                }
            }

            return ( ValidDelimiterCount == delimiterCount );
        }

        static Boolean TryBuildEntry(String traceLine, IngestionOptions options, int[] delimiterIndices, out TraceEntry? traceEntry)
        {
            int lenFieldDelimiter = options.FieldDelimiter.Length;

            traceEntry = null; // Initialize out parameter to null.

            // TraceEntry item start indices:
            int timestampStart  = 0;
            int severityStart   = delimiterIndices[0] + lenFieldDelimiter;
            int componentStart  = delimiterIndices[1] + lenFieldDelimiter;
            int messageStart    = delimiterIndices[2] + lenFieldDelimiter;

            // TraceEntry item end indices:
            int timestampLength = delimiterIndices[0];
            int severityLength  = delimiterIndices[1] - severityStart;
            int componentLength = delimiterIndices[2] - componentStart;
            int messageLength   = traceLine.Length - messageStart;

            // TraceEntry items:
            ReadOnlySpan<char> timestampSpan = traceLine.AsSpan( timestampStart , timestampLength ).Trim();
            ReadOnlySpan<char> severitySpan  = traceLine.AsSpan( severityStart  , severityLength  ).Trim();
            ReadOnlySpan<char> componentSpan = traceLine.AsSpan( componentStart , componentLength ).Trim();
            ReadOnlySpan<char> messageSpan   = traceLine.AsSpan( messageStart   , messageLength   ).Trim();
            
            Boolean isValidTimestamp = DateTimeOffset.TryParseExact(
                timestampSpan,
                "yyyy-MM-dd'T'HH:mm:ss.fffzzz",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTimeOffset validatedTimestamp); // Validate timestamp.
            Boolean isValidSeverity = Severity.TryParse(severitySpan, true, out Severity validatedSeverity); // Validate severity.
            Boolean isValidTraceLine = ( false != ( isValidTimestamp && isValidSeverity ) ); // True if TraceEntry instance can be built.

            if ( false != isValidTraceLine )
            {
                traceEntry = new TraceEntry(
                    validatedTimestamp,
                    validatedSeverity,
                    componentSpan.ToString(),
                    messageSpan.ToString()
                );
            }

            return isValidTraceLine;
        }
    }
}
