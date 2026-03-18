using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a delegate type declaration.
/// </summary>
public class DelegateType : CodeType
{
	/// <summary>
	/// Gets or sets the return type of this delegate.
	/// </summary>
	public required TypeRef ReturnType { get; set; }

	/// <summary>
	/// Gets the list of parameters for this delegate.
	/// </summary>
	public IList<Parameter> Parameters { get; init; } = [];

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitDelegateTypeAsync(this, cancellationToken);
	}
}
