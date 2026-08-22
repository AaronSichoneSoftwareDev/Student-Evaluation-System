# Student-Evaluation-System

Student evaluation & report-card platform built on .NET 9 / Blazor Server with Clean Architecture — CQRS via MediatR, EF Core + SQLite, ASP.NET Identity, and QuestPDF-generated report cards.

**Evaluate** lets teachers score students per topic across their assigned subjects, then rolls finalized results up into parent-facing PDF report cards — gated so a report card only becomes downloadable once a student has been evaluated in every subject registered to their class.

## Architecture

Built as a Clean Architecture solution with a strict dependency direction:

```
Evaluate.Domain          — entities, value objects, domain events, no external dependencies
Evaluate.Application     — CQRS commands/queries, validation, interfaces (the "ports")
Evaluate.Infrastructure  — EF Core, ASP.NET Identity, QuestPDF (the "adapters")
Evaluate.Web             — Blazor Server UI, composition root
```

## Design patterns & practices

- **CQRS** via MediatR — every write is a `Command`, every read a `Query`, each with its own handler
- **Repository pattern** — one repository per aggregate (`IStudentRepository`, `IEvaluationRepository`, `ICourseRepository`, etc.), each scoped to its module in Application and implemented against EF Core in Infrastructure. Command/query handlers depend on these narrow interfaces, never on the EF Core `DbContext` directly — `IApplicationDbContext` is now an Infrastructure-only concern that the repositories (and the data seeder) are built on
- **Unit of Work** — `IUnitOfWork.SaveChangesAsync` commits whatever a request's repositories staged, kept as a separate interface from the repositories themselves so "collect changes" and "commit changes" stay distinct responsibilities
- **Pipeline behaviours** wrapping every request: unhandled-exception logging, permission-based authorization (`[RequirePermission]`), FluentValidation, and performance logging
- **Result pattern** (`Result` / `Result<T>`) for expected business-rule failures, kept separate from `ValidationException` (input validation) and thrown exceptions (unexpected/exceptional cases)
- **Factory pattern** — domain entities expose private constructors and static `Create(...)` factories that enforce invariants at construction time
- **Strategy pattern** — `IGradingStrategy` abstracts how topic scores become a final percentage and letter grade
- **Specification pattern** — composable query filters (e.g. `EvaluationsFilterSpecification`), applied inside `IEvaluationRepository`
- **Domain events** — raised by entities, dispatched through an EF Core `SaveChanges` interceptor; a second interceptor handles audit logging (created/modified by, timestamps) automatically
- **ASP.NET Core Identity** with permissions modeled as role claims, enforced declaratively via `[RequirePermission]`

## Tech stack

- .NET 9, Blazor Server (Interactive Server render mode)
- EF Core 9 + SQLite
- MediatR (CQRS) + FluentValidation
- ASP.NET Core Identity
- QuestPDF for server-generated report card PDFs
- xUnit, including a MediatR-pipeline test suite that exercises commands/queries through the real DI-wired pipeline (not just handlers in isolation) — this is what caught a bug where the authorization behaviour silently blocked every action in the absence of a login UI

## Modules

Academic Years / Terms / Classes, Courses / Topics, Students / Enrollments, Teachers & Subject Assignments, Evaluations (per-topic scoring), and PDF Report Cards — plus an app-wide toast/confirm-dialog system for consistent UI feedback on every action.

## Running the project

Prerequisites: .NET 9 SDK.

```bash
dotnet run --project src/Evaluate.Web
```

Then open the URL printed in the console (see `src/Evaluate.Web/Properties/launchSettings.json`, default `http://localhost:5119`). On first run the app creates a SQLite database at `src/Evaluate.Web/App_Data/evaluate.db`, applies EF Core migrations, and seeds sample academic years, classes, courses, teachers, and students automatically. There is no login screen yet, so every page is reachable directly.

Run the test suite:

```bash
dotnet test tests/Evaluate.Application.Tests
```
