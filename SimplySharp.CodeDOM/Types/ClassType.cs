using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

public class ClassType : CodeType
{
	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	public ICodeNodeCollection<IClassNode> Nodes { get; init; } = new CodeNodeCollection<IClassNode>();

	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitClassTypeAsync(this, cancellationToken);
	}
}
