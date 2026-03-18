using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class StructTypeTests
{
	[Test]
	public void StructType_Implements_IsEmpty_ByDefault()
	{
		var s = new StructType { Name = "Foo" };

		Assert.That(s.Implements, Is.Empty);
	}

	[Test]
	public void StructType_PrimaryConstructorParameters_IsNull_ByDefault()
	{
		var s = new StructType { Name = "Foo" };

		Assert.That(s.PrimaryConstructorParameters, Is.Null);
	}

	[Test]
	public void StructType_Nodes_SetsParent()
	{
		var s = new StructType { Name = "Foo" };
		var field = new FieldNode { Type = TypeRef.Int, Name = "x" };

		s.Nodes.Add(field);

		Assert.That(field.Parent, Is.SameAs(s));
	}

	[Test]
	public async Task StructType_AcceptAsync_CallsVisitStructTypeAsync()
	{
		var s = new StructType { Name = "Foo" };
		var visitor = new Mock<CodeDomVisitor>();

		await s.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitStructTypeAsync(s, It.IsAny<CancellationToken>()), Times.Once);
	}
}
