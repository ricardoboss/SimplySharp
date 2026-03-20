using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Represents an event declaration within a type.
/// </summary>
public class EventNode : MemberNode
{
	/// <summary>
	/// Gets or sets the delegate type of this event.
	/// </summary>
	public required TypeRef Type { get; set; }

	/// <summary>
	/// Gets or sets the name of this event.
	/// </summary>
	public required string Name { get; set; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitEventAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
