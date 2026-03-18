namespace SimplySharp.CodeDOM.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class RequiresLanguageVersionAttribute(int major) : Attribute
{
	public int Major { get; } = major;
}
