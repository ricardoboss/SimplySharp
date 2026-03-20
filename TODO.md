# TODO

Planned features and goals for SimplySharp.

## T2: Support Roslyn round-trip

Enable parsing existing C# source files into the CodeDOM via Roslyn for read-modify-write workflows. This allows
consumers to load a `.cs` file, modify the DOM, and write it back — not just generate from scratch.

---

## T3: Provide source generator integration

Provide helpers or base classes for using SimplySharp.CodeDOM within Roslyn incremental source generators, so generator
authors can build up a `Workspace` and emit it as generated source.

---

## T4: Enable structural diffing

Given two versions of a DOM tree, compute a structural diff. Useful for incremental regeneration (only re-emit changed
types) or conflict detection when multiple generators touch the same output. Consider making the DOM immutable by
default to simplify diffing, enable thread safety, and support caching.

---

## T5: Add a validation pipeline

Provide a pluggable validation system that checks the DOM for errors before emission:

- Missing return types on non-void methods
- Duplicate member names within a type
- Invalid modifier combinations (`static abstract` only valid in interfaces)
- Required properties without initializers on non-abstract types
- Enum cases with duplicate values

---

## T6: Offer a fluent builder API

Provide an optional fluent builder layer on top of raw constructors for concise DOM construction:

```csharp
var cls = ClassBuilder.Create("Cake")
    .WithProperty("Height", TypeRef.Double)
    .WithProperty("Sweetness", TypeRef.Double)
    .WithMethod("Bake", m => m.ReturnsVoid().WithBody(...))
    .Build();
```

---

## T7: Implement basic statements & expressions

This relates to DG1. Expressions and statements should be fully typed instead of simple strings:

```csharp
var ex = new ConstantIntExpression(125);
var st = new ReturnStatement(ex);

var foo = new IfStatement(
	condition: new ConstantBooleanExpression(false),
	body: st,
);
```

---

## T8: Support reading code style from an .editorconfig file for CodeGen

The default C# code emitter should be able to accomodate styling options from an `.editorconfig`.

This includes at least:
- indent style
- indent size
- line endings
- charset
- fine new line
- trim trailing whitespace
- C# brace placement
- C# attribute placement

Basically everything should be emitted so that `dotnet format` doesn't change anything (because it also supports custom
`.editorconfig` rules).

---

## T9: Multi-file emission with a virtual file system

The current `CSharpCodeWriter` produces a single string for an entire `CodeWorkspace`. Real-world usage typically
requires one output file per type (or per namespace). To support this without forcing callers to decompose and
reconstruct workspaces manually, introduce:

1. **`ICodeFileSystem`** — an abstraction for writing output files. Implementations could target the real file system,
   an in-memory dictionary (for testing / source generators), or a `SourceProductionContext`. The interface should
   accept a file path, content, and an `Encoding` (configured via `CodeWriteSettings.Charset`, defaulting to UTF-8).

2. **`ICodeFileStrategy`** (or similar) — a strategy that decides *how many files* to produce and *what path* each gets.
   At minimum, provide two built-in strategies:
   - **One file per type** — each `CodeType` is emitted to `{TypeName}.cs` (nested inside its namespace path).
   - **One file per namespace** — all types in a `CodeNamespace` are emitted to a single file.

   The strategy receives each namespace/type pair and returns the target file path. `CSharpCodeWriter` (or a new
   orchestrator that wraps it) iterates the workspace, calls the strategy to get a path, creates a fresh
   `SourceWriter` per file, emits the relevant subtree, and writes the result to the `ICodeFileSystem`.

3. **`CodeWriteSettings.Charset`** — an `Encoding` property (default `Encoding.UTF8`) passed through to the file
   system when writing. This mirrors `.editorconfig`'s `charset` setting.

The existing single-string `CSharpCodeWriter.ToString()` workflow should continue to work unchanged for callers that
don't need multi-file output.
