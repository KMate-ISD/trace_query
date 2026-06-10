using System;
using System.IO;

namespace TraceQuery.Core.Ingestion;

/// <summary>
/// Reads source file line-by-line.
/// Disposable: resource owning object with deterministic cleanup.
/// </summary>
public sealed class TraceFileSource : IDisposable
{
    private readonly StreamReader _streamReader;

    /// <summary>Initializes the owned StreamReader object with the input path.</summary>
    /// <param name="path">Path to trace file to source.</param>
    public TraceFileSource(String path)
    {
        _streamReader = new StreamReader(path);
    }

    /// <summary>Fetches line at current cursor position.</summary>
    /// <returns>Next line or null at end of file.</returns>
    public String? GetNextLine()
    {
        return _streamReader.ReadLine();
    }

    /// <summary>Dispose owned resource.</summary>
    public void Dispose()
    {
        _streamReader.Dispose();
    }
}
