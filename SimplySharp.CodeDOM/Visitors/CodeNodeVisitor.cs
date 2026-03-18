using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Visitors;

public class CodeNodeVisitor : ICodeNodeVisitor
{
	public virtual Task VisitAsync(FieldNode fieldNode, CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}

	public virtual Task VisitAsync(ClassType classType, CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}
}
