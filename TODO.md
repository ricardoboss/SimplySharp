# TODO

Planned features and goals for SimplySharp.

## T1: Ship a reference C# code emitter

Provide a `CSharpCodeWriter` implementation of `CodeDomVisitor` that generates compilable C# source files from the DOM.
Without this, every consumer must write their own emitter from scratch. A working emitter also serves as the primary
integration test for the entire DOM.

---

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
