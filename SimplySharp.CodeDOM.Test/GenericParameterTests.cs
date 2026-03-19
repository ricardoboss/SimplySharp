using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class GenericParameterTests
{
	[Test]
	public void GenericParameter_Defaults_VarianceNone_ConstraintsEmpty()
	{
		var param = new GenericParameter { Name = "T" };

		Assert.Multiple(() =>
		{
			Assert.That(param.Name, Is.EqualTo("T"));
			Assert.That(param.Variance, Is.EqualTo(Variance.None));
			Assert.That(param.Constraints, Is.Empty);
		});
	}

	[Test]
	public void GenericParameter_Constraints_CanBePopulated()
	{
		var param = new GenericParameter
		{
			Name = "T",
			Constraints = [new ClassConstraint(), new TypeConstraint(TypeRef.Object), new NewConstraint()],
		};

		Assert.That(param.Constraints, Has.Count.EqualTo(3));
	}

	[Test]
	public void TypeConstraint_Holds_TypeRef()
	{
		var constraint = new TypeConstraint(TypeRef.String);

		Assert.That(constraint.Type, Is.EqualTo(TypeRef.String));
	}

	[Test]
	public void ClassConstraint_IsNullable_DefaultsFalse()
	{
		var constraint = new ClassConstraint();

		Assert.That(constraint.IsNullable, Is.False);
	}

	[Test]
	public void ClassConstraint_IsNullable_CanBeTrue()
	{
		var constraint = new ClassConstraint(IsNullable: true);

		Assert.That(constraint.IsNullable, Is.True);
	}

	[Test]
	public void AllConstraintSubtypes_AreGenericConstraints()
	{
		GenericConstraint[] constraints =
		[
			new ClassConstraint(),
			new StructConstraint(),
			new UnmanagedConstraint(),
			new NotNullConstraint(),
			new NewConstraint(),
			new DefaultConstraint(),
			new TypeConstraint(TypeRef.Int),
		];

		Assert.That(constraints, Has.Length.EqualTo(7));
	}

	[Test]
	public void ClassType_GenericParameters_IsEmpty_ByDefault()
	{
		var cls = new ClassType { Name = "Foo" };

		Assert.That(cls.GenericParameters, Is.Empty);
	}

	[Test]
	public void ClassType_GenericParameters_CanBePopulated()
	{
		var cls = new ClassType
		{
			Name = "Foo",
			GenericParameters =
			[
				new GenericParameter
				{
					Name = "T",
					Constraints = [new ClassConstraint(), new TypeConstraint(new NamedTypeRef("IDisposable", "System"))],
				},
			],
		};

		Assert.That(cls.GenericParameters, Has.Count.EqualTo(1));
		Assert.That(cls.GenericParameters[0].Constraints, Has.Count.EqualTo(2));
	}

	[Test]
	public void InterfaceType_GenericParameters_SupportsVariance()
	{
		var iface = new InterfaceType
		{
			Name = "IReadable",
			GenericParameters = [new GenericParameter { Name = "T", Variance = Variance.Out }],
		};

		Assert.That(iface.GenericParameters[0].Variance, Is.EqualTo(Variance.Out));
	}

	[Test]
	public void DelegateType_GenericParameters_SupportsVariance()
	{
		var del = new DelegateType
		{
			Name = "MyFunc",
			ReturnType = TypeRef.Void,
			GenericParameters =
			[
				new GenericParameter { Name = "TIn", Variance = Variance.In },
				new GenericParameter { Name = "TOut", Variance = Variance.Out },
			],
		};

		Assert.Multiple(() =>
		{
			Assert.That(del.GenericParameters, Has.Count.EqualTo(2));
			Assert.That(del.GenericParameters[0].Variance, Is.EqualTo(Variance.In));
			Assert.That(del.GenericParameters[1].Variance, Is.EqualTo(Variance.Out));
		});
	}

	[Test]
	public void MethodNode_GenericParameters_IsEmpty_ByDefault()
	{
		var method = new MethodNode { ReturnType = TypeRef.Void, Name = "DoStuff" };

		Assert.That(method.GenericParameters, Is.Empty);
	}

	[Test]
	public void MethodNode_GenericParameters_CanBePopulated()
	{
		var method = new MethodNode
		{
			ReturnType = TypeRef.Void,
			Name = "DoStuff",
			GenericParameters =
			[
				new GenericParameter { Name = "T", Constraints = [new StructConstraint()] },
			],
		};

		Assert.That(method.GenericParameters, Has.Count.EqualTo(1));
		Assert.That(method.GenericParameters[0].Constraints[0], Is.InstanceOf<StructConstraint>());
	}

	[Test]
	public void EnumType_IsNot_GenericCodeType()
	{
		var enumType = new EnumType { Name = "Colors" };

		Assert.That(enumType, Is.Not.InstanceOf<GenericCodeType>());
	}

	[Test]
	public void GenericCodeType_IsAbstract_IsSealed_IsPartial_DefaultFalse()
	{
		var cls = new ClassType { Name = "Foo" };

		Assert.Multiple(() =>
		{
			Assert.That(cls.IsAbstract, Is.False);
			Assert.That(cls.IsSealed, Is.False);
			Assert.That(cls.IsPartial, Is.False);
		});
	}
}
