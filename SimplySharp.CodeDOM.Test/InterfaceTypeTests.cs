using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class InterfaceTypeTests
{
	[Test]
	public void InterfaceType_Extends_IsEmpty_ByDefault()
	{
		var iface = new InterfaceType { Name = "IFoo" };

		Assert.That(iface.Extends, Is.Empty);
	}

	[Test]
	public void InterfaceType_CanExtend_MultipleInterfaces()
	{
		var iface = new InterfaceType { Name = "IFoo" };
		iface.Extends.Add(new NamedTypeRef("IBar"));
		iface.Extends.Add(new NamedTypeRef("IBaz"));

		Assert.That(iface.Extends, Has.Count.EqualTo(2));
	}

	[Test]
	public void InterfaceType_Nodes_SetsParent()
	{
		var iface = new InterfaceType { Name = "IFoo" };
		var method = new MethodNode { ReturnType = TypeRef.Void, Name = "DoStuff" };

		iface.Nodes.Add(method);

		Assert.That(method.Parent, Is.SameAs(iface));
	}

	[Test]
	public async Task InterfaceType_AcceptAsync_CallsVisitInterfaceTypeAsync()
	{
		var iface = new InterfaceType { Name = "IFoo" };
		var visitor = new Mock<CodeDomVisitor>();

		await iface.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitInterfaceTypeAsync(iface, It.IsAny<CancellationToken>()), Times.Once);
	}
}
