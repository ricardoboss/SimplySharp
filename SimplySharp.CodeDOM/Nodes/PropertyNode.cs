using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Represents a property declaration within a type.
/// </summary>
public class PropertyNode : MemberNode
{
	/// <summary>
	/// Gets or sets the type of this property.
	/// </summary>
	public required TypeRef Type { get; set; }

	/// <summary>
	/// Gets or sets the name of this property.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this property has a getter.
	/// </summary>
	public bool HasGetter { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether this property has a setter.
	/// </summary>
	public bool HasSetter { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether the setter is an <c>init</c> accessor.
	/// </summary>
	public bool IsSetterInit { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this property is declared with the <c>required</c> modifier.
	/// </summary>
	public bool IsRequired { get; set; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitPropertyAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
