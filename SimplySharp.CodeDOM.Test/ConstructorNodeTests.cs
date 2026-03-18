using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class ConstructorNodeTests
{
	[Test]
	public void ConstructorNode_Extends_MemberNode()
	{
		var ctor = new ConstructorNode();

		Assert.That(ctor, Is.InstanceOf<MemberNode>());
	}

	[Test]
	public void ConstructorNode_Parameters_IsEmpty_ByDefault()
	{
		var ctor = new ConstructorNode();

		Assert.That(ctor.Parameters, Is.Empty);
	}

	[Test]
	public void ConstructorNode_Initializer_IsNull_ByDefault()
	{
		var ctor = new ConstructorNode();

		Assert.That(ctor.Initializer, Is.Null);
	}

	[Test]
	public void ConstructorNode_Initializer_CanBeBase()
	{
		var ctor = new ConstructorNode { Initializer = ConstructorInitializerKind.Base };

		Assert.That(ctor.Initializer, Is.EqualTo(ConstructorInitializerKind.Base));
	}

	[Test]
	public void ConstructorNode_Initializer_CanBeThis()
	{
		var ctor = new ConstructorNode { Initializer = ConstructorInitializerKind.This };

		Assert.That(ctor.Initializer, Is.EqualTo(ConstructorInitializerKind.This));
	}

	[Test]
	public void ConstructorNode_Parameters_CanBePopulated()
	{
		var ctor = new ConstructorNode
		{
			Parameters = [new Parameter(TypeRef.String, "name")],
		};

		Assert.That(ctor.Parameters, Has.Count.EqualTo(1));
	}

	[Test]
	public async Task ConstructorNode_AcceptAsync_CallsVisitConstructorAsync()
	{
		var ctor = new ConstructorNode();
		var visitor = new Mock<CodeDomVisitor>();

		await ctor.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitConstructorAsync(ctor, It.IsAny<CancellationToken>()), Times.Once);
	}
}
