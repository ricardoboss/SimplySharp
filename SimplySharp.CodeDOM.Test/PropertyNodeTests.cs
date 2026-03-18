using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class PropertyNodeTests
{
	[Test]
	public void PropertyNode_Extends_MemberNode()
	{
		var prop = new PropertyNode { Type = TypeRef.String, Name = "Foo" };

		Assert.That(prop, Is.InstanceOf<MemberNode>());
	}

	[Test]
	public void PropertyNode_HasGetter_DefaultsTrue()
	{
		var prop = new PropertyNode { Type = TypeRef.String, Name = "Foo" };

		Assert.That(prop.HasGetter, Is.True);
	}

	[Test]
	public void PropertyNode_HasSetter_DefaultsTrue()
	{
		var prop = new PropertyNode { Type = TypeRef.String, Name = "Foo" };

		Assert.That(prop.HasSetter, Is.True);
	}

	[Test]
	public void PropertyNode_IsSetterInit_DefaultsFalse()
	{
		var prop = new PropertyNode { Type = TypeRef.String, Name = "Foo" };

		Assert.That(prop.IsSetterInit, Is.False);
	}

	[Test]
	public void PropertyNode_IsRequired_DefaultsFalse()
	{
		var prop = new PropertyNode { Type = TypeRef.String, Name = "Foo" };

		Assert.That(prop.IsRequired, Is.False);
	}

	[Test]
	public async Task PropertyNode_AcceptAsync_CallsVisitPropertyAsync()
	{
		var prop = new PropertyNode { Type = TypeRef.String, Name = "Foo" };
		var visitor = new Mock<CodeDomVisitor>();

		await prop.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitPropertyAsync(prop, It.IsAny<CancellationToken>()), Times.Once);
	}
}
