namespace SimplySharp.CodeDOM.Nodes;

/// <summary>
/// Specifies the kind of operator being overloaded.
/// </summary>
public enum OperatorKind
{
	/// <summary>The binary addition operator (<c>+</c>).</summary>
	Addition,

	/// <summary>The binary subtraction operator (<c>-</c>).</summary>
	Subtraction,

	/// <summary>The binary multiplication operator (<c>*</c>).</summary>
	Multiply,

	/// <summary>The binary division operator (<c>/</c>).</summary>
	Division,

	/// <summary>The binary modulus operator (<c>%</c>).</summary>
	Modulus,

	/// <summary>The unary plus operator (<c>+</c>).</summary>
	UnaryPlus,

	/// <summary>The unary negation operator (<c>-</c>).</summary>
	UnaryNegation,

	/// <summary>The increment operator (<c>++</c>).</summary>
	Increment,

	/// <summary>The decrement operator (<c>--</c>).</summary>
	Decrement,

	/// <summary>The less-than comparison operator (<c>&lt;</c>).</summary>
	LessThan,

	/// <summary>The greater-than comparison operator (<c>&gt;</c>).</summary>
	GreaterThan,

	/// <summary>The less-than-or-equal comparison operator (<c>&lt;=</c>).</summary>
	LessThanOrEqual,

	/// <summary>The greater-than-or-equal comparison operator (<c>&gt;=</c>).</summary>
	GreaterThanOrEqual,

	/// <summary>The equality operator (<c>==</c>).</summary>
	Equality,

	/// <summary>The inequality operator (<c>!=</c>).</summary>
	Inequality,

	/// <summary>The logical NOT operator (<c>!</c>).</summary>
	LogicalNot,

	/// <summary>The bitwise AND operator (<c>&amp;</c>).</summary>
	BitwiseAnd,

	/// <summary>The bitwise OR operator (<c>|</c>).</summary>
	BitwiseOr,

	/// <summary>The exclusive OR operator (<c>^</c>).</summary>
	ExclusiveOr,

	/// <summary>The bitwise complement operator (<c>~</c>).</summary>
	BitwiseNot,

	/// <summary>An implicit conversion operator.</summary>
	Implicit,

	/// <summary>An explicit conversion operator.</summary>
	Explicit,

	/// <summary>The <c>true</c> operator.</summary>
	True,

	/// <summary>The <c>false</c> operator.</summary>
	False,
}
