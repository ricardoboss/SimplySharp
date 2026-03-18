namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Abstract base class for all member declarations within a type (fields, properties, methods, etc.).
/// </summary>
public abstract class MemberNode : CodeNode
{
	/// <summary>
	/// Gets or sets the access modifier for this member.
	/// </summary>
	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	/// <summary>
	/// Gets or sets a value indicating whether this member is declared as static.
	/// </summary>
	public bool IsStatic { get; set; }
}
