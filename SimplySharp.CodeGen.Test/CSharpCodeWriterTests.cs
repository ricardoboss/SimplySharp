using SimplySharp.CodeDOM;
using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeGen.Test;

public class CSharpCodeWriterTests
{
	private static async Task<string> EmitAsync(Action<CodeWorkspace> setup)
	{
		var workspace = new CodeWorkspace();
		setup(workspace);
		var writer = new CSharpCodeWriter();
		await writer.VisitWorkspaceAsync(workspace);
		return writer.ToString();
	}

	private static async Task<string> EmitTypeAsync(CodeType type)
	{
		return await EmitAsync(ws =>
		{
			var ns = new CodeNamespace { Name = "Test" };
			ws.Namespaces.Add(ns);
			ns.Types.Add(type);
		});
	}

	[Test]
	public async Task EmitsNamespaceDeclaration()
	{
		var result = await EmitAsync(ws =>
		{
			var ns = new CodeNamespace { Name = "MyApp" };
			ws.Namespaces.Add(ns);
			ns.Types.Add(new ClassType { Name = "Foo" });
		});

		Assert.That(result, Does.StartWith("namespace MyApp;"));
	}

	[Test]
	public async Task EmitsNestedNamespaceWithFullName()
	{
		var result = await EmitAsync(ws =>
		{
			var parent = new CodeNamespace { Name = "MyApp" };
			ws.Namespaces.Add(parent);

			var child = new CodeNamespace { Name = "Models" };
			parent.Children.Add(child);
			child.Types.Add(new ClassType { Name = "Foo" });
		});

		Assert.That(result, Does.Contain("namespace MyApp.Models;"));
	}

	[Test]
	public async Task EmitsEmptyClass()
	{
		var result = await EmitTypeAsync(new ClassType { Name = "Empty" });

		Assert.That(result, Does.Contain("public class Empty"));
		Assert.That(result, Does.Contain("{"));
		Assert.That(result, Does.Contain("}"));
	}

	[Test]
	public async Task EmitsClassWithBaseAndInterfaces()
	{
		var cls = new ClassType
		{
			Name = "Derived",
			Extends = new NamedTypeRef("Base"),
			Implements = [new NamedTypeRef("IFoo"), new NamedTypeRef("IBar")],
		};

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public class Derived : Base, IFoo, IBar"));
	}

	[Test]
	public async Task EmitsAbstractSealedPartialClass()
	{
		var cls = new ClassType
		{
			Name = "Foo",
			IsAbstract = true,
			IsPartial = true,
		};

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public abstract partial class Foo"));
	}

	[Test]
	public async Task EmitsSealedClass()
	{
		var cls = new ClassType
		{
			Name = "Sealed",
			IsSealed = true,
		};

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public sealed class Sealed"));
	}

	[Test]
	public async Task EmitsClassWithPrimaryConstructor()
	{
		var cls = new ClassType
		{
			Name = "Point",
			PrimaryConstructorParameters =
			[
				new Parameter(TypeRef.Double, "x"),
				new Parameter(TypeRef.Double, "y"),
			],
		};

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public class Point(double x, double y)"));
	}

	[Test]
	public async Task EmitsGenericClassWithConstraints()
	{
		var cls = new ClassType
		{
			Name = "Repository",
			GenericParameters =
			[
				new GenericParameter
				{
					Name = "T",
					Constraints = [new ClassConstraint(), new TypeConstraint(new NamedTypeRef("IEntity")), new NewConstraint()],
				},
			],
		};

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public class Repository<T>"));
		Assert.That(result, Does.Contain("where T : class, IEntity, new()"));
	}

	[Test]
	public async Task EmitsMultipleGenericParameters()
	{
		var cls = new ClassType
		{
			Name = "Dict",
			GenericParameters =
			[
				new GenericParameter
				{
					Name = "TKey",
					Constraints = [new NotNullConstraint()],
				},
				new GenericParameter
				{
					Name = "TValue",
				},
			],
		};

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public class Dict<TKey, TValue>"));
		Assert.That(result, Does.Contain("where TKey : notnull"));
		Assert.That(result, Does.Not.Contain("where TValue"));
	}

	[Test]
	public async Task EmitsInterface()
	{
		var iface = new InterfaceType
		{
			Name = "IRepository",
			GenericParameters =
			[
				new GenericParameter
				{
					Name = "T",
					Variance = Variance.Out,
				},
			],
			Extends = [new NamedTypeRef("IDisposable", "System")],
		};
		iface.Nodes.Add(new MethodNode { ReturnType = TypeRef.Void, Name = "Save", IsAbstract = true });

		var result = await EmitTypeAsync(iface);

		Assert.That(result, Does.Contain("public interface IRepository<out T> : System.IDisposable"));
		Assert.That(result, Does.Contain("public abstract void Save();"));
	}

	[Test]
	public async Task EmitsStruct()
	{
		var s = new StructType
		{
			Name = "Point",
			Implements = [new NamedTypeRef("IEquatable", "System")],
		};
		s.Nodes.Add(new FieldNode { Type = TypeRef.Double, Name = "X" });
		s.Nodes.Add(new FieldNode { Type = TypeRef.Double, Name = "Y" });

		var result = await EmitTypeAsync(s);

		Assert.That(result, Does.Contain("public struct Point : System.IEquatable"));
		Assert.That(result, Does.Contain("public double X;"));
		Assert.That(result, Does.Contain("public double Y;"));
	}

	[Test]
	public async Task EmitsStructWithPrimaryConstructor()
	{
		var s = new StructType
		{
			Name = "Vector",
			PrimaryConstructorParameters =
			[
				new Parameter(TypeRef.Float, "x"),
				new Parameter(TypeRef.Float, "y"),
			],
		};

		var result = await EmitTypeAsync(s);

		Assert.That(result, Does.Contain("public struct Vector(float x, float y)"));
	}

	[Test]
	public async Task EmitsRecordClass()
	{
		var rec = new RecordType
		{
			Name = "Person",
			PrimaryConstructorParameters =
			[
				new Parameter(TypeRef.String, "Name"),
				new Parameter(TypeRef.Int, "Age"),
			],
		};

		var result = await EmitTypeAsync(rec);

		Assert.That(result, Does.Contain("public record Person(string Name, int Age);"));
	}

	[Test]
	public async Task EmitsRecordClassWithBody()
	{
		var rec = new RecordType
		{
			Name = "Person",
			PrimaryConstructorParameters =
			[
				new Parameter(TypeRef.String, "Name"),
			],
		};
		rec.Nodes.Add(new PropertyNode { Type = TypeRef.Int, Name = "Age" });

		var result = await EmitTypeAsync(rec);

		Assert.That(result, Does.Contain("public record Person(string Name)"));
		Assert.That(result, Does.Not.Contain("Person(string Name);"));
		Assert.That(result, Does.Contain("public int Age { get; set; }"));
	}

	[Test]
	public async Task EmitsRecordStruct()
	{
		var rec = new RecordType
		{
			Name = "Point",
			Kind = RecordKind.Struct,
			PrimaryConstructorParameters =
			[
				new Parameter(TypeRef.Double, "X"),
				new Parameter(TypeRef.Double, "Y"),
			],
		};

		var result = await EmitTypeAsync(rec);

		Assert.That(result, Does.Contain("public record struct Point(double X, double Y);"));
	}

	[Test]
	public async Task EmitsRecordWithBaseAndInterfaces()
	{
		var rec = new RecordType
		{
			Name = "Student",
			Extends = new NamedTypeRef("Person"),
			Implements = [new NamedTypeRef("IEquatable")],
			PrimaryConstructorParameters =
			[
				new Parameter(TypeRef.String, "School"),
			],
		};

		var result = await EmitTypeAsync(rec);

		Assert.That(result, Does.Contain("public record Student(string School) : Person, IEquatable;"));
	}

	[Test]
	public async Task EmitsRecordWithoutPrimaryConstructor()
	{
		var rec = new RecordType
		{
			Name = "Empty",
		};

		var result = await EmitTypeAsync(rec);

		Assert.That(result, Does.Contain("public record Empty"));
		Assert.That(result, Does.Contain("{"));
		Assert.That(result, Does.Contain("}"));
	}

	[Test]
	public async Task EmitsEnum()
	{
		var e = new EnumType { Name = "Color" };
		e.Members.Add(new EnumMemberNode { Name = "Red" });
		e.Members.Add(new EnumMemberNode { Name = "Green", Value = 1 });
		e.Members.Add(new EnumMemberNode { Name = "Blue", Value = 2 });

		var result = await EmitTypeAsync(e);

		Assert.That(result, Does.Contain("public enum Color"));
		Assert.That(result, Does.Contain("Red,"));
		Assert.That(result, Does.Contain("Green = 1,"));
		Assert.That(result, Does.Contain("Blue = 2"));
	}

	[Test]
	public async Task EmitsEnumWithUnderlyingType()
	{
		var e = new EnumType
		{
			Name = "Flags",
			UnderlyingType = TypeRef.Byte,
		};
		e.Members.Add(new EnumMemberNode { Name = "None", Value = 0 });

		var result = await EmitTypeAsync(e);

		Assert.That(result, Does.Contain("public enum Flags : byte"));
	}

	[Test]
	public async Task EmitsDelegate()
	{
		var del = new DelegateType
		{
			Name = "MyHandler",
			ReturnType = TypeRef.Void,
			Parameters = [new Parameter(TypeRef.Object, "sender"), new Parameter(new NamedTypeRef("EventArgs"), "e")],
		};

		var result = await EmitTypeAsync(del);

		Assert.That(result, Does.Contain("public delegate void MyHandler(object sender, EventArgs e);"));
	}

	[Test]
	public async Task EmitsGenericDelegate()
	{
		var del = new DelegateType
		{
			Name = "Converter",
			ReturnType = new NamedTypeRef("TOutput"),
			GenericParameters =
			[
				new GenericParameter { Name = "TInput", Variance = Variance.In },
				new GenericParameter { Name = "TOutput", Variance = Variance.Out },
			],
			Parameters = [new Parameter(new NamedTypeRef("TInput"), "input")],
		};

		var result = await EmitTypeAsync(del);

		Assert.That(result, Does.Contain("public delegate TOutput Converter<in TInput, out TOutput>(TInput input);"));
	}

	[Test]
	public async Task EmitsField()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "_count", AccessModifier = AccessModifier.Private });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("private int _count;"));
	}

	[Test]
	public async Task EmitsStaticReadonlyField()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode { Type = TypeRef.String, Name = "Default", IsStatic = true, IsReadonly = true });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static readonly string Default;"));
	}

	[Test]
	public async Task EmitsAutoProperty()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode { Type = TypeRef.String, Name = "Name" });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public string Name { get; set; }"));
	}

	[Test]
	public async Task EmitsGetOnlyProperty()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode { Type = TypeRef.Int, Name = "Count", HasSetter = false });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public int Count { get; }"));
	}

	[Test]
	public async Task EmitsInitProperty()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode { Type = TypeRef.String, Name = "Id", IsSetterInit = true });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public string Id { get; init; }"));
	}

	[Test]
	public async Task EmitsRequiredProperty()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode
		{
			Type = TypeRef.String,
			Name = "Name",
			IsRequired = true,
			IsSetterInit = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public required string Name { get; init; }"));
	}

	[Test]
	public async Task EmitsStaticProperty()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode { Type = TypeRef.Int, Name = "Instance", IsStatic = true, HasSetter = false });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static int Instance { get; }"));
	}

	[Test]
	public async Task EmitsSetOnlyProperty()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode { Type = TypeRef.String, Name = "Token", HasGetter = false });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public string Token { set; }"));
	}

	[Test]
	public async Task EmitsMethod()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = TypeRef.Void,
			Name = "DoWork",
			Parameters = [new Parameter(TypeRef.Int, "count")],
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public void DoWork(int count)"));
		Assert.That(result, Does.Contain("{"));
		Assert.That(result, Does.Contain("}"));
	}

	[Test]
	public async Task EmitsAbstractMethod()
	{
		var cls = new ClassType { Name = "Foo", IsAbstract = true };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = TypeRef.String,
			Name = "GetName",
			IsAbstract = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public abstract string GetName();"));
	}

	[Test]
	public async Task EmitsVirtualMethod()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = TypeRef.Void,
			Name = "OnClick",
			IsVirtual = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public virtual void OnClick()"));
	}

	[Test]
	public async Task EmitsOverrideMethod()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = TypeRef.String,
			Name = "ToString",
			IsOverride = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public override string ToString()"));
	}

	[Test]
	public async Task EmitsAsyncMethod()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = new GenericTypeRef("Task", "System.Threading.Tasks", [TypeRef.Int]),
			Name = "GetCountAsync",
			IsAsync = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public async System.Threading.Tasks.Task<int> GetCountAsync()"));
	}

	[Test]
	public async Task EmitsStaticMethod()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = TypeRef.Int,
			Name = "Create",
			IsStatic = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static int Create()"));
	}

	[Test]
	public async Task EmitsGenericMethod()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = new NamedTypeRef("T"),
			Name = "Parse",
			GenericParameters =
			[
				new GenericParameter
				{
					Name = "T",
					Constraints = [new StructConstraint()],
				},
			],
			Parameters = [new Parameter(TypeRef.String, "input")],
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public T Parse<T>(string input) where T : struct"));
	}

	[Test]
	public async Task EmitsConstructor()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new ConstructorNode
		{
			Parameters = [new Parameter(TypeRef.Int, "value")],
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public Foo(int value)"));
	}

	[Test]
	public async Task EmitsConstructorWithBaseInitializer()
	{
		var cls = new ClassType { Name = "Derived" };
		cls.Nodes.Add(new ConstructorNode
		{
			Initializer = ConstructorInitializerKind.Base,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public Derived() : base()"));
	}

	[Test]
	public async Task EmitsConstructorWithThisInitializer()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new ConstructorNode
		{
			Parameters = [new Parameter(TypeRef.String, "name")],
			Initializer = ConstructorInitializerKind.This,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public Foo(string name) : this()"));
	}

	[Test]
	public async Task EmitsStaticConstructor()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new ConstructorNode { IsStatic = true });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("static Foo()"));
		Assert.That(result, Does.Not.Contain("public static Foo()"));
	}

	[Test]
	public async Task EmitsEvent()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new EventNode
		{
			Type = new NamedTypeRef("EventHandler"),
			Name = "Click",
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public event EventHandler Click;"));
	}

	[Test]
	public async Task EmitsStaticEvent()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new EventNode
		{
			Type = new NamedTypeRef("EventHandler"),
			Name = "GlobalEvent",
			IsStatic = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static event EventHandler GlobalEvent;"));
	}

	[Test]
	public async Task EmitsIndexer()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new IndexerNode
		{
			ReturnType = TypeRef.String,
			Parameters = [new Parameter(TypeRef.Int, "index")],
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public string this[int index] { get; set; }"));
	}

	[Test]
	public async Task EmitsReadOnlyIndexer()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new IndexerNode
		{
			ReturnType = TypeRef.String,
			Parameters = [new Parameter(TypeRef.Int, "index")],
			HasSetter = false,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public string this[int index] { get; }"));
	}

	[Test]
	public async Task EmitsAdditionOperator()
	{
		var cls = new ClassType { Name = "Vec" };
		cls.Nodes.Add(new OperatorNode
		{
			Kind = OperatorKind.Addition,
			ReturnType = new NamedTypeRef("Vec"),
			Parameters = [new Parameter(new NamedTypeRef("Vec"), "a"), new Parameter(new NamedTypeRef("Vec"), "b")],
			IsStatic = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static Vec operator +(Vec a, Vec b)"));
	}

	[Test]
	public async Task EmitsImplicitConversionOperator()
	{
		var cls = new ClassType { Name = "Wrapper" };
		cls.Nodes.Add(new OperatorNode
		{
			Kind = OperatorKind.Implicit,
			ReturnType = new NamedTypeRef("Wrapper"),
			Parameters = [new Parameter(TypeRef.Int, "value")],
			IsStatic = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static implicit operator Wrapper(int value)"));
	}

	[Test]
	public async Task EmitsExplicitConversionOperator()
	{
		var cls = new ClassType { Name = "Wrapper" };
		cls.Nodes.Add(new OperatorNode
		{
			Kind = OperatorKind.Explicit,
			ReturnType = TypeRef.Int,
			Parameters = [new Parameter(new NamedTypeRef("Wrapper"), "w")],
			IsStatic = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static explicit operator int(Wrapper w)"));
	}

	[Test]
	public async Task EmitsEnumMembers()
	{
		var e = new EnumType { Name = "Status" };
		e.Members.Add(new EnumMemberNode { Name = "Active" });
		e.Members.Add(new EnumMemberNode { Name = "Inactive", Value = 5 });

		var result = await EmitTypeAsync(e);

		Assert.That(result, Does.Contain("Active,"));
		Assert.That(result, Does.Contain("Inactive = 5"));
	}

	[Test]
	public async Task EmitsSingleEnumMemberWithoutTrailingComma()
	{
		var e = new EnumType { Name = "Solo" };
		e.Members.Add(new EnumMemberNode { Name = "Only" });

		var result = await EmitTypeAsync(e);

		Assert.That(result, Does.Contain("Only"));
		Assert.That(result, Does.Not.Contain("Only,"));
	}

	[Test]
	public async Task EmitsParameterModifiers()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = TypeRef.Void,
			Name = "DoWork",
			Parameters =
			[
				new Parameter(TypeRef.Int, "x", ParameterModifier.Ref),
				new Parameter(TypeRef.Int, "y", ParameterModifier.Out),
				new Parameter(TypeRef.Int, "z", ParameterModifier.In),
				new Parameter(new ArrayTypeRef(TypeRef.String), "args", ParameterModifier.Params),
			],
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("ref int x"));
		Assert.That(result, Does.Contain("out int y"));
		Assert.That(result, Does.Contain("in int z"));
		Assert.That(result, Does.Contain("params string[] args"));
	}

	[Test]
	public async Task EmitsAllAccessModifiers()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "a", AccessModifier = AccessModifier.Public });
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "b", AccessModifier = AccessModifier.Protected });
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "c", AccessModifier = AccessModifier.Internal });
		cls.Nodes.Add(new FieldNode
		{
			Type = TypeRef.Int, Name = "d", AccessModifier = AccessModifier.ProtectedInternal,
		});
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "e", AccessModifier = AccessModifier.Private });
		cls.Nodes.Add(new FieldNode
		{
			Type = TypeRef.Int, Name = "f", AccessModifier = AccessModifier.PrivateProtected,
		});
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "g", AccessModifier = AccessModifier.File });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public int a;"));
		Assert.That(result, Does.Contain("protected int b;"));
		Assert.That(result, Does.Contain("internal int c;"));
		Assert.That(result, Does.Contain("protected internal int d;"));
		Assert.That(result, Does.Contain("private int e;"));
		Assert.That(result, Does.Contain("private protected int f;"));
		Assert.That(result, Does.Contain("file int g;"));
	}

	[Test]
	public async Task EmitsNullableTypeRef()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode { Type = new NullableTypeRef(TypeRef.Int), Name = "x" });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public int? x;"));
	}

	[Test]
	public async Task EmitsArrayTypeRef()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode { Type = new ArrayTypeRef(TypeRef.Int), Name = "items" });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public int[] items;"));
	}

	[Test]
	public async Task EmitsMultiDimensionalArrayTypeRef()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode { Type = new ArrayTypeRef(TypeRef.Int, 2), Name = "matrix" });

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public int[,] matrix;"));
	}

	[Test]
	public async Task EmitsGenericTypeRef()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode
		{
			Type = new GenericTypeRef("List", "System.Collections.Generic", [TypeRef.String]),
			Name = "items",
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public System.Collections.Generic.List<string> items;"));
	}

	[Test]
	public async Task EmitsTupleTypeRef()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode
		{
			Type = new TupleTypeRef([(TypeRef.Int, "X"), (TypeRef.Int, "Y")]),
			Name = "point",
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public (int X, int Y) point;"));
	}

	[Test]
	public async Task EmitsTupleTypeRefWithoutNames()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode
		{
			Type = new TupleTypeRef([(TypeRef.Int, null), (TypeRef.String, null)]),
			Name = "pair",
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public (int, string) pair;"));
	}

	[Test]
	public async Task EmitsAllGenericConstraintTypes()
	{
		var cls = new ClassType
		{
			Name = "Foo",
			GenericParameters =
			[
				new GenericParameter { Name = "A", Constraints = [new ClassConstraint()] },
				new GenericParameter { Name = "B", Constraints = [new ClassConstraint(true)] },
				new GenericParameter { Name = "C", Constraints = [new StructConstraint()] },
				new GenericParameter { Name = "D", Constraints = [new UnmanagedConstraint()] },
				new GenericParameter { Name = "E", Constraints = [new NotNullConstraint()] },
				new GenericParameter { Name = "F", Constraints = [new DefaultConstraint()] },
				new GenericParameter { Name = "G", Constraints = [new NewConstraint()] },
				new GenericParameter
				{
					Name = "H", Constraints = [new TypeConstraint(new NamedTypeRef("IDisposable", "System"))],
				},
			],
		};

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("where A : class"));
		Assert.That(result, Does.Contain("where B : class?"));
		Assert.That(result, Does.Contain("where C : struct"));
		Assert.That(result, Does.Contain("where D : unmanaged"));
		Assert.That(result, Does.Contain("where E : notnull"));
		Assert.That(result, Does.Contain("where F : default"));
		Assert.That(result, Does.Contain("where G : new()"));
		Assert.That(result, Does.Contain("where H : System.IDisposable"));
	}

	[Test]
	public async Task EmitsAllOperatorKinds()
	{
		var operators = new (OperatorKind Kind, string Token)[]
		{
			(OperatorKind.Addition, "+"),
			(OperatorKind.Subtraction, "-"),
			(OperatorKind.Multiply, "*"),
			(OperatorKind.Division, "/"),
			(OperatorKind.Modulus, "%"),
			(OperatorKind.UnaryPlus, "+"),
			(OperatorKind.UnaryNegation, "-"),
			(OperatorKind.Increment, "++"),
			(OperatorKind.Decrement, "--"),
			(OperatorKind.LessThan, "<"),
			(OperatorKind.GreaterThan, ">"),
			(OperatorKind.LessThanOrEqual, "<="),
			(OperatorKind.GreaterThanOrEqual, ">="),
			(OperatorKind.Equality, "=="),
			(OperatorKind.Inequality, "!="),
			(OperatorKind.LogicalNot, "!"),
			(OperatorKind.BitwiseAnd, "&"),
			(OperatorKind.BitwiseOr, "|"),
			(OperatorKind.ExclusiveOr, "^"),
			(OperatorKind.BitwiseNot, "~"),
			(OperatorKind.True, "true"),
			(OperatorKind.False, "false"),
		};

		foreach (var (kind, token) in operators)
		{
			var cls = new ClassType { Name = "Op" };
			cls.Nodes.Add(new OperatorNode
			{
				Kind = kind,
				ReturnType = new NamedTypeRef("Op"),
				Parameters = [new Parameter(new NamedTypeRef("Op"), "a")],
				IsStatic = true,
			});

			var result = await EmitTypeAsync(cls);

			Assert.That(result, Does.Contain($"operator {token}"),
				$"Expected operator token '{token}' for kind {kind}");
		}
	}

	[Test]
	public async Task EmitsMultipleNamespaces()
	{
		var result = await EmitAsync(ws =>
		{
			var ns1 = new CodeNamespace { Name = "First" };
			ws.Namespaces.Add(ns1);
			ns1.Types.Add(new ClassType { Name = "A" });

			var ns2 = new CodeNamespace { Name = "Second" };
			ws.Namespaces.Add(ns2);
			ns2.Types.Add(new ClassType { Name = "B" });
		});

		Assert.That(result, Does.Contain("namespace First;"));
		Assert.That(result, Does.Contain("namespace Second;"));
		Assert.That(result, Does.Contain("public class A"));
		Assert.That(result, Does.Contain("public class B"));
	}

	[Test]
	public async Task EmitsMultipleTypesInNamespace()
	{
		var result = await EmitAsync(ws =>
		{
			var ns = new CodeNamespace { Name = "Test" };
			ws.Namespaces.Add(ns);
			ns.Types.Add(new ClassType { Name = "A" });
			ns.Types.Add(new ClassType { Name = "B" });
		});

		Assert.That(result, Does.Contain("public class A"));
		Assert.That(result, Does.Contain("public class B"));
	}

	[Test]
	public async Task EmitsMultipleMembersWithBlankLineSeparation()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "x" });
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "y" });

		var result = await EmitTypeAsync(cls);

		var lines = result.Split('\n');
		var xLine = Array.FindIndex(lines, l => l.Contains("public int x;"));
		var yLine = Array.FindIndex(lines, l => l.Contains("public int y;"));

		Assert.That(xLine, Is.GreaterThan(-1));
		Assert.That(yLine, Is.GreaterThan(xLine + 1), "Expected a blank line between members");
	}

	[Test]
	public async Task ToStringReturnsOutput()
	{
		var writer = new CSharpCodeWriter();
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "Test" };
		workspace.Namespaces.Add(ns);
		ns.Types.Add(new ClassType { Name = "Foo" });

		await writer.VisitWorkspaceAsync(workspace);

		var result = writer.ToString();
		Assert.That(result, Does.Contain("namespace Test;"));
		Assert.That(result, Does.Contain("public class Foo"));
	}

	[Test]
	public async Task IndentStyleCustomization()
	{
		var settings = new CodeWriteSettings { IndentStyle = "    " };
		var writer = new CSharpCodeWriter(settings);
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "Test" };
		workspace.Namespaces.Add(ns);
		var cls = new ClassType { Name = "Foo" };
		ns.Types.Add(cls);
		cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "x" });

		await writer.VisitWorkspaceAsync(workspace);

		var result = writer.ToString();
		Assert.That(result, Does.Contain("    public int x;"));
	}

	[Test]
	public async Task CustomLineEndings_UsedInOutput()
	{
		var settings = new CodeWriteSettings { LineEnding = "\r\n" };
		var writer = new CSharpCodeWriter(settings);
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "Test" };
		workspace.Namespaces.Add(ns);
		ns.Types.Add(new ClassType { Name = "Foo" });

		await writer.VisitWorkspaceAsync(workspace);

		var result = writer.ToString();
		Assert.That(result, Does.Contain("\r\n"));
		Assert.That(result, Does.Not.Contain("\n\n").And.Not.Contain("\r\r"));
	}

	[Test]
	public async Task FinalNewline_IsAppended()
	{
		var result = await EmitTypeAsync(new ClassType { Name = "Foo" });

		Assert.That(result, Does.EndWith("\n"));
	}

	[Test]
	public async Task NoFinalNewline_WhenDisabled()
	{
		var settings = new CodeWriteSettings { InsertFinalNewline = false };
		var writer = new CSharpCodeWriter(settings);
		var workspace = new CodeWorkspace();
		var ns = new CodeNamespace { Name = "Test" };
		workspace.Namespaces.Add(ns);
		ns.Types.Add(new ClassType { Name = "Foo" });

		await writer.VisitWorkspaceAsync(workspace);

		var result = writer.ToString();
		// Content ends with "}\n" from the last WriteLine("}") — no extra newline appended
		Assert.That(result, Does.EndWith("}\n"));
		Assert.That(result, Does.Not.EndWith("}\n\n"));
	}

	[Test]
	public async Task EmitsInterfaceWithContravariantParameter()
	{
		var iface = new InterfaceType
		{
			Name = "IComparer",
			GenericParameters =
			[
				new GenericParameter { Name = "T", Variance = Variance.In },
			],
		};

		var result = await EmitTypeAsync(iface);

		Assert.That(result, Does.Contain("public interface IComparer<in T>"));
	}

	[Test]
	public async Task EmitsDelegateWithConstraints()
	{
		var del = new DelegateType
		{
			Name = "Factory",
			ReturnType = new NamedTypeRef("T"),
			GenericParameters =
			[
				new GenericParameter
				{
					Name = "T",
					Constraints = [new ClassConstraint(), new NewConstraint()],
				},
			],
		};

		var result = await EmitTypeAsync(del);

		Assert.That(result, Does.Contain("public delegate T Factory<T>() where T : class, new();"));
	}

	[Test]
	public async Task EmitsMethodWithRefOutInParams()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new MethodNode
		{
			ReturnType = TypeRef.Bool,
			Name = "TryParse",
			Parameters =
			[
				new Parameter(TypeRef.String, "input"),
				new Parameter(TypeRef.Int, "result", ParameterModifier.Out),
			],
			IsStatic = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static bool TryParse(string input, out int result)"));
	}

	[Test]
	public async Task EmitsClassOnlyInterfaces()
	{
		var cls = new ClassType
		{
			Name = "Foo",
			Implements = [new NamedTypeRef("IDisposable")],
		};

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public class Foo : IDisposable"));
	}

	[Test]
	public async Task EmitsStaticIndexer()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new IndexerNode
		{
			ReturnType = TypeRef.String,
			Parameters = [new Parameter(TypeRef.Int, "i")],
			IsStatic = true,
		});

		var result = await EmitTypeAsync(cls);

		Assert.That(result, Does.Contain("public static string this[int i] { get; set; }"));
	}

	[Test]
	public async Task EmitsGenericClassWithVariance()
	{
		var iface = new InterfaceType
		{
			Name = "ICovariant",
			GenericParameters =
			[
				new GenericParameter { Name = "T", Variance = Variance.None },
			],
		};

		var result = await EmitTypeAsync(iface);

		Assert.That(result, Does.Contain("public interface ICovariant<T>"));
		Assert.That(result, Does.Not.Contain("in T"));
		Assert.That(result, Does.Not.Contain("out T"));
	}

	[Test]
	public async Task EmitsConstructorInStruct()
	{
		var s = new StructType { Name = "Point" };
		s.Nodes.Add(new ConstructorNode
		{
			Parameters = [new Parameter(TypeRef.Int, "x"), new Parameter(TypeRef.Int, "y")],
		});

		var result = await EmitTypeAsync(s);

		Assert.That(result, Does.Contain("public Point(int x, int y)"));
	}

	[Test]
	public async Task EmitsEnumStringValue()
	{
		var e = new EnumType { Name = "Tags" };
		e.Members.Add(new EnumMemberNode { Name = "Label", Value = "hello" });

		var result = await EmitTypeAsync(e);

		Assert.That(result, Does.Contain("Label = \"hello\""));
	}

	[Test]
	public async Task EmitsEnumCharValue()
	{
		var e = new EnumType { Name = "Markers" };
		e.Members.Add(new EnumMemberNode { Name = "Star", Value = '*' });

		var result = await EmitTypeAsync(e);

		Assert.That(result, Does.Contain("Star = '*'"));
	}

	// --- Language version tests ---

	private static async Task<string> EmitWithVersionAsync(int languageVersion, Action<CodeWorkspace> setup)
	{
		var workspace = new CodeWorkspace();
		setup(workspace);
		var writer = new CSharpCodeWriter(new CodeWriteSettings { LanguageVersion = languageVersion });
		await writer.VisitWorkspaceAsync(workspace);
		return writer.ToString();
	}

	private static async Task<string> EmitTypeWithVersionAsync(int languageVersion, CodeType type)
	{
		return await EmitWithVersionAsync(languageVersion, ws =>
		{
			var ns = new CodeNamespace { Name = "Test" };
			ws.Namespaces.Add(ns);
			ns.Types.Add(type);
		});
	}

	[Test]
	public async Task EmitsFileScopedNamespace_WhenTargetingCSharp10()
	{
		var result = await EmitWithVersionAsync(CSharpLanguageVersion.CSharp10, ws =>
		{
			var ns = new CodeNamespace { Name = "MyApp" };
			ws.Namespaces.Add(ns);
			ns.Types.Add(new ClassType { Name = "Foo" });
		});

		Assert.That(result, Does.Contain("namespace MyApp;"));
	}

	[Test]
	public async Task EmitsBlockScopedNamespace_WhenTargetingCSharp9()
	{
		var result = await EmitWithVersionAsync(CSharpLanguageVersion.CSharp9, ws =>
		{
			var ns = new CodeNamespace { Name = "MyApp" };
			ws.Namespaces.Add(ns);
			ns.Types.Add(new ClassType { Name = "Foo" });
		});

		Assert.That(result, Does.Contain("namespace MyApp"));
		Assert.That(result, Does.Not.Contain("namespace MyApp;"));
		Assert.That(result, Does.Contain("{"));
		Assert.That(result, Does.Contain("}"));
	}

	[Test]
	public async Task BlockScopedNamespace_IndentsTypeMembers()
	{
		var result = await EmitWithVersionAsync(CSharpLanguageVersion.CSharp9, ws =>
		{
			var ns = new CodeNamespace { Name = "MyApp" };
			ws.Namespaces.Add(ns);
			var cls = new ClassType { Name = "Foo" };
			ns.Types.Add(cls);
			cls.Nodes.Add(new FieldNode { Type = TypeRef.Int, Name = "x" });
		});

		// Block-scoped namespace: namespace body indented once, class body indented twice
		Assert.That(result, Does.Contain("\t\tpublic int x;"));
	}

	[Test]
	public async Task BlockScopedNamespace_HandlesNestedNamespaces()
	{
		var result = await EmitWithVersionAsync(CSharpLanguageVersion.CSharp9, ws =>
		{
			var parent = new CodeNamespace { Name = "MyApp" };
			ws.Namespaces.Add(parent);

			var child = new CodeNamespace { Name = "Models" };
			parent.Children.Add(child);
			child.Types.Add(new ClassType { Name = "Foo" });
		});

		Assert.That(result, Does.Contain("namespace MyApp.Models"));
		Assert.That(result, Does.Not.Contain("namespace MyApp.Models;"));
	}

	[Test]
	public void Throws_WhenRecordTargetsCSharp8()
	{
		var rec = new RecordType { Name = "Person" };

		Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp8, rec));
	}

	[Test]
	public async Task AllowsRecord_WhenTargetingCSharp9()
	{
		var rec = new RecordType { Name = "Person" };

		var result = await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp9, rec);

		Assert.That(result, Does.Contain("public record Person"));
	}

	[Test]
	public void Throws_WhenRecordStructTargetsCSharp9()
	{
		var rec = new RecordType { Name = "Point", Kind = RecordKind.Struct };

		Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp9, rec));
	}

	[Test]
	public async Task AllowsRecordStruct_WhenTargetingCSharp10()
	{
		var rec = new RecordType
		{
			Name = "Point",
			Kind = RecordKind.Struct,
			PrimaryConstructorParameters = [new Parameter(TypeRef.Int, "X")],
		};

		var result = await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp10, rec);

		Assert.That(result, Does.Contain("public record struct Point(int X);"));
	}

	[Test]
	public void Throws_WhenFileAccessModifierTargetsCSharp9()
	{
		var cls = new ClassType { Name = "Foo", AccessModifier = AccessModifier.File };

		Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp9, cls));
	}

	[Test]
	public async Task AllowsFileAccessModifier_WhenTargetingCSharp10()
	{
		var cls = new ClassType { Name = "Foo", AccessModifier = AccessModifier.File };

		var result = await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp10, cls);

		Assert.That(result, Does.Contain("file class Foo"));
	}

	[Test]
	public void Throws_WhenClassPrimaryConstructorTargetsCSharp11()
	{
		var cls = new ClassType
		{
			Name = "Point",
			PrimaryConstructorParameters = [new Parameter(TypeRef.Int, "x")],
		};

		Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp11, cls));
	}

	[Test]
	public async Task AllowsClassPrimaryConstructor_WhenTargetingCSharp12()
	{
		var cls = new ClassType
		{
			Name = "Point",
			PrimaryConstructorParameters = [new Parameter(TypeRef.Int, "x")],
		};

		var result = await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp12, cls);

		Assert.That(result, Does.Contain("public class Point(int x)"));
	}

	[Test]
	public void Throws_WhenStructPrimaryConstructorTargetsCSharp11()
	{
		var s = new StructType
		{
			Name = "Vector",
			PrimaryConstructorParameters = [new Parameter(TypeRef.Float, "x")],
		};

		Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp11, s));
	}

	[Test]
	public async Task AllowsStructPrimaryConstructor_WhenTargetingCSharp12()
	{
		var s = new StructType
		{
			Name = "Vector",
			PrimaryConstructorParameters = [new Parameter(TypeRef.Float, "x")],
		};

		var result = await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp12, s);

		Assert.That(result, Does.Contain("public struct Vector(float x)"));
	}

	[Test]
	public void Throws_WhenRequiredPropertyTargetsCSharp10()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode
		{
			Type = TypeRef.String,
			Name = "Name",
			IsRequired = true,
			IsSetterInit = true,
		});

		Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp10, cls));
	}

	[Test]
	public async Task AllowsRequiredProperty_WhenTargetingCSharp11()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode
		{
			Type = TypeRef.String,
			Name = "Name",
			IsRequired = true,
			IsSetterInit = true,
		});

		var result = await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp11, cls);

		Assert.That(result, Does.Contain("public required string Name { get; init; }"));
	}

	[Test]
	public void Throws_WhenInitPropertyTargetsCSharp8()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode
		{
			Type = TypeRef.String,
			Name = "Id",
			IsSetterInit = true,
		});

		Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp8, cls));
	}

	[Test]
	public async Task AllowsInitProperty_WhenTargetingCSharp9()
	{
		var cls = new ClassType { Name = "Foo" };
		cls.Nodes.Add(new PropertyNode
		{
			Type = TypeRef.String,
			Name = "Id",
			IsSetterInit = true,
		});

		var result = await EmitTypeWithVersionAsync(CSharpLanguageVersion.CSharp9, cls);

		Assert.That(result, Does.Contain("public string Id { get; init; }"));
	}

	[Test]
	public async Task DefaultSettings_UsesLatestLanguageVersion()
	{
		// Default settings emit file-scoped namespaces (C# 10+)
		var result = await EmitAsync(ws =>
		{
			var ns = new CodeNamespace { Name = "Test" };
			ws.Namespaces.Add(ns);
			ns.Types.Add(new ClassType { Name = "Foo" });
		});

		Assert.That(result, Does.Contain("namespace Test;"));
	}

	[Test]
	public async Task BlockScopedNamespace_MultipleTypes()
	{
		var result = await EmitWithVersionAsync(CSharpLanguageVersion.CSharp9, ws =>
		{
			var ns = new CodeNamespace { Name = "Test" };
			ws.Namespaces.Add(ns);
			ns.Types.Add(new ClassType { Name = "A" });
			ns.Types.Add(new ClassType { Name = "B" });
		});

		Assert.That(result, Does.Contain("namespace Test"));
		Assert.That(result, Does.Contain("public class A"));
		Assert.That(result, Does.Contain("public class B"));
	}
}
