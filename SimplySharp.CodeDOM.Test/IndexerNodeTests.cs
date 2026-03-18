using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class IndexerNodeTests
{
	[Test]
	public void IndexerNode_Extends_MemberNode()
	{
		var indexer = new IndexerNode { ReturnType = TypeRef.String };

		Assert.That(indexer, Is.InstanceOf<MemberNode>());
	}

	[Test]
	public void IndexerNode_Parameters_IsEmpty_ByDefault()
	{
		var indexer = new IndexerNode { ReturnType = TypeRef.String };

		Assert.That(indexer.Parameters, Is.Empty);
	}

	[Test]
	public void IndexerNode_HasGetter_DefaultsTrue()
	{
		var indexer = new IndexerNode { ReturnType = TypeRef.String };

		Assert.That(indexer.HasGetter, Is.True);
	}

	[Test]
	public void IndexerNode_HasSetter_DefaultsTrue()
	{
		var indexer = new IndexerNode { ReturnType = TypeRef.String };

		Assert.That(indexer.HasSetter, Is.True);
	}

	[Test]
	public void IndexerNode_Parameters_CanBePopulated()
	{
		var indexer = new IndexerNode
		{
			ReturnType = TypeRef.String,
			Parameters = [new Parameter(TypeRef.Int, "index")],
		};

		Assert.That(indexer.Parameters, Has.Count.EqualTo(1));
	}

	[Test]
	public async Task IndexerNode_AcceptAsync_CallsVisitIndexerAsync()
	{
		var indexer = new IndexerNode { ReturnType = TypeRef.String };
		var visitor = new Mock<CodeDomVisitor>();

		await indexer.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitIndexerAsync(indexer, It.IsAny<CancellationToken>()), Times.Once);
	}
}
