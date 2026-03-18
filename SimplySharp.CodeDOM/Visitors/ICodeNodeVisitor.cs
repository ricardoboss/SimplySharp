using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Visitors;

public interface ICodeNodeVisitor
{
	Task VisitAsync(FieldNode fieldNode, CancellationToken cancellationToken = default);
}
