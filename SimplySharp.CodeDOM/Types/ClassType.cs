using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a class type declaration.
/// </summary>
public class ClassType : CodeType
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ClassType"/> class.
	/// </summary>
	public ClassType()
	{
		Nodes ??= new(this);
	}

	/// <summary>
	/// Gets or sets the base class this class extends, or <see langword="null"/> if none.
	/// </summary>
	public TypeRef? Extends { get; set; }

	/// <summary>
	/// Gets the collection of interfaces this class implements.
	/// </summary>
	public ICollection<TypeRef> Implements { get; init; } = [];

	/// <summary>
	/// Gets or sets the primary constructor parameters for this class (C# 12), or <see langword="null"/> if not using a primary constructor.
	/// </summary>
	public IList<Parameter>? PrimaryConstructorParameters { get; set; }

	/// <summary>
	/// Gets the collection of member nodes contained in this class.
	/// </summary>
	public CodeNodeCollection<CodeNode> Nodes { get; init; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitClassTypeAsync(this, cancellationToken);
	}
}
