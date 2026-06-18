using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using TraceQuery.Core.Configuration;
using TraceQuery.Core.Ingestion;
using TraceQuery.Core.Model;

namespace TraceQuery.Profiling;

[MemoryDiagnoser]
public class IngestionBenchmark
{
    private const int IterationCount = 10_000;
    private const int Seed = 12345;

    private static readonly IngestionOptions Options = new();

    private List<String> SampleTraceLines = null!;

    [Benchmark]
    public void UsingTLP()
    {
        List<String> lines = SampleTraceLines;

        for (int i = 0; i < lines.Count; i++)
        {
            TraceLineParser.TryParseTraceLine(lines[i], Options, out _);
        }
    }

    [Benchmark]
    public void UsingTLPSimple()
    {
        List<String> lines = SampleTraceLines;

        for (int i = 0; i < lines.Count; i++)
        {
            TraceLineParser_Simple.TryParseTraceLine(lines[i], Options, out _);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        SampleTraceLines = BuildTraceLines();
    }

    private static List<String> BuildTraceLines()
    {
        List<String> list = new List<String>(IterationCount);

        for (int i = 0; i < IterationCount; i++)
        {
            list.Add(TraceLineBuilder(Seed + i));
        }

        return list;
    }

    private static string TraceLineBuilder(int seed)
    {
        Random random = new Random(seed);

        // Generate timestamp:
        long min = DateTimeOffset.UnixEpoch.ToUnixTimeMilliseconds();
        long max = DateTimeOffset.UtcNow.AddYears(10).ToUnixTimeMilliseconds();
        long range = max - min;
        long rdtUnix = min + (Math.Abs(random.NextInt64()) % range);
        DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeMilliseconds(rdtUnix);

        // Generate severity:
        int severityCount = Enum.GetValues<Severity>().Length;
        Severity severity = (Severity)(random.Next() % severityCount);

        // Generate component:
        int componentLength = random.Next(4, 16);
        String component = random.GetHexString(componentLength);

        // Generate message:
        int messageLength = random.Next(32, 64);
        String message = random.GetHexString(messageLength);

        StringBuilder sb = new StringBuilder();

        if ( 0 == random.Next(43) )
        {
            sb.Append(Options.CommentPrefix);
        }
        else
        { 
            const String TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";
            sb.Append(timestamp.ToString(TimestampFormat));

            String delimiter = Options.FieldDelimiter;
            sb.Append(delimiter);
            sb.Append(severity);

            sb.Append(delimiter);
            sb.Append(component);
            
            sb.Append(delimiter);
        }

        sb.Append(message);

        return sb.ToString();
    }
}