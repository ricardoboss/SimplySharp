using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Test;

public class TypeRefTests
{
	[Test]
	public void FromType_Int_ReturnsIntAlias()
	{
		var result = TypeRef.FromType(typeof(int));

		Assert.That(result, Is.EqualTo(TypeRef.Int));
	}

	[Test]
	public void FromType_String_ReturnsStringAlias()
	{
		var result = TypeRef.FromType(typeof(string));

		Assert.That(result, Is.EqualTo(TypeRef.String));
	}

	[Test]
	public void FromType_Bool_ReturnsBoolAlias()
	{
		var result = TypeRef.FromType(typeof(bool));

		Assert.That(result, Is.EqualTo(TypeRef.Bool));
	}

	[Test]
	public void FromType_Guid_ReturnsNamedTypeRefWithNamespace()
	{
		var result = TypeRef.FromType(typeof(Guid));

		Assert.That(result, Is.EqualTo(new NamedTypeRef("Guid", "System")));
	}

	[Test]
	public void FromType_IntArray_ReturnsArrayTypeRef()
	{
		var result = TypeRef.FromType(typeof(int[]));

		Assert.That(result, Is.EqualTo(new ArrayTypeRef(TypeRef.Int, 1)));
	}

	[Test]
	public void FromType_2DArray_ReturnsRank2()
	{
		var result = TypeRef.FromType(typeof(string[,]));

		Assert.That(result, Is.EqualTo(new ArrayTypeRef(TypeRef.String, 2)));
	}

	[Test]
	public void FromType_NullableInt_ReturnsNullableTypeRef()
	{
		var result = TypeRef.FromType(typeof(int?));

		Assert.That(result, Is.EqualTo(new NullableTypeRef(TypeRef.Int)));
	}

	[Test]
	public void FromType_ListOfInt_ReturnsGenericTypeRef()
	{
		var result = TypeRef.FromType(typeof(List<int>));

		var generic = result as GenericTypeRef;
		Assert.That(generic, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(generic!.Name, Is.EqualTo("List"));
			Assert.That(generic.Namespace, Is.EqualTo("System.Collections.Generic"));
			Assert.That(generic.Arguments, Has.Count.EqualTo(1));
			Assert.That(generic.Arguments[0], Is.EqualTo(TypeRef.Int));
		});
	}

	[Test]
	public void FromType_DictionaryOfStringInt_ReturnsGenericTypeRef()
	{
		var result = TypeRef.FromType(typeof(Dictionary<string, int>));

		var generic = result as GenericTypeRef;
		Assert.That(generic, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(generic!.Name, Is.EqualTo("Dictionary"));
			Assert.That(generic.Namespace, Is.EqualTo("System.Collections.Generic"));
			Assert.That(generic.Arguments, Has.Count.EqualTo(2));
			Assert.That(generic.Arguments[0], Is.EqualTo(TypeRef.String));
			Assert.That(generic.Arguments[1], Is.EqualTo(TypeRef.Int));
		});
	}

	[Test]
	public void FromType_CustomClass_ReturnsNamedTypeRef()
	{
		var result = TypeRef.FromType(typeof(CodeNamespace));

		Assert.That(result, Is.EqualTo(new NamedTypeRef("CodeNamespace", "SimplySharp.CodeDOM")));
	}

	[Test]
	public void FromType_Null_ThrowsArgumentNullException()
	{
		Assert.That(() => TypeRef.FromType(null!), Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ImplicitConversion_FromType_Works()
	{
		TypeRef result = typeof(int);

		Assert.That(result, Is.EqualTo(TypeRef.Int));
	}
}
