using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Represents a method declaration within a type.
/// </summary>
public class MethodNode : MemberNode
{
	/// <summary>
	/// Gets or sets the return type of this method.
	/// </summary>
	public required TypeRef ReturnType { get; set; }

	/// <summary>
	/// Gets or sets the name of this method.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Gets the list of parameters for this method.
	/// </summary>
	public IList<Parameter> Parameters { get; init; } = [];

	/// <summary>
	/// Gets or sets a value indicating whether this method is declared as abstract.
	/// </summary>
	public bool IsAbstract { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this method is declared as virtual.
	/// </summary>
	public bool IsVirtual { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this method is declared as override.
	/// </summary>
	public bool IsOverride { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this method is declared as async.
	/// </summary>
	public bool IsAsync { get; set; }

	/// <summary>
	/// Gets the list of generic type parameters declared on this method
	/// (e.g., <c>T</c> in <c>void DoStuff&lt;T&gt;(T value)</c>).
	/// </summary>
	public IList<GenericParameter> GenericParameters { get; init; } = [];

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitMethodAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
