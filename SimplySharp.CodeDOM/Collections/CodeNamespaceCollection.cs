using System.Collections.ObjectModel;

namespace SimplySharp.CodeDOM.Collections;

public class CodeNamespaceCollection : Collection<CodeNamespace>
{
	private readonly CodeNamespace? namespaceOwner;
	private readonly CodeWorkspace? workspaceOwner;

	public CodeNamespaceCollection(CodeNamespace owner) => namespaceOwner = owner;

	public CodeNamespaceCollection(CodeWorkspace owner) => workspaceOwner = owner;

	protected override void InsertItem(int index, CodeNamespace item)
	{
		ArgumentNullException.ThrowIfNull(item);

		item.Parent = namespaceOwner;
		item.Workspace = workspaceOwner;

		base.InsertItem(index, item);
	}

	protected override void RemoveItem(int index)
	{
		this[index].Parent = null;
		this[index].Workspace = null;

		base.RemoveItem(index);
	}

	protected override void SetItem(int index, CodeNamespace item)
	{
		ArgumentNullException.ThrowIfNull(item);

		this[index].Parent = null;
		this[index].Workspace = null;
		item.Parent = namespaceOwner;
		item.Workspace = workspaceOwner;

		base.SetItem(index, item);
	}

	protected override void ClearItems()
	{
		foreach (var item in this)
		{
			item.Parent = null;
			item.Workspace = null;
		}

		base.ClearItems();
	}
}
