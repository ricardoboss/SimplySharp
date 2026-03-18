using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class FieldNodeTests
{
	[Test]
	public void FieldNode_Extends_MemberNode()
	{
		var field = new FieldNode { Type = TypeRef.Int, Name = "x" };

		Assert.That(field, Is.InstanceOf<MemberNode>());
	}

	[Test]
	public void FieldNode_DefaultAccessModifier_IsPublic()
	{
		var field = new FieldNode { Type = TypeRef.Int, Name = "x" };

		Assert.That(field.AccessModifier, Is.EqualTo(AccessModifier.Public));
	}

	[Test]
	public void FieldNode_IsStatic_DefaultsFalse()
	{
		var field = new FieldNode { Type = TypeRef.Int, Name = "x" };

		Assert.That(field.IsStatic, Is.False);
	}

	[Test]
	public void FieldNode_IsReadonly_DefaultsFalse()
	{
		var field = new FieldNode { Type = TypeRef.Int, Name = "x" };

		Assert.That(field.IsReadonly, Is.False);
	}

	[Test]
	public void FieldNode_IsReadonly_CanBeSet()
	{
		var field = new FieldNode { Type = TypeRef.Int, Name = "x", IsReadonly = true };

		Assert.That(field.IsReadonly, Is.True);
	}

	[Test]
	public async Task FieldNode_AcceptAsync_CallsVisitFieldAsync()
	{
		var field = new FieldNode { Type = TypeRef.Int, Name = "x" };
		var visitor = new Mock<CodeDomVisitor>();

		await field.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitFieldAsync(field, It.IsAny<CancellationToken>()), Times.Once);
	}
}
