namespace SimplySharp.CodeDOM.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class RequiresLanguageVersionAttribute(int major) : Attribute
{
	public int Major { get; } = major;
}
