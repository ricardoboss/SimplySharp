using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM;

/// <summary>
/// Abstract base class for visiting all nodes in a code DOM tree.
/// Override virtual methods to handle specific node types.
/// </summary>
public abstract class CodeDomVisitor
{
	/// <summary>
	/// Visits a workspace node, iterating over all its namespaces.
	/// </summary>
	/// <param name="workspace">The workspace to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual async Task VisitWorkspaceAsync(CodeWorkspace workspace,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(workspace);

		foreach (var ns in workspace.Namespaces)
			await ns.AcceptAsync(this, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Visits a namespace node, iterating over its types and child namespaces.
	/// </summary>
	/// <param name="ns">The namespace to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual async Task VisitNamespaceAsync(CodeNamespace ns, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(ns);

		foreach (var type in ns.Types)
			await type.AcceptAsync(this, cancellationToken).ConfigureAwait(false);

		foreach (var child in ns.Children)
			await child.AcceptAsync(this, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Visits a class type declaration, iterating over its member nodes.
	/// </summary>
	/// <param name="classType">The class type to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual async Task VisitClassTypeAsync(ClassType classType, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(classType);

		foreach (var node in classType.Nodes)
			await node.AcceptAsync(this, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Visits an interface type declaration, iterating over its member nodes.
	/// </summary>
	/// <param name="interfaceType">The interface type to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual async Task VisitInterfaceTypeAsync(InterfaceType interfaceType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(interfaceType);

		foreach (var node in interfaceType.Nodes)
			await node.AcceptAsync(this, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Visits a struct type declaration, iterating over its member nodes.
	/// </summary>
	/// <param name="structType">The struct type to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual async Task VisitStructTypeAsync(StructType structType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(structType);

		foreach (var node in structType.Nodes)
			await node.AcceptAsync(this, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Visits a record type declaration, iterating over its member nodes.
	/// </summary>
	/// <param name="recordType">The record type to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual async Task VisitRecordTypeAsync(RecordType recordType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(recordType);

		foreach (var node in recordType.Nodes)
			await node.AcceptAsync(this, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Visits an enum type declaration, iterating over its members.
	/// </summary>
	/// <param name="enumType">The enum type to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual async Task VisitEnumTypeAsync(EnumType enumType, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(enumType);

		foreach (var member in enumType.Members)
			await member.AcceptAsync(this, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Visits a delegate type declaration.
	/// </summary>
	/// <param name="delegateType">The delegate type to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitDelegateTypeAsync(DelegateType delegateType,
		CancellationToken cancellationToken = default) =>
		Task.CompletedTask;

	/// <summary>
	/// Visits a field declaration.
	/// </summary>
	/// <param name="fieldNode">The field node to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitFieldAsync(FieldNode fieldNode, CancellationToken cancellationToken = default) =>
		Task.CompletedTask;

	/// <summary>
	/// Visits a property declaration.
	/// </summary>
	/// <param name="propertyNode">The property node to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitPropertyAsync(PropertyNode propertyNode, CancellationToken cancellationToken = default) =>
		Task.CompletedTask;

	/// <summary>
	/// Visits a method declaration.
	/// </summary>
	/// <param name="methodNode">The method node to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitMethodAsync(MethodNode methodNode, CancellationToken cancellationToken = default) =>
		Task.CompletedTask;

	/// <summary>
	/// Visits a constructor declaration.
	/// </summary>
	/// <param name="constructorNode">The constructor node to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitConstructorAsync(ConstructorNode constructorNode,
		CancellationToken cancellationToken = default) =>
		Task.CompletedTask;

	/// <summary>
	/// Visits an event declaration.
	/// </summary>
	/// <param name="eventNode">The event node to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitEventAsync(EventNode eventNode, CancellationToken cancellationToken = default) =>
		Task.CompletedTask;

	/// <summary>
	/// Visits an indexer declaration.
	/// </summary>
	/// <param name="indexerNode">The indexer node to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitIndexerAsync(IndexerNode indexerNode, CancellationToken cancellationToken = default) =>
		Task.CompletedTask;

	/// <summary>
	/// Visits an operator overload declaration.
	/// </summary>
	/// <param name="operatorNode">The operator node to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitOperatorAsync(OperatorNode operatorNode, CancellationToken cancellationToken = default) =>
		Task.CompletedTask;

	/// <summary>
	/// Visits an enum member declaration.
	/// </summary>
	/// <param name="enumMemberNode">The enum member node to visit.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	public virtual Task VisitEnumMemberAsync(EnumMemberNode enumMemberNode,
		CancellationToken cancellationToken = default) =>
		Task.CompletedTask;
}
