using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class RecordTypeTests
{
	[Test]
	public void RecordType_Kind_DefaultsToClass()
	{
		var rec = new RecordType { Name = "Foo" };

		Assert.That(rec.Kind, Is.EqualTo(RecordKind.Class));
	}

	[Test]
	public void RecordType_Kind_CanBeStruct()
	{
		var rec = new RecordType { Name = "Foo", Kind = RecordKind.Struct };

		Assert.That(rec.Kind, Is.EqualTo(RecordKind.Struct));
	}

	[Test]
	public void RecordType_Extends_IsNull_ByDefault()
	{
		var rec = new RecordType { Name = "Foo" };

		Assert.That(rec.Extends, Is.Null);
	}

	[Test]
	public void RecordType_PrimaryConstructorParameters_CanBeSet()
	{
		var rec = new RecordType
		{
			Name = "Point",
			PrimaryConstructorParameters =
			[
				new Parameter(TypeRef.Double, "X"),
				new Parameter(TypeRef.Double, "Y"),
			],
		};

		Assert.That(rec.PrimaryConstructorParameters, Has.Count.EqualTo(2));
	}

	[Test]
	public void RecordType_Nodes_SetsParent()
	{
		var rec = new RecordType { Name = "Foo" };
		var prop = new PropertyNode { Type = TypeRef.String, Name = "Bar" };

		rec.Nodes.Add(prop);

		Assert.That(prop.Parent, Is.SameAs(rec));
	}

	[Test]
	public async Task RecordType_AcceptAsync_CallsVisitRecordTypeAsync()
	{
		var rec = new RecordType { Name = "Foo" };
		var visitor = new Mock<CodeDomVisitor>();

		await rec.AcceptAsync(visitor.Object);

		visitor.Verify(v => v.VisitRecordTypeAsync(rec, It.IsAny<CancellationToken>()), Times.Once);
	}
}
