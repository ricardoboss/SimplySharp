namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a struct type declaration.
/// </summary>
public class StructType : ConstructibleCodeType
{
	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitStructTypeAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
