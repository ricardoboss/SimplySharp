using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Abstract base class for all type declarations (classes, interfaces, enums, structs, records, delegates).
/// </summary>
public abstract class CodeType : CodeNode
{
	/// <summary>
	/// Gets or sets the namespace that contains this type.
	/// </summary>
	public CodeNamespace? ContainingNamespace { get; internal set; }

	/// <summary>
	/// Gets or sets the access modifier for this type.
	/// </summary>
	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	/// <summary>
	/// Gets or sets the name of this type.
	/// </summary>
	public required string Name { get; set; }

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
}
