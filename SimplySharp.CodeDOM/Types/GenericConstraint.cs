namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Abstract base for generic type parameter constraints (<c>where T : ...</c>).
/// </summary>
public abstract record GenericConstraint;

/// <summary>
/// Represents a <c>class</c> or <c>class?</c> reference-type constraint.
/// </summary>
/// <param name="IsNullable">When <see langword="true"/>, represents <c>class?</c>; otherwise <c>class</c>.</param>
public sealed record ClassConstraint(bool IsNullable = false) : GenericConstraint;

/// <summary>
/// Represents a <c>struct</c> value-type constraint (implies <c>notnull</c>).
/// </summary>
public sealed record StructConstraint : GenericConstraint;

/// <summary>
/// Represents an <c>unmanaged</c> constraint.
/// </summary>
public sealed record UnmanagedConstraint : GenericConstraint;

/// <summary>
/// Represents a <c>notnull</c> constraint.
/// </summary>
public sealed record NotNullConstraint : GenericConstraint;

/// <summary>
/// Represents a <c>new()</c> parameterless-constructor constraint.
/// </summary>
public sealed record NewConstraint : GenericConstraint;

/// <summary>
/// Represents a <c>default</c> constraint, used in overrides to remove inherited constraints.
/// </summary>
public sealed record DefaultConstraint : GenericConstraint;

/// <summary>
/// Represents a base type or interface constraint (e.g., <c>where T : IDisposable</c>).
/// </summary>
/// <param name="Type">The type that the generic parameter must derive from or implement.</param>
public sealed record TypeConstraint(TypeRef Type) : GenericConstraint;
