using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents an interface type declaration.
/// </summary>
public class InterfaceType : GenericCodeType
{
	/// <summary>
	/// Initializes a new instance of the <see cref="InterfaceType"/> class.
	/// </summary>
	public InterfaceType()
	{
		Nodes ??= new(this);
	}

	/// <summary>
	/// Gets the collection of interfaces this interface extends.
	/// </summary>
	public ICollection<TypeRef> Extends { get; init; } = [];

	/// <summary>
	/// Gets the collection of member nodes contained in this interface.
	/// </summary>
	public CodeNodeCollection<CodeNode> Nodes { get; init; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitInterfaceTypeAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
