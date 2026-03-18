using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents an enum type declaration.
/// </summary>
public class EnumType : CodeType
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EnumType"/> class.
	/// </summary>
	public EnumType()
	{
		Members ??= new(this);
	}

	/// <summary>
	/// Gets or sets the underlying type of this enum, or <see langword="null"/> for the default (<c>int</c>).
	/// </summary>
	public TypeRef? UnderlyingType { get; set; }

	/// <summary>
	/// Gets the collection of enum members declared in this enum.
	/// </summary>
	public CodeNodeCollection<EnumMemberNode> Members { get; init; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitEnumTypeAsync(this, cancellationToken);
	}
}
