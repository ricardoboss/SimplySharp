using SimplySharp.CodeDOM.Collections;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Types;

/// <summary>
/// Represents a record type declaration (<c>record class</c> or <c>record struct</c>).
/// </summary>
public class RecordType : CodeType
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RecordType"/> class.
	/// </summary>
	public RecordType()
	{
		Nodes ??= new(this);
	}

	/// <summary>
	/// Gets or sets the kind of this record (class or struct).
	/// </summary>
	public RecordKind Kind { get; set; } = RecordKind.Class;

	/// <summary>
	/// Gets or sets the base record this record extends, or <see langword="null"/> if none.
	/// Only valid when <see cref="Kind"/> is <see cref="RecordKind.Class"/>.
	/// </summary>
	public TypeRef? Extends { get; set; }

	/// <summary>
	/// Gets the collection of interfaces this record implements.
	/// </summary>
	public ICollection<TypeRef> Implements { get; init; } = [];

	/// <summary>
	/// Gets or sets the primary constructor parameters for this record, or <see langword="null"/> if not using positional syntax.
	/// </summary>
	public IList<Parameter>? PrimaryConstructorParameters { get; set; }

	/// <summary>
	/// Gets the collection of member nodes contained in this record.
	/// </summary>
	public CodeNodeCollection<CodeNode> Nodes { get; init; }

	/// <inheritdoc />
	public override async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitRecordTypeAsync(this, cancellationToken);
	}
}
