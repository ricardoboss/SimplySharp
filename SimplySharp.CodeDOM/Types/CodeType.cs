using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

public abstract class CodeType : CodeNode
{
	public CodeNamespace? ContainingNamespace { get; internal set; }

	public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

	public required string Name { get; set; }
}
