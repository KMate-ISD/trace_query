using System;
using System.IO;

namespace TraceQuery.Core.Ingestion;

/// <summary>
/// Reads source file line-by-line.
/// Disposable: resource owning object with deterministic cleanup.
/// </summary>
public sealed class FileReader : IDisposable
{
    private readonly StreamReader _streamReader;

    /// <summary>Constructor for FileReader class.</summary>
    /// <param name="path">String?</param>
    public FileReader(String path)
    {
        _streamReader = new StreamReader(path);
    }

    /// <summary>Routes line at cursor position.</summary>
    /// <returns>String?</returns>
    public String? RouteLine()
    {
        return _streamReader.ReadLine();
    }

    /// <summary>Dispose owned resource.</summary>
    public void Dispose()
    {
        _streamReader.Dispose();
    }
}
