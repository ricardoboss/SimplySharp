using System.Collections.ObjectModel;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM;

public class CodeNamespace
{
	public const char NamespaceSeparator = '.';

	public CodeNamespace? Parent
	{
		get;
		init
		{
			field?.children.Remove(this);
			value?.children.Add(this);

			field = value;
		}
	}

	private readonly List<CodeNamespace> children = [];

	public IReadOnlyList<CodeNamespace> Children => children;

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

	public CodeNamespace Root => Parent?.Root ?? this;

	private readonly Collection<CodeType> types = [];

	public IReadOnlyCollection<CodeType> Types => types;

	public void AddType(CodeType type)
	{
		types.Add(type);

		type.Namespace = this;
	}

	public void RemoveType(CodeType type)
	{
		type.Namespace = null;

		types.Remove(type);
	}

	public CodeType? FindTypeByName(string typeName)
	{
		var type = types.FirstOrDefault(t => t.Name == typeName);
		if (type is not null)
			return type;

		foreach (var child in Children)
		{
			type = child.FindTypeByName(typeName);
			if (type is not null)
				return type;
		}

		return null;
	}

	public CodeNamespace? FindContainingNamespace(string typeName)
	{
		var type = types.FirstOrDefault(t => t.Name == typeName);
		if (type is not null)
			return this;

		return Children
			.Select(child => child.FindContainingNamespace(typeName))
			.OfType<CodeNamespace>()
			.FirstOrDefault();
	}

	public async Task AcceptAsync(CodeDomVisitor visitor, CancellationToken cancellationToken = default)
	{
		await visitor.VisitNamespaceAsync(this, cancellationToken);
	}
}
