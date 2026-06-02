using System;

namespace TraceQuery.Core.Model;

/// <summary>
/// Single trace/log entry.
/// Timestamp (timezone/UTC-aware), severity, source/component identifier, message.
/// Rationale of `class` as type choice:
/// size > 16 Bytes (struct guideline), contains reference, has identity.
/// </summary>
public class TraceEntry
{
    /// <summary>Constructor for TraceEntry class.</summary>
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
    /// Justification of Byte type: suspected memory overhead of storing many strings.
    /// Possible computational overhead of comparing TraceEntry components on analysis.
    /// TODO: component->id (ingestion), id->component (reporting)
    /// note: Complexity introduced without profiling. Monitor debt carefully.
    /// </summary>
    public Byte Component { get; }

    /// <summary>Event description.</summary>
    public String? Message { get; }
}