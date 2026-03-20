using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Represents an operator overload declaration within a type.
/// </summary>
public class OperatorNode : MemberNode
{
	/// <summary>
	/// Gets or sets the kind of operator being overloaded.
	/// </summary>
	public required OperatorKind Kind { get; set; }

	/// <summary>
	/// Gets or sets the return type of this operator.
	/// </summary>
	public required TypeRef ReturnType { get; set; }

	/// <summary>
	/// Gets the list of parameters for this operator.
	/// </summary>
	public IList<Parameter> Parameters { get; init; } = [];

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitOperatorAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
