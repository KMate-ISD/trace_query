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
    /// <summary>Sets the attributes of a TraceEntry object.</summary>
    /// <param name="timestamp">Date and time of event with UTC offset.</param>
    /// <param name="severity">Severity classification of the event.</param>
    /// <param name="component">ID of component associated with the event.</param>
    /// <param name="message">Event description.</param>
    public TraceEntry(DateTimeOffset timestamp, Severity severity, String component, String? message)
    {
        Timestamp = timestamp;
        Severity = severity;
        Component = component;
        Message = message;
    }

    /// <summary>Date and time of event with UTC offset.</summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>Severity classification of the event.</summary>
    public Severity Severity { get; }

    /// <summary>ID of component associated with the event.
    /// TODO: Switch to int/enum if profiling shows substantial memory overhead due to storing many strings.
    /// TODO: Switch to int if profiling shows substantial computational overhead when comparing TraceEntry components.
    /// </summary>
    public String Component { get; }

    /// <summary>Event description.</summary>
    public String? Message { get; }
}