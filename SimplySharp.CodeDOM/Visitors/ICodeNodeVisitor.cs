using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Visitors;

public interface ICodeNodeVisitor
{
	Task VisitAsync(FieldNode fieldNode, CancellationToken cancellationToken = default);

	Task VisitAsync(ClassType classType, CancellationToken cancellationToken = default);
}
