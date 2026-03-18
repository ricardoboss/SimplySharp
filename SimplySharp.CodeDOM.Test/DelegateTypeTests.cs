using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class DelegateTypeTests
{
	[Test]
	public void DelegateType_ReturnType_IsRequired()
	{
		var del = new DelegateType { Name = "MyHandler", ReturnType = TypeRef.Void };

		Assert.That(del.ReturnType, Is.EqualTo(TypeRef.Void));
	}

	[Test]
	public void DelegateType_Parameters_IsEmpty_ByDefault()
	{
		var del = new DelegateType { Name = "MyHandler", ReturnType = TypeRef.Void };

		Assert.That(del.Parameters, Is.Empty);
	}

	[Test]
	public void DelegateType_Parameters_CanBePopulated()
	{
		var del = new DelegateType
		{
			Name = "MyHandler",
			ReturnType = TypeRef.Void,
			Parameters = [new Parameter(TypeRef.Object, "sender"), new Parameter(TypeRef.Int, "args")],
		};

		Assert.That(del.Parameters, Has.Count.EqualTo(2));
	}

	[Test]
	public async Task DelegateType_AcceptAsync_CallsVisitDelegateTypeAsync()
	{
		var del = new DelegateType { Name = "MyHandler", ReturnType = TypeRef.Void };
		var visitor = new Mock<CodeDomVisitor>();

		await del.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitDelegateTypeAsync(del, It.IsAny<CancellationToken>()), Times.Once);
	}
}
