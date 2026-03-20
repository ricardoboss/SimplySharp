namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a record type declaration (<c>record class</c> or <c>record struct</c>).
/// </summary>
public class RecordType : ConstructibleCodeType
{
	/// <summary>
	/// Gets or sets the kind of this record (class or struct).
	/// </summary>
	public RecordKind Kind { get; set; } = RecordKind.Class;

	/// <summary>
	/// Gets or sets the base record this record extends, or <see langword="null"/> if none.
	/// Only valid when <see cref="Kind"/> is <see cref="RecordKind.Class"/>.
	/// </summary>
	public TypeRef? Extends { get; set; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(visitor);

		await visitor.VisitRecordTypeAsync(this, cancellationToken).ConfigureAwait(false);
	}
}
