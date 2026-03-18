using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Nodes;

public class FieldNode : CodeNode
{
	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	public required TypeRef Type { get; set; }

	public required string Name { get; set; }

	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitFieldAsync(this, cancellationToken);
	}
}
