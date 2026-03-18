namespace SimplySharp.CodeDOM.Nodes;

public abstract class CodeNode
{
	public CodeNode? Parent { get; internal set; }

	public abstract Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default);
}
