namespace SimplySharp.CodeDOM.Nodes;

public interface ICodeNode
{
	Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default);
}
