namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a class type declaration.
/// </summary>
public class ClassType : ConstructibleCodeType
{
	/// <summary>
	/// Gets or sets the base class this class extends, or <see langword="null"/> if none.
	/// </summary>
	public TypeRef? Extends { get; set; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitClassTypeAsync(this, cancellationToken);
	}
}
