# Contributing to SimplySharp

Thank you for your interest in contributing to SimplySharp!

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (see `global.json` for the exact version)

## Building

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

### Code Coverage

Code coverage is enforced per test project via [coverlet](https://github.com/coverlet-coverage/coverlet). Each test project defines a `<Threshold>` in its `.csproj` that sets the minimum line coverage percentage. The build will fail if coverage drops below this threshold.

When adding new features, write tests that maintain or increase coverage. If coverage improves, bump the `<Threshold>` in the corresponding test project's `.csproj` to lock in the new baseline. The threshold must only go up.

## Code Style

- **Static analysis** is enabled with `AnalysisMode=All` — resolve all warnings before submitting.
- **Code style** is enforced at build time (`EnforceCodeStyleInBuild=true`).
- **Nullable reference types** are enabled — avoid nullable warnings.

## Design Guidelines

Read [DESIGN.md](DESIGN.md) before making changes. It describes the architectural principles that govern how code should be written in this project, including:

- Keeping code bodies structured, not stringly-typed
- Sharing common member surfaces through the type hierarchy
- Modeling all common C# constructs
- Supporting generic type constraints
- Writing effective, meaningful tests
- Documenting all members

## Planned Features

See [TODO.md](TODO.md) for a list of planned features and goals. If you'd like to tackle one, open an issue first to discuss your approach.

## Pull Requests

CI runs automatically on pull requests to `main`. The pipeline runs restore, build, and test — all three must pass before merging.
