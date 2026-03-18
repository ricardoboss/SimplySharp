using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

public class ClassType : CodeType
{
	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	public ICollection<IClassNode> Nodes { get; init; } = [];

	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitClassTypeAsync(this, cancellationToken);
	}
}
