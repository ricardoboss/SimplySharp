using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class VisitorDispatchTests
{
	[Test]
	public async Task Visitor_TraversesClassType_WithMembers()
	{
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "MyApp" };
		workspace.Namespaces.Add(ns);
		var cls = new ClassType { Name = "Foo" };
		ns.Types.Add(cls);
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "x" });
		cls.Nodes.Add(new PropertyNode { Type = TypeRef.String, Name = "Bar" });
		cls.Nodes.Add(new MethodNode { ReturnType = TypeRef.Void, Name = "DoStuff" });
		cls.Nodes.Add(new ConstructorNode());
		cls.Nodes.Add(new EventNode { Type = new NamedTypeRef("EventHandler"), Name = "Click" });
		cls.Nodes.Add(new IndexerNode { ReturnType = TypeRef.String });
		cls.Nodes.Add(new OperatorNode { Kind = OperatorKind.Addition, ReturnType = TypeRef.Int });

		var visitor = new Mock<CodeDomVisitor> { CallBase = true };

		await visitor.Object.VisitWorkspaceAsync(workspace).ConfigureAwait(false);

		visitor.Verify(v => v.VisitNamespaceAsync(ns, It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitClassTypeAsync(cls, It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitFieldAsync(It.IsAny<FieldNode>(), It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitPropertyAsync(It.IsAny<PropertyNode>(), It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitMethodAsync(It.IsAny<MethodNode>(), It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitConstructorAsync(It.IsAny<ConstructorNode>(), It.IsAny<CancellationToken>()),
			Times.Once);
		visitor.Verify(v => v.VisitEventAsync(It.IsAny<EventNode>(), It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitIndexerAsync(It.IsAny<IndexerNode>(), It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitOperatorAsync(It.IsAny<OperatorNode>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Visitor_TraversesInterfaceType()
	{
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "MyApp" };
		workspace.Namespaces.Add(ns);
		var iface = new InterfaceType { Name = "IFoo" };
		ns.Types.Add(iface);
		iface.Nodes.Add(new MethodNode { ReturnType = TypeRef.Void, Name = "DoStuff" });

		var visitor = new Mock<CodeDomVisitor> { CallBase = true };

		await visitor.Object.VisitWorkspaceAsync(workspace).ConfigureAwait(false);

		visitor.Verify(v => v.VisitInterfaceTypeAsync(iface, It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitMethodAsync(It.IsAny<MethodNode>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Visitor_TraversesStructType()
	{
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "MyApp" };
		workspace.Namespaces.Add(ns);
		var s = new StructType { Name = "Point" };
		ns.Types.Add(s);
		s.Nodes.Add(new FieldNode { Type = TypeRef.Double, Name = "X" });

		var visitor = new Mock<CodeDomVisitor> { CallBase = true };

		await visitor.Object.VisitWorkspaceAsync(workspace).ConfigureAwait(false);

		visitor.Verify(v => v.VisitStructTypeAsync(s, It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitFieldAsync(It.IsAny<FieldNode>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Visitor_TraversesRecordType()
	{
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "MyApp" };
		workspace.Namespaces.Add(ns);
		var rec = new RecordType { Name = "Person" };
		ns.Types.Add(rec);
		rec.Nodes.Add(new PropertyNode { Type = TypeRef.String, Name = "Name" });

		var visitor = new Mock<CodeDomVisitor> { CallBase = true };

		await visitor.Object.VisitWorkspaceAsync(workspace).ConfigureAwait(false);

		visitor.Verify(v => v.VisitRecordTypeAsync(rec, It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitPropertyAsync(It.IsAny<PropertyNode>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Visitor_TraversesEnumType_WithMembers()
	{
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "MyApp" };
		workspace.Namespaces.Add(ns);
		var e = new EnumType { Name = "Color" };
		ns.Types.Add(e);
		e.Members.Add(new EnumMemberNode { Name = "Red" });
		e.Members.Add(new EnumMemberNode { Name = "Green", Value = 1 });
		e.Members.Add(new EnumMemberNode { Name = "Blue", Value = 2 });

		var visitor = new Mock<CodeDomVisitor> { CallBase = true };

		await visitor.Object.VisitWorkspaceAsync(workspace).ConfigureAwait(false);

		visitor.Verify(v => v.VisitEnumTypeAsync(e, It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitEnumMemberAsync(It.IsAny<EnumMemberNode>(), It.IsAny<CancellationToken>()),
			Times.Exactly(3));
	}

	[Test]
	public async Task Visitor_TraversesDelegateType()
	{
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "MyApp" };
		workspace.Namespaces.Add(ns);
		var del = new DelegateType { Name = "MyHandler", ReturnType = TypeRef.Void };
		ns.Types.Add(del);

		var visitor = new Mock<CodeDomVisitor> { CallBase = true };

		await visitor.Object.VisitWorkspaceAsync(workspace).ConfigureAwait(false);

		visitor.Verify(v => v.VisitDelegateTypeAsync(del, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Visitor_TraversesNestedNamespaces()
	{
		var workspace = new CodeWorkspace();
		var parentNs = new CodeNamespace { Name = "MyApp" };
		workspace.Namespaces.Add(parentNs);
		var parentClass = new ClassType { Name = "Foo" };
		parentNs.Types.Add(parentClass);

		var childNs = new CodeNamespace { Name = "Models" };
		parentNs.Children.Add(childNs);
		var childClass = new ClassType { Name = "Bar" };
		childNs.Types.Add(childClass);

		var visitor = new Mock<CodeDomVisitor> { CallBase = true };

		await visitor.Object.VisitWorkspaceAsync(workspace).ConfigureAwait(false);

		visitor.Verify(v => v.VisitNamespaceAsync(It.IsAny<CodeNamespace>(), It.IsAny<CancellationToken>()),
			Times.Exactly(2));
		visitor.Verify(v => v.VisitClassTypeAsync(parentClass, It.IsAny<CancellationToken>()), Times.Once);
		visitor.Verify(v => v.VisitClassTypeAsync(childClass, It.IsAny<CancellationToken>()), Times.Once);
	}
}
