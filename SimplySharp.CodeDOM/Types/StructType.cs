using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a struct type declaration.
/// </summary>
public class StructType : GenericCodeType
{
	/// <summary>
	/// Initializes a new instance of the <see cref="StructType"/> class.
	/// </summary>
	public StructType()
	{
		Nodes ??= new(this);
	}

	/// <summary>
	/// Gets the collection of interfaces this struct implements.
	/// </summary>
	public ICollection<TypeRef> Implements { get; init; } = [];

	/// <summary>
	/// Gets or sets the primary constructor parameters for this struct (C# 12), or <see langword="null"/> if not using a primary constructor.
	/// </summary>
	public IList<Parameter>? PrimaryConstructorParameters { get; set; }

	/// <summary>
	/// Gets the collection of member nodes contained in this struct.
	/// </summary>
	public CodeNodeCollection<CodeNode> Nodes { get; init; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitStructTypeAsync(this, cancellationToken);
	}
}
