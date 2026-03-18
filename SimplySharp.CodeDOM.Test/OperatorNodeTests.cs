using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class OperatorNodeTests
{
	[Test]
	public void OperatorNode_Extends_MemberNode()
	{
		var op = new OperatorNode { Kind = OperatorKind.Addition, ReturnType = TypeRef.Int };

		Assert.That(op, Is.InstanceOf<MemberNode>());
	}

	[Test]
	public void OperatorNode_Parameters_IsEmpty_ByDefault()
	{
		var op = new OperatorNode { Kind = OperatorKind.Addition, ReturnType = TypeRef.Int };

		Assert.That(op.Parameters, Is.Empty);
	}

	[Test]
	public void OperatorNode_BinaryOperator_WithParameters()
	{
		var op = new OperatorNode
		{
			Kind = OperatorKind.Addition,
			ReturnType = TypeRef.Int,
			Parameters =
			[
				new Parameter(TypeRef.Int, "left"),
				new Parameter(TypeRef.Int, "right"),
			],
		};

		Assert.Multiple(() =>
		{
			Assert.That(op.Kind, Is.EqualTo(OperatorKind.Addition));
			Assert.That(op.Parameters, Has.Count.EqualTo(2));
		});
	}

	[Test]
	public void OperatorNode_ConversionOperator()
	{
		var op = new OperatorNode
		{
			Kind = OperatorKind.Implicit,
			ReturnType = TypeRef.String,
			Parameters = [new Parameter(TypeRef.Int, "value")],
		};

		Assert.That(op.Kind, Is.EqualTo(OperatorKind.Implicit));
	}

	[Test]
	public async Task OperatorNode_AcceptAsync_CallsVisitOperatorAsync()
	{
		var op = new OperatorNode { Kind = OperatorKind.Addition, ReturnType = TypeRef.Int };
		var visitor = new Mock<CodeDomVisitor>();

		await op.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitOperatorAsync(op, It.IsAny<CancellationToken>()), Times.Once);
	}
}
