PROJECT DOCUMENT
================

## Project name
**TraceQuery**

## Purpose
A CLI tool plus reusable library that ingests structured log/trace files, lets the user query and aggregate them, and emits reports.

## Stack
> C# (.NET LTS: net10.0)

> .NET CLI

> VS Code

> Standard library only (no third-party deps), deliberately libless

## Architecture
> Two projects in one solution:
- TraceQuery.Core (library: domain model, ingestion, query/aggregation engine)
- TraceQuery.Cli (console front-end consuming Core)

> One-way dependency: Cli -> Core.

> File/Module map:

> ```bash
.
├───samples
└───src
    ├───TraceQuery.Cli
    └───TraceQuery.Core
        ├───Ingestion
        ├───Model
        ├───Querying
        └───Reporting
```

## Constraints
> No NuGet runtime deps

> Clean compile under nullable-enabled

> Public API of Core documented with XML doc comments

> No business logic in Cli.

## Increments criteria
> Solution builds with zero warnings (warnings-as-errors target).

> Core has no dependency on Cli; Core is usable as a standalone library.

> Malformed input never crashes the process - it is reported and handled.

> Every increment leaves the solution in a runnable state.

## Concepts used
type system, memory/GC, members, params, exceptions, interfaces, generics, delegates/events, collections/iterators, LINQ, records & pattern matching, nullable reference types