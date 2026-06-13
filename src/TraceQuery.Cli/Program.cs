using System;
using System.Collections.Generic;
using System.Diagnostics;
using TraceQuery.Core.Configuration;
using TraceQuery.Core.Ingestion;
using TraceQuery.Core.Model;
using TraceQuery.Core.TraceLineParser;

internal class Program
{
    private static void Main(string[] args)
    {
        if ( 0 < args.Length )
        {
            String path = args[0];
            String? lineBuffer;

            // Temporary cli-core wiring.
            // TODO: Rework on deliberate implementation.
            {
                try // TraceFileSource usage
                {
                    using TraceFileSource traceFileSource = new TraceFileSource(path);

                    Byte lineCount = 0;
                    while ( null != ( lineBuffer = traceFileSource.GetNextLine() ) )
                    {
                        ++lineCount;
                    }
                    Console.WriteLine(string.Format("[{0}] {1} line(s) read.", path, lineCount));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                try // IngestionOptions usage
                {
                    IngestionOptions config1 = new();
                    IngestionOptions config2 = new()
                    {
                        SeverityThreshold = Severity.Warning,
                        CommentPrefix = "@",
                        FieldDelimiter = "\t",
                    };
                    IngestionOptions config3 = new()
                    {
                        CommentPrefix = String.Empty,
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                try // TraceLineParser usage
                {
                    using TraceFileSource traceFileSource = new TraceFileSource(path);
                    IngestionOptions config1 = new();
                    List<TraceEntry> traces = new List<TraceEntry>();

                    int traceLineCnt = 0;
                    int validTraceLineCnt = 0;
                    while ( null != ( lineBuffer = traceFileSource.GetNextLine() ) )
                    {
                        Boolean isParseSuccess = lineBuffer.TryParseTraceLine(config1, out TraceEntry? te);

                        if ( false != isParseSuccess )
                        {
                            traces.Add(te!);
                            ++validTraceLineCnt;
                        }
                        ++traceLineCnt;
                    }

                    Console.WriteLine(string.Format("[{0}] {1} of {2} trace line(s) parsed.", path, validTraceLineCnt, traceLineCnt));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}