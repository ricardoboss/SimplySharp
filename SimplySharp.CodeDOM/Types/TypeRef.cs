namespace SimplySharp.CodeDOM.Types;

public abstract record TypeRef
{
#pragma warning disable CA1720 // Identifier 'x' contains type name
	public static readonly TypeRef Int = new NamedTypeRef("int");

	public static readonly TypeRef Long = new NamedTypeRef("long");

	public static readonly TypeRef Short = new NamedTypeRef("short");

	public static readonly TypeRef Float = new NamedTypeRef("float");

	public static readonly TypeRef Double = new NamedTypeRef("double");

	public static readonly TypeRef Decimal = new NamedTypeRef("decimal");

	public static readonly TypeRef UInt = new NamedTypeRef("uint");

	public static readonly TypeRef ULong = new NamedTypeRef("ulong");

	public static readonly TypeRef UShort = new NamedTypeRef("ushort");

	public static readonly TypeRef SByte = new NamedTypeRef("sbyte");

	public static readonly TypeRef String = new NamedTypeRef("string");

	public static readonly TypeRef Bool = new NamedTypeRef("bool");

	public static readonly TypeRef Void = new NamedTypeRef("void");

	public static readonly TypeRef Object = new NamedTypeRef("object");

	public static readonly TypeRef Char = new NamedTypeRef("char");

	public static readonly TypeRef Guid = new NamedTypeRef("Guid", "System");
#pragma warning restore CA1720 // Identifier 'x' contains type name

	public static readonly TypeRef NInt = new NamedTypeRef("nint");

	public static readonly TypeRef NUInt = new NamedTypeRef("nuint");

	public static readonly TypeRef Byte = new NamedTypeRef("byte");

	public static readonly TypeRef DateTime = new NamedTypeRef("DateTime", "System");

	public static readonly TypeRef DateOnly = new NamedTypeRef("DateOnly", "System");

	public static implicit operator TypeRef(Type type)
	{
		return FromType(type);
	}

	public static TypeRef FromType(Type type)
	{
		ArgumentNullException.ThrowIfNull(type);

		var underlying = Nullable.GetUnderlyingType(type);
		if (underlying is not null)
			return new NullableTypeRef(FromType(underlying));

		if (type.IsArray)
			return new ArrayTypeRef(FromType(type.GetElementType()!), type.GetArrayRank());

		if (type.Namespace == "System")
		{
			var alias = type.Name switch
			{
				"Int32" => Int,
				"Int64" => Long,
				"Int16" => Short,
				"Byte" => Byte,
				"UInt32" => UInt,
				"UInt64" => ULong,
				"UInt16" => UShort,
				"SByte" => SByte,
				"Single" => Float,
				"Double" => Double,
				"Decimal" => Decimal,
				"String" => String,
				"Boolean" => Bool,
				"Char" => Char,
				"Object" => Object,
				"Void" => Void,
				"Guid" => Guid,
				"DateTime" => DateTime,
				"DateOnly" => DateOnly,
				"IntPtr" => NInt,
				"UIntPtr" => NUInt,
				_ => null,
			};

			if (alias is not null)
				return alias;
		}

		if (type.IsGenericType)
		{
			var def = type.GetGenericTypeDefinition();
			var name = def.Name[..def.Name.IndexOf('`', StringComparison.Ordinal)];
			var args = type.GetGenericArguments().Select(FromType).ToArray();

			return new GenericTypeRef(name, def.Namespace, args);
		}

		return new NamedTypeRef(type.Name, type.Namespace);
	}
}

public record NamedTypeRef(string Name, string? Namespace = null) : TypeRef
{
	public override string ToString() => Namespace is null ? Name : $"{Namespace}.{Name}";
}

public sealed record GenericTypeRef(string Name, string? Namespace, IReadOnlyList<TypeRef> Arguments)
	: NamedTypeRef(Name, Namespace)
{
	public override string ToString() => $"{base.ToString()}<{string.Join(", ", Arguments)}>";
}

public sealed record ArrayTypeRef(TypeRef ElementType, int Rank = 1) : TypeRef
{
	public override string ToString() => $"{ElementType}[{new(',', Rank - 1)}]";
}

public sealed record NullableTypeRef(TypeRef UnderlyingType) : TypeRef
{
	public override string ToString() => $"{UnderlyingType}?";
}

public sealed record TupleTypeRef(IReadOnlyList<(TypeRef Type, string? Name)> Elements) : TypeRef
{
	public override string ToString()
	{
		var parts = Elements.Select(e => e.Name is null ? $"{e.Type}" : $"{e.Type} {e.Name}");

		return $"({string.Join(", ", parts)})";
	}
}
