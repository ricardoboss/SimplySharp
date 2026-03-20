# CLAUDE.md

Project guide for AI assistants working on SimplySharp.

## What is SimplySharp?

A .NET library providing a Code DOM (Document Object Model) for C# source code. It represents C# constructs as an AST (classes, interfaces, enums, records, structs, delegates, and their members). The CodeGen module provides a reference C# emitter (`CSharpCodeWriter`). The CodeParse module (parsing) is planned.

## Build & Test

```bash
dotnet build          # build all projects
dotnet test           # run all tests (includes coverage enforcement)
dotnet restore        # restore NuGet packages
```

Requires .NET 10.0 SDK (exact version and roll-forward policy in `global.json`).

**NuGet packages** are managed centrally via `Directory.Packages.props` — add or update package versions there, not in individual `.csproj` files. Shared build settings live in `Directory.Build.props`; shared test settings (coverage, test framework references) live in `Directory.Test.props`.

## Project Structure

```
SimplySharp.CodeDOM/          # Core DOM library (types, nodes, collections, visitor)
SimplySharp.CodeDOM.Test/     # NUnit tests for CodeDOM
SimplySharp.CodeGen/          # Code generation (CSharpCodeWriter)
SimplySharp.CodeGen.Test/     # Tests for CodeGen (73 tests, 80% threshold)
SimplySharp.CodeParse/        # Parsing (planned, currently empty)
SimplySharp.CodeParse.Test/   # Tests for CodeParse
```

Key directories inside CodeDOM:
- `Types/` — type declarations (ClassType, InterfaceType, StructType, RecordType, EnumType, DelegateType), TypeRef records, GenericParameter, GenericConstraint, Variance
- `Nodes/` — member declarations (FieldNode, PropertyNode, MethodNode, ConstructorNode, EventNode, IndexerNode, OperatorNode, EnumMemberNode, Parameter) with MemberNode as the abstract base and OperatorKind enum
- `Collections/` — parent-tracking collections (CodeNodeCollection, CodeTypeCollection, CodeNamespaceCollection)
- `CodeDomVisitor.cs` — abstract async visitor base class for tree traversal
- `CodeWorkspace.cs` — root container for namespaces

Key solution-level files:
- `Directory.Build.props` — shared MSBuild properties (target framework, analysis mode, code style)
- `Directory.Test.props` — shared test configuration (coverage, test framework references)
- `Directory.Packages.props` — centralized NuGet package versions

## Architecture & Patterns

- **Visitor pattern**: all nodes implement `AcceptAsync(CodeDomVisitor, CancellationToken)`. Visitor methods are async.
- **Parent tracking**: collections automatically set/clear parent references on insert/remove.
- **Type hierarchy**: `CodeNode` → `CodeType` → `GenericCodeType` → concrete types; `MemberNode` → concrete member nodes.
- **TypeRef records**: `NamedTypeRef`, `GenericTypeRef`, `ArrayTypeRef`, `NullableTypeRef`, `TupleTypeRef` with static aliases for primitives (`TypeRef.Int`, `TypeRef.String`, etc.).
- **`required` and `init` properties** used for essential fields.
- **Code emitter**: `CSharpCodeWriter` in CodeGen — a `CodeDomVisitor` that emits C# source via a `SourceWriter`. Handles all node types, generic parameters/constraints, parameter modifiers, operator tokens, and base type lists. Formatting (indentation, line endings, final newline) and the target C# language version are configured through `CodeWriteSettings` and applied by `SourceWriter`. Language-version-aware: version-gated features (file-scoped namespaces, `record struct`, `file` modifier, `required`, `init`, primary constructors on classes/structs) are validated or adapted based on `CodeWriteSettings.LanguageVersion`. Well-known version constants live in `CSharpLanguageVersion`.

## Testing

- **Framework**: NUnit 4.5 with Moq for mocking.
- **Coverage**: enforced via coverlet. Active test projects set a `<Threshold>` in their `.csproj` for minimum line coverage (e.g., CodeDOM.Test has threshold 76). Build fails if coverage drops below it. Threshold must only increase.
- **Test philosophy** (DG5): test real logic (collection parent-tracking, computed properties, conversion methods, visitor traversal), not auto-property defaults or compiler-enforced type hierarchies.

## Code Style

- Tabs for indentation in `.cs` files (4-space width); spaces for XML/JSON/YAML (2-space).
- `AnalysisMode=All` — resolve all static analysis warnings.
- `EnforceCodeStyleInBuild=true` — code style enforced at build time.
- `Nullable=enable` — no nullable warnings.
- All public API members need XML documentation (summary, params, returns, remarks).
- See `.editorconfig` for full formatting rules.

## Design Guidelines (from DESIGN.md)

- **DG1**: Code bodies as AST nodes, not raw strings. `VerbatimCode` is an escape hatch only.
- **DG2**: Share common surface (DocBlock, Attributes, AccessModifier) via type hierarchy.
- **DG3**: Model all common C# constructs (types + members listed above).
- **DG4**: Support `where T : ...` constraints on types and methods via `GenericParameter`.
- **DG5**: Write effective tests alongside implementation; test real logic, not compiler features.
- **DG6**: Document all public members with XML docs.
- **DG7**: CodeGen must emit compilable, idiomatic C# that requires no modification.
- **DG8**: No silent gaps — new node types must be handled in CodeGen and CodeParse.
- **DG9**: Handle unsupported syntax gracefully with a consistent strategy (skip, wrap, or throw).
- **DG10**: Preserve round-trip fidelity through parse-then-emit cycles.
- **DG11**: Be language-version-aware; use `[RequiresLanguageVersion]` attribute for gated features.

## Planned Work (from TODO.md)

- **T2**: Roslyn round-trip parsing
- **T3**: Source generator integration helpers
- **T4**: Structural diffing of DOM trees
- **T5**: Validation pipeline (pre-emission error checking)
- **T6**: Fluent builder API
- **T9**: Multi-file emission with a virtual file system (`ICodeFileSystem`, file-per-type / file-per-namespace strategies, `Charset` setting)

## Documentation Maintenance

When making changes to the project, keep documentation in sync:
- Completing a task from `TODO.md` → mark it done in `TODO.md`, update relevant sections in `DESIGN.md` and this file (`CLAUDE.md`).
- Adding new node types, patterns, or public API → update `DESIGN.md` (if a guideline is affected), the Architecture & Patterns section here, and the Project Structure section if new directories/files are introduced.
- Changing build, test, or CI configuration → update the Build & Test and CI sections here and `CONTRIBUTING.md`.

In general, any change to code or project structure should be accompanied by updates to all documentation files that reference the affected areas.

## CI

GitHub Actions on push/PR to `main`: restore → build → test. All must pass.
