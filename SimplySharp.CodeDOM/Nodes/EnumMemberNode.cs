namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Represents a single member (case) within an enum declaration.
/// </summary>
public class EnumMemberNode : CodeNode
{
	/// <summary>
	/// Gets or sets the name of this enum member.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets the explicit value assigned to this enum member, or <see langword="null"/> for the compiler default.
	/// </summary>
	public object? Value { get; set; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitEnumMemberAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
