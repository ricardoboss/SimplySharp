using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Visitors;

namespace SimplySharp.CodeDOM.Types;

public class ClassType : IType<IClassNode>, IClassNode
{
	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	public required string Name { get; set; }

	public ICodeNodeCollection<IClassNode> Nodes { get; init; } = new CodeNodeCollection<IClassNode>();

	public async Task AcceptAsync(ITypeVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitAsync(this, cancellationToken);
	}

	public async Task AcceptAsync(ICodeNodeVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitAsync(this, cancellationToken);
	}
}
