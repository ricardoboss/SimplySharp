using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class CodeNodeCollectionTests
{
	[Test]
	public void CodeNodeCollection_Add_SetsParent()
	{
		var cls = new ClassType { Name = "Foo" };
		var field = new FieldNode { Type = TypeRef.Int, Name = "x" };

		cls.Nodes.Add(field);

		Assert.That(field.Parent, Is.SameAs(cls));
	}

	[Test]
	public void CodeNodeCollection_Remove_ClearsParent()
	{
		var cls = new ClassType { Name = "Foo" };
		var field = new FieldNode { Type = TypeRef.Int, Name = "x" };
		cls.Nodes.Add(field);

		cls.Nodes.Remove(field);

		Assert.That(field.Parent, Is.Null);
	}

	[Test]
	public void CodeNodeCollection_SetItem_ClearsOldParent_SetsNewParent()
	{
		var cls = new ClassType { Name = "Foo" };
		var oldField = new FieldNode { Type = TypeRef.Int, Name = "x" };
		var newField = new FieldNode { Type = TypeRef.String, Name = "y" };
		cls.Nodes.Add(oldField);

		cls.Nodes[0] = newField;

		Assert.Multiple(() =>
		{
			Assert.That(oldField.Parent, Is.Null);
			Assert.That(newField.Parent, Is.SameAs(cls));
		});
	}

	[Test]
	public void CodeNodeCollection_Clear_ClearsAllParents()
	{
		var cls = new ClassType { Name = "Foo" };
		var field1 = new FieldNode { Type = TypeRef.Int, Name = "a" };
		var field2 = new FieldNode { Type = TypeRef.Int, Name = "b" };
		var field3 = new FieldNode { Type = TypeRef.Int, Name = "c" };
		cls.Nodes.Add(field1);
		cls.Nodes.Add(field2);
		cls.Nodes.Add(field3);

		cls.Nodes.Clear();

		Assert.Multiple(() =>
		{
			Assert.That(field1.Parent, Is.Null);
			Assert.That(field2.Parent, Is.Null);
			Assert.That(field3.Parent, Is.Null);
		});
	}

	[Test]
	public void CodeTypeCollection_Add_SetsContainingNamespace()
	{
		var ns = new CodeNamespace { Name = "MyApp" };
		var cls = new ClassType { Name = "Foo" };

		ns.Types.Add(cls);

		Assert.That(cls.ContainingNamespace, Is.SameAs(ns));
	}

	[Test]
	public void CodeTypeCollection_Remove_ClearsContainingNamespace()
	{
		var ns = new CodeNamespace { Name = "MyApp" };
		var cls = new ClassType { Name = "Foo" };
		ns.Types.Add(cls);

		ns.Types.Remove(cls);

		Assert.That(cls.ContainingNamespace, Is.Null);
	}

	[Test]
	public void CodeNamespaceCollection_AddToNamespace_SetsParent()
	{
		var parent = new CodeNamespace { Name = "System" };
		var child = new CodeNamespace { Name = "Collections" };

		parent.Children.Add(child);

		Assert.That(child.Parent, Is.SameAs(parent));
	}

	[Test]
	public void CodeNamespaceCollection_AddToWorkspace_SetsWorkspace()
	{
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "MyApp" };

		workspace.Namespaces.Add(ns);

		Assert.That(ns.Workspace, Is.SameAs(workspace));
	}
}
