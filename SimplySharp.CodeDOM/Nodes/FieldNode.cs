using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Represents a field declaration within a type.
/// </summary>
public class FieldNode : MemberNode
{
	/// <summary>
	/// Gets or sets the type of this field.
	/// </summary>
	public required TypeRef Type { get; set; }

	/// <summary>
	/// Gets or sets the name of this field.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this field is declared as readonly.
	/// </summary>
	public bool IsReadonly { get; set; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitFieldAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
