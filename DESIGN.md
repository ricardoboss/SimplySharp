# Design Guidelines

Guidelines for SimplySharp.

---

## DG1: Keep code bodies structured, not stringly-typed

Method bodies, property accessors, and initializers should be represented as a statement/expression AST (e.g.,
`IfStatement`, `ReturnStatement`, `MethodCallExpression`, `BinaryExpression`) rather than plain strings. A `VerbatimCode`
escape hatch for raw text is acceptable, but should not be the primary mechanism. Blocks should hold a list of `Code`
children with nesting/indentation support rather than eagerly flattening to a single string.

---

## DG2: Share common member surface through the type hierarchy

Properties like `DocBlock`, `Attributes`, and `AccessModifier` should live in a shared base or interface rather than being
duplicated across unrelated types. `Constructor`, `PrimaryConstructor`, and `EnumCase` should all participate in the
`Member` hierarchy or share a common interface (`IDocumentable`, `IAttributable`). Specifically:

- `Constructor` should extend `Member`.
- `PrimaryConstructor` should be a subtype of `Constructor`.
- `EnumCase` should extend `Member` or implement shared interfaces.

---

## DG3: Model all common C# constructs

The DOM should cover the full range of everyday C# type and member declarations:

- **Types:** classes, interfaces, enums, records (`record class`, `record struct`), structs, delegates
- **Members:** fields, properties, methods, constructors, events, indexers, operator overloads

---

## DG4: Support generic type constraints

Any type or method that can be generic (`ClassType`, `InterfaceType`, `RecordType`, `StructType`, `Method`) should support
`where T : ...` constraints. A `GenericParameter` type (distinct from `TypeRef`) with a `Constraints` collection serves
this purpose.

---

## DG5: Write effective tests alongside the implementation

Tests should exercise real logic — collection parent-tracking, computed properties, conversion methods, visitor traversal
paths — not auto-property defaults or compiler-enforced type hierarchies. A test that merely verifies a `bool` property
defaults to `false` or that setting a property stores a value tests the C# language, not the library. Every test should be
justified by a code path that could plausibly break: branching logic, side effects, recursive computation, or cross-object
invariants. Testing should happen alongside implementation, not deferred to a later phase.

Line coverage is enforced via coverlet for every test project. `Directory.Test.props` configures coverage collection
globally with a default threshold of 0%. Each test project sets its own `<Threshold>` in its `.csproj` to the current
coverage floor. The threshold should only go up: when adding new features, write tests that maintain or increase coverage.
If coverage improves, bump `<Threshold>` in the test project's `.csproj` to lock in the new baseline.

---

## DG6: Document members

All new or modified public API members should include XML documentation blocks (summary, params, returns, remarks as
appropriate). Unchanged code does not need to be retroactively documented.

---

## DG7: Emit compilable, idiomatic C#

The CodeGen emitter should produce output that compiles without modification and follows standard C# formatting
conventions. If a valid DOM tree can be constructed, the emitter should be able to produce valid C# from it. The emitter
also serves as the primary integration test for the entire DOM.

---

## DG8: No silent gaps in node handling

When a new node type is added to CodeDOM, both CodeGen and CodeParse should be updated to handle it. An unhandled node
type should produce an explicit error rather than being silently skipped. This prevents incomplete output or partial
parsing from going unnoticed.

---

## DG9: Handle unsupported syntax gracefully in CodeParse

The DOM does not model every C# feature (e.g., local functions, LINQ query syntax, preprocessor directives). When the
parser encounters a construct it cannot represent, it should follow a consistent, documented strategy — whether that means
skipping with a diagnostic, wrapping in a `VerbatimCode` node, or throwing. The chosen approach should be predictable and
easy for consumers to handle.

---

## DG10: Preserve round-trip fidelity

Parsing a C# file and re-emitting it should produce semantically equivalent code. Exact whitespace and formatting
preservation is not required, but the structural and semantic meaning of the source should be maintained through a
parse-then-emit cycle.

---

## DG11: Be language-version-aware across all layers

CodeDOM and CodeParse should always target the latest C# language features. CodeGen, however, should be aware of the
target language version and emit appropriate constructs accordingly — for example, falling back to extension methods when
targeting a version that predates extension blocks. DOM nodes and enum values representing version-gated features should be
annotated with `[RequiresLanguageVersion]` (as already done for `AccessModifier.File`), and CodeGen should consult these
annotations when deciding what to emit.
