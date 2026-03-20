using System.Collections.ObjectModel;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Collections;

public class CodeNodeCollection<T>(CodeNode owner) : Collection<T> where T : CodeNode
{
	protected override void InsertItem(int index, T item)
	{
		ArgumentNullException.ThrowIfNull(item);

		item.Parent = owner;

		base.InsertItem(index, item);
	}

	protected override void RemoveItem(int index)
	{
		this[index].Parent = null;

		base.RemoveItem(index);
	}

	protected override void SetItem(int index, T item)
	{
		ArgumentNullException.ThrowIfNull(item);

		this[index].Parent = null;
		item.Parent = owner;

		base.SetItem(index, item);
	}

	protected override void ClearItems()
	{
		foreach (var item in this)
			item.Parent = null;

		base.ClearItems();
	}
}
