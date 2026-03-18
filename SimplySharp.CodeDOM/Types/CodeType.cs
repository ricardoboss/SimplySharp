namespace SimplySharp.CodeDOM.Types;

public abstract class CodeType
{
	public CodeNamespace? Namespace { get; set; }

	public required string Name { get; set; }

	public abstract Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default);
}
