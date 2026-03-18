using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Represents an indexer declaration within a type.
/// </summary>
public class IndexerNode : MemberNode
{
	/// <summary>
	/// Gets or sets the return type of this indexer.
	/// </summary>
	public required TypeRef ReturnType { get; set; }

	/// <summary>
	/// Gets the list of parameters for this indexer.
	/// </summary>
	public IList<Parameter> Parameters { get; init; } = [];

	/// <summary>
	/// Gets or sets a value indicating whether this indexer has a getter.
	/// </summary>
	public bool HasGetter { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether this indexer has a setter.
	/// </summary>
	public bool HasSetter { get; set; } = true;

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitIndexerAsync(this, cancellationToken);
	}
}
