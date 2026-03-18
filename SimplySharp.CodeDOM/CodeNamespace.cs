using SimplySharp.CodeDOM.Collections;

namespace SimplySharp.CodeDOM;

public class CodeNamespace
{
	public const char NamespaceSeparator = '.';

	public CodeWorkspace? Workspace { get; internal set; }

	public CodeNamespace? Parent { get; internal set; }

	public CodeNamespaceCollection Children { get; }

	public CodeTypeCollection Types { get; }

	public CodeNamespace Root => Parent?.Root ?? this;

	public required string Name { get; init; }

	public string FullName
	{
		get
		{
			var parentFullName = Parent?.FullName;

			return string.IsNullOrWhiteSpace(parentFullName)
				? Name
				: $"{parentFullName}{NamespaceSeparator}{Name}";
		}
	}

	public CodeNamespace()
	{
		Children = new(this);
		Types = new(this);
	}

	public async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitNamespaceAsync(this, cancellationToken);
	}
}
