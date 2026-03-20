using SimplySharp.CodeDOM.Attributes;
using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a <see cref="GenericCodeType"/> that can contain a primary constructor
/// (<see cref="PrimaryConstructorParameters"/>) and implement interfaces. All inheritors also support
/// <see cref="CodeNode"/> children.
/// </summary>
public abstract class ConstructibleCodeType : GenericCodeType
{
	/// <summary>
	/// Initializes <see cref="Nodes"/> with this instance as the owner.
	/// </summary>
	protected ConstructibleCodeType()
	{
		Nodes ??= new(this);
	}

	/// <summary>
	/// Gets or sets the primary constructor parameters for this class (C# 12), or <see langword="null"/> if not using a primary constructor.
	/// </summary>
	[RequiresLanguageVersion(12)]
	public IList<Parameter>? PrimaryConstructorParameters { get; init; }

	/// <summary>
	/// Gets the collection of interfaces this class implements.
	/// </summary>
	public ICollection<TypeRef> Implements { get; init; } = [];

	/// <summary>
	/// Gets the collection of member nodes contained in this class.
	/// </summary>
	public CodeNodeCollection<CodeNode> Nodes { get; init; }
}
