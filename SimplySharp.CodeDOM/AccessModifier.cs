using SimplySharp.CodeDOM.Attributes;

namespace SimplySharp.CodeDOM;

public enum AccessModifier
{
	Public,

	Protected,

	Internal,

	ProtectedInternal,

	Private,

	PrivateProtected,

	[RequiresLanguageVersion(10)]
	File,
}
