namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Specifies the kind of constructor initializer (<c>: base(...)</c> or <c>: this(...)</c>).
/// </summary>
public enum ConstructorInitializerKind
{
	/// <summary>
	/// Calls a base class constructor (<c>: base(...)</c>).
	/// </summary>
	Base,

	/// <summary>
	/// Calls another constructor on the same type (<c>: this(...)</c>).
	/// </summary>
	This,
}

/// <summary>
/// Represents a constructor declaration within a type.
/// </summary>
public class ConstructorNode : MemberNode
{
	/// <summary>
	/// Gets the list of parameters for this constructor.
	/// </summary>
	public IList<Parameter> Parameters { get; init; } = [];

	/// <summary>
	/// Gets or sets the constructor initializer kind (<c>base</c> or <c>this</c>), or <see langword="null"/> if no initializer is used.
	/// </summary>
	public ConstructorInitializerKind? Initializer { get; set; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitConstructorAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
