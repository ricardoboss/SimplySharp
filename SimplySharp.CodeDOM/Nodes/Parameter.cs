using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Specifies the modifier applied to a method or indexer parameter.
/// </summary>
public enum ParameterModifier
{
	/// <summary>
	/// No modifier; the parameter is passed by value.
	/// </summary>
	None,

	/// <summary>
	/// The parameter is passed by reference.
	/// </summary>
	Ref,

	/// <summary>
	/// The parameter is an output parameter.
	/// </summary>
	Out,

	/// <summary>
	/// The parameter is a read-only reference parameter.
	/// </summary>
	In,

	/// <summary>
	/// The parameter accepts a variable number of arguments.
	/// </summary>
	Params,
}

/// <summary>
/// Represents a parameter in a method, constructor, delegate, or indexer declaration.
/// </summary>
/// <param name="Type">The type of the parameter.</param>
/// <param name="Name">The name of the parameter.</param>
/// <param name="Modifier">The parameter modifier (ref, out, in, params, or none).</param>
public record Parameter(TypeRef Type, string Name, ParameterModifier Modifier = ParameterModifier.None);
