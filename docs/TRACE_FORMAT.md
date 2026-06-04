# TraceQuery input format — v1

Trace/log files exported from the existing platform. One record per line.

## Record grammar

    <timestamp> | <severity> | <source> | <message>

Fields are separated by `" | "` (space, pipe, space).

- **timestamp** — ISO 8601 with offset, e.g. `2026-05-30T02:00:09.020+00:00`. Offsets vary by host; not always UTC.
- **severity** — one of `TRACE`, `INFO`, `WARNING`, `ERROR`, `CRITICAL`. Case-insensitive on the wire.
- **source** — component/service name. Free text, no embedded `" | "`.
- **message** — free text. May itself contain `|`; treat everything after the third `" | "` as the message (i.e. split into at most 4 fields).

## Non-records

- Blank / whitespace-only lines carry no record.
- Lines beginning with `#` are export headers/comments, not records.

## Dirty data

This is a production export over existing infrastructure — expect malformed lines (bad timestamps, unknown severities, missing fields, junk). Per the project's global acceptance criteria, a malformed line must be **reported and skipped, never fatal**. The exact handling contract (skip vs. default vs. error tally) is defined when ingestion is specified — not here.

## Sample files

- `clean_small.log` — small, all well-formed; every severity represented; mixed offsets (note the `+02:00` line).
- `mixed_realistic.log` — ~30 well-formed lines, a realistic run (a payment-gateway degradation and recovery, plus an account lockout). Good for line-count now and for grouping / top-N / time-bucketing later.
- `messy.log` — well-formed records interleaved with edge and malformed lines (see below).

## What `messy.log` exercises (line intent — not a handling spec)

- A `#` header line and a blank line (non-records).
- A record whose **message contains embedded `|`** (must survive the at-most-4 split).
- An **unknown severity** (`debug`) — not in the v1 set.
- A **lowercase severity** on an otherwise valid record (case-insensitivity).
- **Leading/trailing whitespace** around a valid record (trimming).
- An **empty message** (trailing delimiter, nothing after it).
- A record **missing fields** (timestamp + severity only).
- A **date-shaped but invalid** timestamp (`2026-13-40T99:99:99`).
- A **non-timestamp** first field.
- A **structureless junk** line.

## Note on distinct sources

Across all three samples there are 8 distinct sources (ApiGateway, AuthService, OrderService, PaymentService, Database, Cache, Scheduler, Worker) — comfortably inside your `Byte` component-id budget. The 256 cap isn't stressed by this dataset; just keep it in mind as the constraint it is.
