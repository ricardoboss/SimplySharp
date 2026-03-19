namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Specifies the variance of a generic type parameter.
/// </summary>
public enum Variance
{
	/// <summary>
	/// The type parameter is invariant.
	/// </summary>
	None,

	/// <summary>
	/// The type parameter is contravariant (<c>in</c>).
	/// </summary>
	In,

	/// <summary>
	/// The type parameter is covariant (<c>out</c>).
	/// </summary>
	Out,
}
