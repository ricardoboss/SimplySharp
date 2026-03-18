using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class EnumTypeTests
{
	[Test]
	public void EnumType_UnderlyingType_IsNull_ByDefault()
	{
		var e = new EnumType { Name = "Color" };

		Assert.That(e.UnderlyingType, Is.Null);
	}

	[Test]
	public void EnumType_UnderlyingType_CanBeSet()
	{
		var e = new EnumType { Name = "Color", UnderlyingType = TypeRef.Byte };

		Assert.That(e.UnderlyingType, Is.EqualTo(TypeRef.Byte));
	}

	[Test]
	public void EnumType_Members_SetsParent()
	{
		var e = new EnumType { Name = "Color" };
		var member = new EnumMemberNode { Name = "Red" };

		e.Members.Add(member);

		Assert.That(member.Parent, Is.SameAs(e));
	}

	[Test]
	public void EnumMemberNode_Value_IsNull_ByDefault()
	{
		var member = new EnumMemberNode { Name = "Red" };

		Assert.That(member.Value, Is.Null);
	}

	[Test]
	public void EnumMemberNode_Value_CanBeSet()
	{
		var member = new EnumMemberNode { Name = "Red", Value = 1 };

		Assert.That(member.Value, Is.EqualTo(1));
	}

	[Test]
	public async Task EnumType_AcceptAsync_CallsVisitEnumTypeAsync()
	{
		var e = new EnumType { Name = "Color" };
		var visitor = new Mock<CodeDomVisitor>();

		await e.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitEnumTypeAsync(e, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task EnumMemberNode_AcceptAsync_CallsVisitEnumMemberAsync()
	{
		var member = new EnumMemberNode { Name = "Red" };
		var visitor = new Mock<CodeDomVisitor>();

		await member.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitEnumMemberAsync(member, It.IsAny<CancellationToken>()), Times.Once);
	}
}
