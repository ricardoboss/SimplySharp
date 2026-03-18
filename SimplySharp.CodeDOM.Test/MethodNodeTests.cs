using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class MethodNodeTests
{
	[Test]
	public void MethodNode_Extends_MemberNode()
	{
		var method = new MethodNode { ReturnType = TypeRef.Void, Name = "DoStuff" };

		Assert.That(method, Is.InstanceOf<MemberNode>());
	}

	[Test]
	public void MethodNode_Parameters_IsEmpty_ByDefault()
	{
		var method = new MethodNode { ReturnType = TypeRef.Void, Name = "DoStuff" };

		Assert.That(method.Parameters, Is.Empty);
	}

	[Test]
	public void MethodNode_Parameters_CanBePopulated()
	{
		var method = new MethodNode
		{
			ReturnType = TypeRef.Void,
			Name = "DoStuff",
			Parameters = [new Parameter(TypeRef.Int, "x"), new Parameter(TypeRef.String, "name")],
		};

		Assert.That(method.Parameters, Has.Count.EqualTo(2));
	}

	[Test]
	public void MethodNode_BooleanModifiers_DefaultFalse()
	{
		var method = new MethodNode { ReturnType = TypeRef.Void, Name = "DoStuff" };

		Assert.Multiple(() =>
		{
			Assert.That(method.IsAbstract, Is.False);
			Assert.That(method.IsVirtual, Is.False);
			Assert.That(method.IsOverride, Is.False);
			Assert.That(method.IsAsync, Is.False);
			Assert.That(method.IsStatic, Is.False);
		});
	}

	[Test]
	public async Task MethodNode_AcceptAsync_CallsVisitMethodAsync()
	{
		var method = new MethodNode { ReturnType = TypeRef.Void, Name = "DoStuff" };
		var visitor = new Mock<CodeDomVisitor>();

		await method.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitMethodAsync(method, It.IsAny<CancellationToken>()), Times.Once);
	}
}
