using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Visitors;

namespace SimplySharp.CodeDOM.Types;

public interface IType<TNodes> where TNodes : ICodeNode
{
	string Name { get; set; }

	ICodeNodeCollection<TNodes> Nodes { get; init; }

	Task AcceptAsync(ITypeVisitor visitor, CancellationToken cancellationToken = default);
}
