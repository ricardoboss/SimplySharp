using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class EventNodeTests
{
	[Test]
	public void EventNode_Extends_MemberNode()
	{
		var evt = new EventNode { Type = new NamedTypeRef("EventHandler"), Name = "Click" };

		Assert.That(evt, Is.InstanceOf<MemberNode>());
	}

	[Test]
	public void EventNode_DefaultAccessModifier_IsPublic()
	{
		var evt = new EventNode { Type = new NamedTypeRef("EventHandler"), Name = "Click" };

		Assert.That(evt.AccessModifier, Is.EqualTo(AccessModifier.Public));
	}

	[Test]
	public async Task EventNode_AcceptAsync_CallsVisitEventAsync()
	{
		var evt = new EventNode { Type = new NamedTypeRef("EventHandler"), Name = "Click" };
		var visitor = new Mock<CodeDomVisitor>();

		await evt.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitEventAsync(evt, It.IsAny<CancellationToken>()), Times.Once);
	}
}
