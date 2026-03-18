using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM;

public abstract class CodeDomVisitor
{
	public virtual async Task VisitWorkspaceAsync(CodeWorkspace workspace,
		CancellationToken cancellationToken = default)
	{
		foreach (var ns in workspace.Namespaces)
			await ns.AcceptAsync(this, cancellationToken);
	}

	public virtual async Task VisitNamespaceAsync(CodeNamespace ns, CancellationToken cancellationToken = default)
	{
		foreach (var type in ns.Types)
			await type.AcceptAsync(this, cancellationToken);

		foreach (var child in ns.Children)
			await child.AcceptAsync(this, cancellationToken);
	}

	public virtual async Task VisitClassTypeAsync(ClassType classType, CancellationToken cancellationToken = default)
	{
		foreach (var node in classType.Nodes)
			await node.AcceptAsync(this, cancellationToken);
	}

	public virtual Task VisitFieldAsync(FieldNode fieldNode, CancellationToken cancellationToken = default) =>
		Task.CompletedTask;
}
