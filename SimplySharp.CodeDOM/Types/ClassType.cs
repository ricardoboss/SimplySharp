using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

public class ClassType : CodeType
{
	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	public TypeRef? Extends { get; set; }

	public ICollection<TypeRef> Implements { get; init; } = [];

	public ICollection<IClassNode> Nodes { get; init; } = [];

	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitClassTypeAsync(this, cancellationToken);
	}
}
