namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Specifies the kind of a record type declaration.
/// </summary>
public enum RecordKind
{
	/// <summary>
	/// A record class (<c>record</c> or <c>record class</c>).
	/// </summary>
	Class,

	/// <summary>
	/// A record struct (<c>record struct</c>).
	/// </summary>
	Struct,
}
