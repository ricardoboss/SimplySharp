namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a generic type parameter declaration (e.g., <c>T</c> in <c>class Foo&lt;T&gt;</c>),
/// distinct from <see cref="TypeRef"/> which represents type references/arguments.
/// </summary>
public class GenericParameter
{
	/// <summary>
	/// Gets or sets the name of this generic type parameter (e.g., <c>T</c>, <c>TKey</c>).
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets the variance of this generic type parameter.
	/// Only meaningful on interfaces and delegates.
	/// </summary>
	public Variance Variance { get; set; } = Variance.None;

	/// <summary>
	/// Gets the collection of constraints applied to this generic type parameter
	/// (e.g., <c>where T : class, IDisposable, new()</c>).
	/// </summary>
	public IList<GenericConstraint> Constraints { get; init; } = [];
}
