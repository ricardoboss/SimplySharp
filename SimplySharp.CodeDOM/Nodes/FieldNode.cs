namespace SimplySharp.CodeDOM.Nodes;

public class FieldNode : ICodeNode
{
	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	public required string Name { get; set; }

	public async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitFieldAsync(this, cancellationToken);
	}
}
