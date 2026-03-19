namespace SimplySharp.CodeDOM.Test;

public class CodeNamespaceTests
{
	[Test]
	public void FullName_TopLevel_ReturnsName()
	{
		var ns = new CodeNamespace { Name = "System" };

		Assert.That(ns.FullName, Is.EqualTo("System"));
	}

	[Test]
	public void FullName_Nested_ReturnsDotSeparatedPath()
	{
		var parent = new CodeNamespace { Name = "System" };
		var child = new CodeNamespace { Name = "Collections" };
		parent.Children.Add(child);

		Assert.That(child.FullName, Is.EqualTo("System.Collections"));
	}

	[Test]
	public void FullName_DeeplyNested_ReturnsFullPath()
	{
		var root = new CodeNamespace { Name = "System" };
		var mid = new CodeNamespace { Name = "Collections" };
		var leaf = new CodeNamespace { Name = "Generic" };
		root.Children.Add(mid);
		mid.Children.Add(leaf);

		Assert.That(leaf.FullName, Is.EqualTo("System.Collections.Generic"));
	}

	[Test]
	public void Root_TopLevel_ReturnsSelf()
	{
		var ns = new CodeNamespace { Name = "System" };

		Assert.That(ns.Root, Is.SameAs(ns));
	}

	[Test]
	public void Root_Nested_ReturnsTopLevel()
	{
		var root = new CodeNamespace { Name = "System" };
		var mid = new CodeNamespace { Name = "Collections" };
		var leaf = new CodeNamespace { Name = "Generic" };
		root.Children.Add(mid);
		mid.Children.Add(leaf);

		Assert.That(leaf.Root, Is.SameAs(root));
	}

	[Test]
	public void Root_AfterRemoval_ReturnsSelf()
	{
		var parent = new CodeNamespace { Name = "System" };
		var child = new CodeNamespace { Name = "Collections" };
		parent.Children.Add(child);

		parent.Children.Remove(child);

		Assert.That(child.Root, Is.SameAs(child));
	}
}
