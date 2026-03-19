namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Intermediate base class for type declarations that support modifiers like <c>abstract</c>,
/// <c>sealed</c>, <c>partial</c>, and generic type parameters. All type declarations except
/// <see cref="EnumType"/> extend this class.
/// </summary>
public abstract class GenericCodeType : CodeType
{
	/// <summary>
	/// Gets or sets a value indicating whether this type is declared as abstract.
	/// </summary>
	public bool IsAbstract { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this type is declared as sealed.
	/// </summary>
	public bool IsSealed { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this type is declared as partial.
	/// </summary>
	public bool IsPartial { get; set; }

	/// <summary>
	/// Gets the list of generic type parameters declared on this type
	/// (e.g., <c>T</c> and <c>TValue</c> in <c>class Dictionary&lt;T, TValue&gt;</c>).
	/// </summary>
	public IList<GenericParameter> GenericParameters { get; init; } = [];
}
