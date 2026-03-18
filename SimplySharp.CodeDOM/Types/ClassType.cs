using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

public class ClassType : CodeType
{
	public ClassType()
	{
		Nodes ??= new(this);
	}

	public TypeRef? Extends { get; set; }

	public ICollection<TypeRef> Implements { get; init; } = [];

	public CodeNodeCollection<CodeNode> Nodes { get; init; }

	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitClassTypeAsync(this, cancellationToken);
	}
}
