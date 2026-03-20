using SimplySharp.CodeDOM;
using SimplySharp.CodeDOM.Attributes;
using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeGen;

/// <summary>
/// A <see cref="CodeDomVisitor"/> implementation that emits compilable, idiomatic C# source code
/// from a code DOM tree. This is the reference emitter for SimplySharp.
/// </summary>
/// <remarks>
/// <para>
/// The writer delegates all text output to a <see cref="SourceWriter"/>, which manages
/// indentation, line endings, and other whitespace conventions according to a
/// <see cref="CodeWriteSettings"/> instance.
/// </para>
/// <para>
/// The target C# language version is controlled by <see cref="CodeWriteSettings.LanguageVersion"/>.
/// Version-gated features (file-scoped namespaces, <c>record struct</c>, <c>file</c> access
/// modifier, primary constructors on classes/structs, <c>required</c> and <c>init</c>) are
/// either adapted or rejected when the target version is too low. Enum values annotated with
/// <see cref="RequiresLanguageVersionAttribute"/> are validated automatically.
/// </para>
/// <para>
/// After visiting a tree (e.g. via <see cref="CodeDomVisitor.VisitWorkspaceAsync"/>),
/// call <see cref="ToString"/> to retrieve the generated source text.
/// </para>
/// </remarks>
/// <param name="settings">Optional formatting settings. If <see langword="null"/>,
/// <see cref="CodeWriteSettings.Default"/> is used.</param>
public class CSharpCodeWriter(CodeWriteSettings? settings = null) : CodeDomVisitor
{
	private readonly CodeWriteSettings _settings = settings ?? CodeWriteSettings.Default;
	private readonly SourceWriter _writer = new(settings);

	/// <summary>
	/// Returns the generated C# source code.
	/// </summary>
	/// <returns>The accumulated source text.</returns>
	public override string ToString() => _writer.ToString();

	/// <inheritdoc />
	public override async Task VisitWorkspaceAsync(CodeWorkspace workspace,
		CancellationToken cancellationToken = default)
	{
		for (var i = 0; i < workspace.Namespaces.Count; i++)
		{
			if (i > 0)
				_writer.WriteLine();

			await workspace.Namespaces[i].AcceptAsync(this, cancellationToken);
		}
	}

	/// <inheritdoc />
	public override async Task VisitNamespaceAsync(CodeNamespace ns, CancellationToken cancellationToken = default)
	{
		if (_settings.LanguageVersion >= CSharpLanguageVersion.CSharp10)
		{
			await WriteFileScopedNamespaceAsync(ns, cancellationToken);
		}
		else
		{
			await WriteBlockScopedNamespaceAsync(ns, cancellationToken);
		}
	}

	/// <summary>
	/// Writes a file-scoped namespace declaration (<c>namespace X;</c>), available in C# 10+.
	/// </summary>
	private async Task WriteFileScopedNamespaceAsync(CodeNamespace ns, CancellationToken cancellationToken)
	{
		_writer.Write("namespace ");
		_writer.Write(ns.FullName);
		_writer.WriteLine(";");
		_writer.WriteLine();

		for (var i = 0; i < ns.Types.Count; i++)
		{
			if (i > 0)
				_writer.WriteLine();

			await ns.Types[i].AcceptAsync(this, cancellationToken);
		}

		for (var i = 0; i < ns.Children.Count; i++)
		{
			if (ns.Types.Count > 0 || i > 0)
				_writer.WriteLine();

			await ns.Children[i].AcceptAsync(this, cancellationToken);
		}
	}

	/// <summary>
	/// Writes a block-scoped namespace declaration (<c>namespace X { ... }</c>), for C# 9 and earlier.
	/// </summary>
	private async Task WriteBlockScopedNamespaceAsync(CodeNamespace ns, CancellationToken cancellationToken)
	{
		_writer.Write("namespace ");
		_writer.Write(ns.FullName);
		_writer.WriteLine();
		_writer.WriteLine("{");
		_writer.Indent();

		for (var i = 0; i < ns.Types.Count; i++)
		{
			if (i > 0)
				_writer.WriteLine();

			await ns.Types[i].AcceptAsync(this, cancellationToken);
		}

		for (var i = 0; i < ns.Children.Count; i++)
		{
			if (ns.Types.Count > 0 || i > 0)
				_writer.WriteLine();

			await ns.Children[i].AcceptAsync(this, cancellationToken);
		}

		_writer.Outdent();
		_writer.WriteLine("}");
	}

	/// <inheritdoc />
	public override async Task VisitClassTypeAsync(ClassType classType,
		CancellationToken cancellationToken = default)
	{
		if (classType.PrimaryConstructorParameters is not null)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp12, "primary constructors on classes");

		WriteAccessModifier(classType.AccessModifier);
		WriteGenericCodeTypeModifiers(classType);
		_writer.Write("class ");
		_writer.Write(classType.Name);
		WriteGenericParameters(classType.GenericParameters);
		WritePrimaryConstructorParameters(classType.PrimaryConstructorParameters);
		WriteBaseTypes(classType.Extends, classType.Implements);
		WriteGenericConstraints(classType.GenericParameters);
		_writer.WriteLine();

		await WriteTypeBodyAsync(classType.Nodes, cancellationToken);
	}

	/// <inheritdoc />
	public override async Task VisitInterfaceTypeAsync(InterfaceType interfaceType,
		CancellationToken cancellationToken = default)
	{
		WriteAccessModifier(interfaceType.AccessModifier);
		WriteGenericCodeTypeModifiers(interfaceType);
		_writer.Write("interface ");
		_writer.Write(interfaceType.Name);
		WriteGenericParameters(interfaceType.GenericParameters);
		WriteInterfaceBaseTypes(interfaceType.Extends);
		WriteGenericConstraints(interfaceType.GenericParameters);
		_writer.WriteLine();

		await WriteTypeBodyAsync(interfaceType.Nodes, cancellationToken);
	}

	/// <inheritdoc />
	public override async Task VisitStructTypeAsync(StructType structType,
		CancellationToken cancellationToken = default)
	{
		if (structType.PrimaryConstructorParameters is not null)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp12, "primary constructors on structs");

		WriteAccessModifier(structType.AccessModifier);
		WriteGenericCodeTypeModifiers(structType);
		_writer.Write("struct ");
		_writer.Write(structType.Name);
		WriteGenericParameters(structType.GenericParameters);
		WritePrimaryConstructorParameters(structType.PrimaryConstructorParameters);
		WriteBaseTypes(null, structType.Implements);
		WriteGenericConstraints(structType.GenericParameters);
		_writer.WriteLine();

		await WriteTypeBodyAsync(structType.Nodes, cancellationToken);
	}

	/// <inheritdoc />
	public override async Task VisitRecordTypeAsync(RecordType recordType,
		CancellationToken cancellationToken = default)
	{
		RequireLanguageVersion(CSharpLanguageVersion.CSharp9, "record types");
		if (recordType.Kind == RecordKind.Struct)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp10, "record structs");

		WriteAccessModifier(recordType.AccessModifier);
		WriteGenericCodeTypeModifiers(recordType);
		_writer.Write("record ");
		if (recordType.Kind == RecordKind.Struct)
			_writer.Write("struct ");

		_writer.Write(recordType.Name);
		WriteGenericParameters(recordType.GenericParameters);
		WritePrimaryConstructorParameters(recordType.PrimaryConstructorParameters);
		WriteBaseTypes(recordType.Extends, recordType.Implements);
		WriteGenericConstraints(recordType.GenericParameters);

		if (recordType.Nodes.Count == 0 && recordType.PrimaryConstructorParameters is not null)
		{
			_writer.WriteLine(";");
		}
		else
		{
			_writer.WriteLine();
			await WriteTypeBodyAsync(recordType.Nodes, cancellationToken);
		}
	}

	/// <inheritdoc />
	public override async Task VisitEnumTypeAsync(EnumType enumType,
		CancellationToken cancellationToken = default)
	{
		WriteAccessModifier(enumType.AccessModifier);
		_writer.Write("enum ");
		_writer.Write(enumType.Name);

		if (enumType.UnderlyingType is not null)
		{
			_writer.Write(" : ");
			WriteTypeRef(enumType.UnderlyingType);
		}

		_writer.WriteLine();
		_writer.WriteLine("{");
		_writer.Indent();

		for (var i = 0; i < enumType.Members.Count; i++)
		{
			await enumType.Members[i].AcceptAsync(this, cancellationToken);

			if (i < enumType.Members.Count - 1)
				_writer.WriteLine(",");
			else
				_writer.WriteLine();
		}

		_writer.Outdent();
		_writer.WriteLine("}");
	}

	/// <inheritdoc />
	public override Task VisitDelegateTypeAsync(DelegateType delegateType,
		CancellationToken cancellationToken = default)
	{
		WriteAccessModifier(delegateType.AccessModifier);
		WriteGenericCodeTypeModifiers(delegateType);
		_writer.Write("delegate ");
		WriteTypeRef(delegateType.ReturnType);
		_writer.Write(" ");
		_writer.Write(delegateType.Name);
		WriteGenericParameters(delegateType.GenericParameters);
		WriteParameterList(delegateType.Parameters);
		WriteGenericConstraints(delegateType.GenericParameters);
		_writer.WriteLine(";");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitFieldAsync(FieldNode fieldNode, CancellationToken cancellationToken = default)
	{
		WriteAccessModifier(fieldNode.AccessModifier);
		if (fieldNode.IsStatic)
			_writer.Write("static ");
		if (fieldNode.IsReadonly)
			_writer.Write("readonly ");

		WriteTypeRef(fieldNode.Type);
		_writer.Write(" ");
		_writer.Write(fieldNode.Name);
		_writer.WriteLine(";");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitPropertyAsync(PropertyNode propertyNode,
		CancellationToken cancellationToken = default)
	{
		if (propertyNode.IsRequired)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp11, "required properties");
		if (propertyNode.IsSetterInit)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp9, "init-only setters");

		WriteAccessModifier(propertyNode.AccessModifier);
		if (propertyNode.IsStatic)
			_writer.Write("static ");
		if (propertyNode.IsRequired)
			_writer.Write("required ");

		WriteTypeRef(propertyNode.Type);
		_writer.Write(" ");
		_writer.Write(propertyNode.Name);
		_writer.Write(" { ");

		if (propertyNode.HasGetter)
			_writer.Write("get; ");
		if (propertyNode.HasSetter)
		{
			if (propertyNode.IsSetterInit)
				_writer.Write("init; ");
			else
				_writer.Write("set; ");
		}

		_writer.WriteLine("}");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitMethodAsync(MethodNode methodNode, CancellationToken cancellationToken = default)
	{
		WriteAccessModifier(methodNode.AccessModifier);
		if (methodNode.IsStatic)
			_writer.Write("static ");
		if (methodNode.IsAbstract)
			_writer.Write("abstract ");
		if (methodNode.IsVirtual)
			_writer.Write("virtual ");
		if (methodNode.IsOverride)
			_writer.Write("override ");
		if (methodNode.IsAsync)
			_writer.Write("async ");

		WriteTypeRef(methodNode.ReturnType);
		_writer.Write(" ");
		_writer.Write(methodNode.Name);
		WriteGenericParameters(methodNode.GenericParameters);
		WriteParameterList(methodNode.Parameters);
		WriteGenericConstraints(methodNode.GenericParameters);

		if (methodNode.IsAbstract)
			_writer.WriteLine(";");
		else
			WriteEmptyBody();

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitConstructorAsync(ConstructorNode constructorNode,
		CancellationToken cancellationToken = default)
	{
		if (!constructorNode.IsStatic)
			WriteAccessModifier(constructorNode.AccessModifier);
		if (constructorNode.IsStatic)
			_writer.Write("static ");

		var typeName = GetEnclosingTypeName(constructorNode);
		_writer.Write(typeName);
		WriteParameterList(constructorNode.Parameters);

		if (constructorNode.Initializer is not null)
		{
			var keyword = constructorNode.Initializer == ConstructorInitializerKind.Base ? "base" : "this";
			_writer.Write(" : ");
			_writer.Write(keyword);
			_writer.Write("()");
		}

		WriteEmptyBody();

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitEventAsync(EventNode eventNode, CancellationToken cancellationToken = default)
	{
		WriteAccessModifier(eventNode.AccessModifier);
		if (eventNode.IsStatic)
			_writer.Write("static ");

		_writer.Write("event ");
		WriteTypeRef(eventNode.Type);
		_writer.Write(" ");
		_writer.Write(eventNode.Name);
		_writer.WriteLine(";");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitIndexerAsync(IndexerNode indexerNode, CancellationToken cancellationToken = default)
	{
		WriteAccessModifier(indexerNode.AccessModifier);
		if (indexerNode.IsStatic)
			_writer.Write("static ");

		WriteTypeRef(indexerNode.ReturnType);
		_writer.Write(" this");
		WriteParameterList(indexerNode.Parameters, "[", "]");
		_writer.Write(" { ");

		if (indexerNode.HasGetter)
			_writer.Write("get; ");
		if (indexerNode.HasSetter)
			_writer.Write("set; ");

		_writer.WriteLine("}");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitOperatorAsync(OperatorNode operatorNode,
		CancellationToken cancellationToken = default)
	{
		WriteAccessModifier(operatorNode.AccessModifier);
		if (operatorNode.IsStatic)
			_writer.Write("static ");

		if (operatorNode.Kind is OperatorKind.Implicit or OperatorKind.Explicit)
		{
			_writer.Write(operatorNode.Kind == OperatorKind.Implicit ? "implicit" : "explicit");
			_writer.Write(" operator ");
			WriteTypeRef(operatorNode.ReturnType);
		}
		else
		{
			WriteTypeRef(operatorNode.ReturnType);
			_writer.Write(" operator ");
			_writer.Write(GetOperatorToken(operatorNode.Kind));
		}

		WriteParameterList(operatorNode.Parameters);
		WriteEmptyBody();

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitEnumMemberAsync(EnumMemberNode enumMemberNode,
		CancellationToken cancellationToken = default)
	{
		_writer.Write(enumMemberNode.Name);

		if (enumMemberNode.Value is not null)
		{
			_writer.Write(" = ");
			_writer.Write(FormatEnumValue(enumMemberNode.Value));
		}

		return Task.CompletedTask;
	}

	/// <summary>
	/// Formats an enum member value for emission.
	/// </summary>
	/// <param name="value">The value to format.</param>
	/// <returns>The formatted string representation.</returns>
	private static string FormatEnumValue(object value)
	{
		return value switch
		{
			string s => $"\"{s}\"",
			char c => $"'{c}'",
			_ => value.ToString() ?? "0",
		};
	}

	/// <summary>
	/// Writes the body of a type (opening brace, members, closing brace).
	/// </summary>
	private async Task WriteTypeBodyAsync(IList<CodeNode> nodes, CancellationToken cancellationToken)
	{
		_writer.WriteLine("{");
		_writer.Indent();

		for (var i = 0; i < nodes.Count; i++)
		{
			if (i > 0)
				_writer.WriteLine();

			await nodes[i].AcceptAsync(this, cancellationToken);
		}

		_writer.Outdent();
		_writer.WriteLine("}");
	}

	/// <summary>
	/// Writes an empty method/constructor body (<c>{ }</c>).
	/// </summary>
	private void WriteEmptyBody()
	{
		_writer.WriteLine();
		_writer.WriteLine("{");
		_writer.WriteLine("}");
	}

	/// <summary>
	/// Writes an access modifier keyword followed by a space.
	/// Validates the modifier against the target language version using
	/// <see cref="RequiresLanguageVersionAttribute"/> annotations.
	/// </summary>
	private void WriteAccessModifier(AccessModifier modifier)
	{
		ValidateEnumLanguageVersion(modifier);

		var keyword = modifier switch
		{
			AccessModifier.Public => "public",
			AccessModifier.Protected => "protected",
			AccessModifier.Internal => "internal",
			AccessModifier.ProtectedInternal => "protected internal",
			AccessModifier.Private => "private",
			AccessModifier.PrivateProtected => "private protected",
			AccessModifier.File => "file",
			_ => throw new ArgumentOutOfRangeException(nameof(modifier), modifier, "Unknown access modifier."),
		};

		_writer.Write(keyword);
		_writer.Write(" ");
	}

	/// <summary>
	/// Writes modifier keywords for a <see cref="GenericCodeType"/> (abstract, sealed, partial).
	/// </summary>
	private void WriteGenericCodeTypeModifiers(GenericCodeType type)
	{
		if (type.IsAbstract)
			_writer.Write("abstract ");
		if (type.IsSealed)
			_writer.Write("sealed ");
		if (type.IsPartial)
			_writer.Write("partial ");
	}

	/// <summary>
	/// Writes a type reference to the output.
	/// </summary>
	private void WriteTypeRef(TypeRef typeRef)
	{
		switch (typeRef)
		{
			case TupleTypeRef tuple:
				_writer.Write("(");
				for (var i = 0; i < tuple.Elements.Count; i++)
				{
					if (i > 0)
						_writer.Write(", ");

					WriteTypeRef(tuple.Elements[i].Type);
					if (tuple.Elements[i].Name is not null)
					{
						_writer.Write(" ");
						_writer.Write(tuple.Elements[i].Name!);
					}
				}
				_writer.Write(")");
				break;
			case ArrayTypeRef array:
				WriteTypeRef(array.ElementType);
				_writer.Write("[");
				_writer.Write(new string(',', array.Rank - 1));
				_writer.Write("]");
				break;
			case NullableTypeRef nullable:
				WriteTypeRef(nullable.UnderlyingType);
				_writer.Write("?");
				break;
			case GenericTypeRef generic:
				WriteNamedTypeRefName(generic);
				_writer.Write("<");
				for (var i = 0; i < generic.Arguments.Count; i++)
				{
					if (i > 0)
						_writer.Write(", ");

					WriteTypeRef(generic.Arguments[i]);
				}
				_writer.Write(">");
				break;
			case NamedTypeRef named:
				WriteNamedTypeRefName(named);
				break;
			default:
				_writer.Write(typeRef.ToString()!);
				break;
		}
	}

	/// <summary>
	/// Writes the name portion of a <see cref="NamedTypeRef"/>, including namespace if present.
	/// </summary>
	private void WriteNamedTypeRefName(NamedTypeRef named)
	{
		if (named.Namespace is not null)
		{
			_writer.Write(named.Namespace);
			_writer.Write(".");
		}

		_writer.Write(named.Name);
	}

	/// <summary>
	/// Writes a generic parameter list (e.g., <c>&lt;T, TValue&gt;</c>).
	/// </summary>
	private void WriteGenericParameters(IList<GenericParameter> parameters)
	{
		if (parameters.Count == 0)
			return;

		_writer.Write("<");
		for (var i = 0; i < parameters.Count; i++)
		{
			if (i > 0)
				_writer.Write(", ");

			var gp = parameters[i];
			if (gp.Variance == Variance.In)
				_writer.Write("in ");
			else if (gp.Variance == Variance.Out)
				_writer.Write("out ");

			_writer.Write(gp.Name);
		}
		_writer.Write(">");
	}

	/// <summary>
	/// Writes generic constraint clauses (e.g., <c>where T : class, new()</c>).
	/// </summary>
	private void WriteGenericConstraints(IList<GenericParameter> parameters)
	{
		foreach (var gp in parameters)
		{
			if (gp.Constraints.Count == 0)
				continue;

			_writer.Write(" where ");
			_writer.Write(gp.Name);
			_writer.Write(" : ");

			for (var i = 0; i < gp.Constraints.Count; i++)
			{
				if (i > 0)
					_writer.Write(", ");

				WriteConstraint(gp.Constraints[i]);
			}
		}
	}

	/// <summary>
	/// Writes a single generic constraint.
	/// </summary>
	private void WriteConstraint(GenericConstraint constraint)
	{
		switch (constraint)
		{
			case ClassConstraint { IsNullable: true }:
				_writer.Write("class?");
				break;
			case ClassConstraint:
				_writer.Write("class");
				break;
			case StructConstraint:
				_writer.Write("struct");
				break;
			case UnmanagedConstraint:
				_writer.Write("unmanaged");
				break;
			case NotNullConstraint:
				_writer.Write("notnull");
				break;
			case NewConstraint:
				_writer.Write("new()");
				break;
			case DefaultConstraint:
				_writer.Write("default");
				break;
			case TypeConstraint tc:
				WriteTypeRef(tc.Type);
				break;
		}
	}

	/// <summary>
	/// Writes a parameter list enclosed in the specified brackets.
	/// </summary>
	private void WriteParameterList(IList<Parameter> parameters, string open = "(", string close = ")")
	{
		_writer.Write(open);
		for (var i = 0; i < parameters.Count; i++)
		{
			if (i > 0)
				_writer.Write(", ");

			WriteParameter(parameters[i]);
		}
		_writer.Write(close);
	}

	/// <summary>
	/// Writes a single parameter declaration.
	/// </summary>
	private void WriteParameter(Parameter parameter)
	{
		if (parameter.Modifier != ParameterModifier.None)
		{
			_writer.Write(parameter.Modifier switch
			{
				ParameterModifier.Ref => "ref",
				ParameterModifier.Out => "out",
				ParameterModifier.In => "in",
				ParameterModifier.Params => "params",
				_ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter.Modifier,
					"Unknown parameter modifier."),
			});
			_writer.Write(" ");
		}

		WriteTypeRef(parameter.Type);
		_writer.Write(" ");
		_writer.Write(parameter.Name);
	}

	/// <summary>
	/// Writes a primary constructor parameter list if present.
	/// </summary>
	private void WritePrimaryConstructorParameters(IList<Parameter>? parameters)
	{
		if (parameters is null)
			return;

		WriteParameterList(parameters);
	}

	/// <summary>
	/// Writes base class and implemented interface list (e.g., <c> : BaseClass, IFoo, IBar</c>).
	/// </summary>
	private void WriteBaseTypes(TypeRef? extends, ICollection<TypeRef> implements)
	{
		var hasBase = extends is not null;
		var hasInterfaces = implements.Count > 0;

		if (!hasBase && !hasInterfaces)
			return;

		_writer.Write(" : ");

		if (hasBase)
		{
			WriteTypeRef(extends!);
			if (hasInterfaces)
				_writer.Write(", ");
		}

		var first = true;
		foreach (var iface in implements)
		{
			if (!first)
				_writer.Write(", ");

			WriteTypeRef(iface);
			first = false;
		}
	}

	/// <summary>
	/// Writes an interface extends list (e.g., <c> : IFoo, IBar</c>).
	/// </summary>
	private void WriteInterfaceBaseTypes(ICollection<TypeRef> extends)
	{
		if (extends.Count == 0)
			return;

		_writer.Write(" : ");

		var first = true;
		foreach (var baseType in extends)
		{
			if (!first)
				_writer.Write(", ");

			WriteTypeRef(baseType);
			first = false;
		}
	}

	/// <summary>
	/// Gets the C# operator token for an <see cref="OperatorKind"/>.
	/// </summary>
	private static string GetOperatorToken(OperatorKind kind) => kind switch
	{
		OperatorKind.Addition => "+",
		OperatorKind.Subtraction => "-",
		OperatorKind.Multiply => "*",
		OperatorKind.Division => "/",
		OperatorKind.Modulus => "%",
		OperatorKind.UnaryPlus => "+",
		OperatorKind.UnaryNegation => "-",
		OperatorKind.Increment => "++",
		OperatorKind.Decrement => "--",
		OperatorKind.LessThan => "<",
		OperatorKind.GreaterThan => ">",
		OperatorKind.LessThanOrEqual => "<=",
		OperatorKind.GreaterThanOrEqual => ">=",
		OperatorKind.Equality => "==",
		OperatorKind.Inequality => "!=",
		OperatorKind.LogicalNot => "!",
		OperatorKind.BitwiseAnd => "&",
		OperatorKind.BitwiseOr => "|",
		OperatorKind.ExclusiveOr => "^",
		OperatorKind.BitwiseNot => "~",
		OperatorKind.True => "true",
		OperatorKind.False => "false",
		_ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown operator kind."),
	};

	/// <summary>
	/// Gets the name of the enclosing type for a constructor node.
	/// </summary>
	private static string GetEnclosingTypeName(ConstructorNode node)
	{
		return node.Parent switch
		{
			CodeType type => type.Name,
			_ => "UnknownType",
		};
	}

	/// <summary>
	/// Throws <see cref="InvalidOperationException"/> if the configured target language version
	/// is below the required version.
	/// </summary>
	/// <param name="requiredVersion">The minimum C# language version required.</param>
	/// <param name="featureName">A human-readable name for the feature, used in the error message.</param>
	private void RequireLanguageVersion(int requiredVersion, string featureName)
	{
		if (_settings.LanguageVersion < requiredVersion)
			throw new InvalidOperationException(
				$"{featureName} require C# {requiredVersion} or later, " +
				$"but the target language version is C# {_settings.LanguageVersion}.");
	}

	/// <summary>
	/// Validates that an enum value's <see cref="RequiresLanguageVersionAttribute"/> (if present)
	/// is compatible with the configured target language version.
	/// </summary>
	/// <typeparam name="TEnum">The enum type.</typeparam>
	/// <param name="value">The enum value to validate.</param>
	private void ValidateEnumLanguageVersion<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		var memberName = value.ToString();
		var memberInfo = typeof(TEnum).GetField(memberName);
		if (memberInfo is null)
			return;

		var attr = Attribute.GetCustomAttribute(memberInfo, typeof(RequiresLanguageVersionAttribute))
			as RequiresLanguageVersionAttribute;
		if (attr is not null)
			RequireLanguageVersion(attr.Major, $"'{memberName}' access modifier");
	}
}
