using System;

namespace TraceQuery.Core;

/// <summary>
/// Single trace/log entry.
/// Timestamp (timezone/UTC-aware), severity, source/component identifier, message.
/// Rationale for choosing `class` type: a log entry is an object that might be used / read / get passed by different features,
/// so a ref type makes sure that there is only one memory area containing the same data.
/// </summary>
public class Trace
{
    /// <summary>Constructor for Trace class.</summary>
    /// <param name="severity">Severity enum</param>
    /// <param name="component">Int16</param>
    /// <param name="message">String</param>
    public Trace(Severity severity, Int16 component, String? message)
    {
        Timestamp = DateTime.Now;
        Severity = severity;
        Component = component;
        Message = message;
    }

    /// <summary>Time of item logged.</summary>
    public DateTime Timestamp { get; }

    /// <summary>Severity classification of the item.</summary>
    public Severity Severity { get; private set; }

    /// <summary>Source / Component ID.</summary>
    public Int16 Component { get; private set; }

    /// <summary>Event description.</summary>
    public String? Message { get; private set; }
}