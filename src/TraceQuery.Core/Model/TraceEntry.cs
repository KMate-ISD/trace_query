using System;

namespace TraceQuery.Core.Model;

/// <summary>
/// Single trace/log entry.
/// Timestamp (timezone/UTC-aware), severity, source/component identifier, message.
/// </summary>
public class TraceEntry
{
    /// <summary>Constructor for Trace class.</summary>
    /// <param name="timestamp">DateTimeOffset</param>
    /// <param name="severity">Severity enum</param>
    /// <param name="component">Byte</param>
    /// <param name="message">String</param>
    public TraceEntry(DateTimeOffset timestamp, Severity severity, Byte component, String? message)
    {
        Timestamp = timestamp;
        Severity = severity;
        Component = component;
        Message = message;
    }

    /// <summary>Time of item logged.</summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>Severity classification of the item.</summary>
    public Severity Severity { get; }

    /// <summary>Source / Component ID.
    /// Justification of Byte type: memory overhead of storing strings.
    /// Computational overhead of comparing TraceEntry components on analysis.
    /// A component to ID lookup table (and reverse counterpart) is proposed.
    /// Conversion would happen at parsing.
    /// Component ID could be refactored to an enum later, if needed.
    /// </summary>
    public Byte Component { get; }

    /// <summary>Event description.</summary>
    public String? Message { get; }
}