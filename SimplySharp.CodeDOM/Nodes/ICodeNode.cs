using SimplySharp.CodeDOM.Visitors;

namespace SimplySharp.CodeDOM.Nodes;

public interface ICodeNode
{
	Task AcceptAsync(ICodeNodeVisitor visitor, CancellationToken cancellationToken = default);
}
