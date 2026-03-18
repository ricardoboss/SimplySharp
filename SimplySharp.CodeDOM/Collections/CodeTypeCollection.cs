using System.Collections.ObjectModel;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Collections;

public class CodeTypeCollection(CodeNamespace owner) : Collection<CodeType>
{
	protected override void InsertItem(int index, CodeType item)
	{
		item.ContainingNamespace = owner;

		base.InsertItem(index, item);
	}

	protected override void RemoveItem(int index)
	{
		this[index].ContainingNamespace = null;

		base.RemoveItem(index);
	}

	protected override void SetItem(int index, CodeType item)
	{
		this[index].ContainingNamespace = null;
		item.ContainingNamespace = owner;

		base.SetItem(index, item);
	}

	protected override void ClearItems()
	{
		foreach (var item in this)
			item.ContainingNamespace = null;

		base.ClearItems();
	}
}
