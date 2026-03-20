using SimplySharp.CodeDOM;
using SimplySharp.CodeDOM.Attributes;
using SimplySharp.CodeDOM.Collections;
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
	private readonly CodeWriteSettings settings = settings ?? CodeWriteSettings.Default;
	private readonly SourceWriter writer = new(settings);

	/// <summary>
	/// Returns the generated C# source code.
	/// </summary>
	/// <returns>The accumulated source text.</returns>
	public override string ToString() => writer.ToString();

	/// <inheritdoc />
	public override async Task VisitWorkspaceAsync(CodeWorkspace workspace,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(workspace);

		for (var i = 0; i < workspace.Namespaces.Count; i++)
		{
			if (i > 0)
				writer.WriteLine();

			await workspace.Namespaces[i].AcceptAsync(this, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public override async Task VisitNamespaceAsync(CodeNamespace ns, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(ns);

		if (settings.LanguageVersion >= CSharpLanguageVersion.CSharp10)
		{
			await WriteFileScopedNamespaceAsync(ns, cancellationToken).ConfigureAwait(false);
		}
		else
		{
			await WriteBlockScopedNamespaceAsync(ns, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Writes a file-scoped namespace declaration (<c>namespace X;</c>), available in C# 10+.
	/// </summary>
	private async Task WriteFileScopedNamespaceAsync(CodeNamespace ns, CancellationToken cancellationToken)
	{
		writer.Write("namespace ");
		writer.Write(ns.FullName);
		writer.WriteLine(";");
		writer.WriteLine();

		for (var i = 0; i < ns.Types.Count; i++)
		{
			if (i > 0)
				writer.WriteLine();

			await ns.Types[i].AcceptAsync(this, cancellationToken).ConfigureAwait(false);
		}

		for (var i = 0; i < ns.Children.Count; i++)
		{
			if (ns.Types.Count > 0 || i > 0)
				writer.WriteLine();

			await ns.Children[i].AcceptAsync(this, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Writes a block-scoped namespace declaration (<c>namespace X { ... }</c>), for C# 9 and earlier.
	/// </summary>
	private async Task WriteBlockScopedNamespaceAsync(CodeNamespace ns, CancellationToken cancellationToken)
	{
		writer.Write("namespace ");
		writer.Write(ns.FullName);
		writer.WriteLine();
		writer.WriteLine("{");
		writer.Indent();

		for (var i = 0; i < ns.Types.Count; i++)
		{
			if (i > 0)
				writer.WriteLine();

			await ns.Types[i].AcceptAsync(this, cancellationToken).ConfigureAwait(false);
		}

		for (var i = 0; i < ns.Children.Count; i++)
		{
			if (ns.Types.Count > 0 || i > 0)
				writer.WriteLine();

			await ns.Children[i].AcceptAsync(this, cancellationToken).ConfigureAwait(false);
		}

		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <inheritdoc />
	public override async Task VisitClassTypeAsync(ClassType classType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(classType);

		if (classType.PrimaryConstructorParameters is not null)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp12, "primary constructors on classes");

		WriteAccessModifier(classType.AccessModifier);
		WriteGenericCodeTypeModifiers(classType);
		writer.Write("class ");
		writer.Write(classType.Name);
		WriteGenericParameters(classType.GenericParameters);
		WritePrimaryConstructorParameters(classType.PrimaryConstructorParameters);
		WriteBaseTypes(classType.Extends, classType.Implements);
		WriteGenericConstraints(classType.GenericParameters);
		writer.WriteLine();

		await WriteTypeBodyAsync(classType.Nodes, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public override async Task VisitInterfaceTypeAsync(InterfaceType interfaceType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(interfaceType);

		WriteAccessModifier(interfaceType.AccessModifier);
		WriteGenericCodeTypeModifiers(interfaceType);
		writer.Write("interface ");
		writer.Write(interfaceType.Name);
		WriteGenericParameters(interfaceType.GenericParameters);
		WriteInterfaceBaseTypes(interfaceType.Extends);
		WriteGenericConstraints(interfaceType.GenericParameters);
		writer.WriteLine();

		await WriteTypeBodyAsync(interfaceType.Nodes, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public override async Task VisitStructTypeAsync(StructType structType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(structType);

		if (structType.PrimaryConstructorParameters is not null)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp12, "primary constructors on structs");

		WriteAccessModifier(structType.AccessModifier);
		WriteGenericCodeTypeModifiers(structType);
		writer.Write("struct ");
		writer.Write(structType.Name);
		WriteGenericParameters(structType.GenericParameters);
		WritePrimaryConstructorParameters(structType.PrimaryConstructorParameters);
		WriteBaseTypes(null, structType.Implements);
		WriteGenericConstraints(structType.GenericParameters);
		writer.WriteLine();

		await WriteTypeBodyAsync(structType.Nodes, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public override async Task VisitRecordTypeAsync(RecordType recordType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(recordType);

		RequireLanguageVersion(CSharpLanguageVersion.CSharp9, "record types");
		if (recordType.Kind == RecordKind.Struct)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp10, "record structs");

		WriteAccessModifier(recordType.AccessModifier);
		WriteGenericCodeTypeModifiers(recordType);
		writer.Write("record ");
		if (recordType.Kind == RecordKind.Struct)
			writer.Write("struct ");

		writer.Write(recordType.Name);
		WriteGenericParameters(recordType.GenericParameters);
		WritePrimaryConstructorParameters(recordType.PrimaryConstructorParameters);
		WriteBaseTypes(recordType.Extends, recordType.Implements);
		WriteGenericConstraints(recordType.GenericParameters);

		if (recordType.Nodes.Count == 0 && recordType.PrimaryConstructorParameters is not null)
		{
			writer.WriteLine(";");
		}
		else
		{
			writer.WriteLine();
			await WriteTypeBodyAsync(recordType.Nodes, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public override async Task VisitEnumTypeAsync(EnumType enumType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(enumType);

		WriteAccessModifier(enumType.AccessModifier);
		writer.Write("enum ");
		writer.Write(enumType.Name);

		if (enumType.UnderlyingType is not null)
		{
			writer.Write(" : ");
			WriteTypeRef(enumType.UnderlyingType);
		}

		writer.WriteLine();
		writer.WriteLine("{");
		writer.Indent();

		for (var i = 0; i < enumType.Members.Count; i++)
		{
			await enumType.Members[i].AcceptAsync(this, cancellationToken).ConfigureAwait(false);

			if (i < enumType.Members.Count - 1)
				writer.WriteLine(",");
			else
				writer.WriteLine();
		}

		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <inheritdoc />
	public override Task VisitDelegateTypeAsync(DelegateType delegateType,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(delegateType);

		WriteAccessModifier(delegateType.AccessModifier);
		WriteGenericCodeTypeModifiers(delegateType);
		writer.Write("delegate ");
		WriteTypeRef(delegateType.ReturnType);
		writer.Write(" ");
		writer.Write(delegateType.Name);
		WriteGenericParameters(delegateType.GenericParameters);
		WriteParameterList(delegateType.Parameters);
		WriteGenericConstraints(delegateType.GenericParameters);
		writer.WriteLine(";");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitFieldAsync(FieldNode fieldNode, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(fieldNode);

		WriteAccessModifier(fieldNode.AccessModifier);
		if (fieldNode.IsStatic)
			writer.Write("static ");
		if (fieldNode.IsReadonly)
			writer.Write("readonly ");

		WriteTypeRef(fieldNode.Type);
		writer.Write(" ");
		writer.Write(fieldNode.Name);
		writer.WriteLine(";");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitPropertyAsync(PropertyNode propertyNode,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(propertyNode);

		if (propertyNode.IsRequired)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp11, "required properties");
		if (propertyNode.IsSetterInit)
			RequireLanguageVersion(CSharpLanguageVersion.CSharp9, "init-only setters");

		WriteAccessModifier(propertyNode.AccessModifier);
		if (propertyNode.IsStatic)
			writer.Write("static ");
		if (propertyNode.IsRequired)
			writer.Write("required ");

		WriteTypeRef(propertyNode.Type);
		writer.Write(" ");
		writer.Write(propertyNode.Name);
		writer.Write(" { ");

		if (propertyNode.HasGetter)
			writer.Write("get; ");
		if (propertyNode.HasSetter)
		{
			if (propertyNode.IsSetterInit)
				writer.Write("init; ");
			else
				writer.Write("set; ");
		}

		writer.WriteLine("}");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitMethodAsync(MethodNode methodNode, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(methodNode);

		WriteAccessModifier(methodNode.AccessModifier);
		if (methodNode.IsStatic)
			writer.Write("static ");
		if (methodNode.IsAbstract)
			writer.Write("abstract ");
		if (methodNode.IsVirtual)
			writer.Write("virtual ");
		if (methodNode.IsOverride)
			writer.Write("override ");
		if (methodNode.IsAsync)
			writer.Write("async ");

		WriteTypeRef(methodNode.ReturnType);
		writer.Write(" ");
		writer.Write(methodNode.Name);
		WriteGenericParameters(methodNode.GenericParameters);
		WriteParameterList(methodNode.Parameters);
		WriteGenericConstraints(methodNode.GenericParameters);

		if (methodNode.IsAbstract)
			writer.WriteLine(";");
		else
			WriteEmptyBody();

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitConstructorAsync(ConstructorNode constructorNode,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(constructorNode);

		if (constructorNode.IsStatic)
			writer.Write("static ");
		else
			WriteAccessModifier(constructorNode.AccessModifier);

		var typeName = GetEnclosingTypeName(constructorNode);
		writer.Write(typeName);
		WriteParameterList(constructorNode.Parameters);

		if (constructorNode.Initializer is not null)
		{
			var keyword = constructorNode.Initializer == ConstructorInitializerKind.Base ? "base" : "this";
			writer.Write(" : ");
			writer.Write(keyword);
			writer.Write("()");
		}

		WriteEmptyBody();

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitEventAsync(EventNode eventNode, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(eventNode);

		WriteAccessModifier(eventNode.AccessModifier);
		if (eventNode.IsStatic)
			writer.Write("static ");

		writer.Write("event ");
		WriteTypeRef(eventNode.Type);
		writer.Write(" ");
		writer.Write(eventNode.Name);
		writer.WriteLine(";");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitIndexerAsync(IndexerNode indexerNode, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(indexerNode);

		WriteAccessModifier(indexerNode.AccessModifier);
		if (indexerNode.IsStatic)
			writer.Write("static ");

		WriteTypeRef(indexerNode.ReturnType);
		writer.Write(" this");
		WriteParameterList(indexerNode.Parameters, "[", "]");
		writer.Write(" { ");

		if (indexerNode.HasGetter)
			writer.Write("get; ");
		if (indexerNode.HasSetter)
			writer.Write("set; ");

		writer.WriteLine("}");

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitOperatorAsync(OperatorNode operatorNode,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(operatorNode);

		WriteAccessModifier(operatorNode.AccessModifier);
		if (operatorNode.IsStatic)
			writer.Write("static ");

		if (operatorNode.Kind is OperatorKind.Implicit or OperatorKind.Explicit)
		{
			writer.Write(operatorNode.Kind == OperatorKind.Implicit ? "implicit" : "explicit");
			writer.Write(" operator ");
			WriteTypeRef(operatorNode.ReturnType);
		}
		else
		{
			WriteTypeRef(operatorNode.ReturnType);
			writer.Write(" operator ");
			writer.Write(GetOperatorToken(operatorNode.Kind));
		}

		WriteParameterList(operatorNode.Parameters);
		WriteEmptyBody();

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public override Task VisitEnumMemberAsync(EnumMemberNode enumMemberNode,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(enumMemberNode);

		writer.Write(enumMemberNode.Name);

		if (enumMemberNode.Value is not null)
		{
			writer.Write(" = ");
			writer.Write(FormatEnumValue(enumMemberNode.Value));
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
	private async Task WriteTypeBodyAsync(CodeNodeCollection<CodeNode> nodes, CancellationToken cancellationToken)
	{
		writer.WriteLine("{");
		writer.Indent();

		for (var i = 0; i < nodes.Count; i++)
		{
			if (i > 0)
				writer.WriteLine();

			await nodes[i].AcceptAsync(this, cancellationToken).ConfigureAwait(false);
		}

		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>
	/// Writes an empty method/constructor body (<c>{ }</c>).
	/// </summary>
	private void WriteEmptyBody()
	{
		writer.WriteLine();
		writer.WriteLine("{");
		writer.WriteLine("}");
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

		writer.Write(keyword);
		writer.Write(" ");
	}

	/// <summary>
	/// Writes modifier keywords for a <see cref="GenericCodeType"/> (abstract, sealed, partial).
	/// </summary>
	private void WriteGenericCodeTypeModifiers(GenericCodeType type)
	{
		if (type.IsAbstract)
			writer.Write("abstract ");
		if (type.IsSealed)
			writer.Write("sealed ");
		if (type.IsPartial)
			writer.Write("partial ");
	}

	/// <summary>
	/// Writes a type reference to the output.
	/// </summary>
	private void WriteTypeRef(TypeRef typeRef)
	{
		switch (typeRef)
		{
			case TupleTypeRef tuple:
				writer.Write("(");
				for (var i = 0; i < tuple.Elements.Count; i++)
				{
					if (i > 0)
						writer.Write(", ");

					WriteTypeRef(tuple.Elements[i].Type);
					if (tuple.Elements[i].Name is not null)
					{
						writer.Write(" ");
						writer.Write(tuple.Elements[i].Name!);
					}
				}
				writer.Write(")");
				break;
			case ArrayTypeRef array:
				WriteTypeRef(array.ElementType);
				writer.Write("[");
				if (array.Rank > 1)
					writer.Write(new string(',', array.Rank - 1));
				writer.Write("]");
				break;
			case NullableTypeRef nullable:
				WriteTypeRef(nullable.UnderlyingType);
				writer.Write("?");
				break;
			case GenericTypeRef generic:
				WriteNamedTypeRefName(generic);
				writer.Write("<");
				for (var i = 0; i < generic.Arguments.Count; i++)
				{
					if (i > 0)
						writer.Write(", ");

					WriteTypeRef(generic.Arguments[i]);
				}
				writer.Write(">");
				break;
			case NamedTypeRef named:
				WriteNamedTypeRefName(named);
				break;
			default:
				writer.Write(typeRef.ToString()!);
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
			writer.Write(named.Namespace);
			writer.Write(".");
		}

		writer.Write(named.Name);
	}

	/// <summary>
	/// Writes a generic parameter list (e.g., <c>&lt;T, TValue&gt;</c>).
	/// </summary>
	private void WriteGenericParameters(IList<GenericParameter> parameters)
	{
		if (parameters.Count == 0)
			return;

		writer.Write("<");
		for (var i = 0; i < parameters.Count; i++)
		{
			if (i > 0)
				writer.Write(", ");

			var gp = parameters[i];
			if (gp.Variance == Variance.In)
				writer.Write("in ");
			else if (gp.Variance == Variance.Out)
				writer.Write("out ");

			writer.Write(gp.Name);
		}
		writer.Write(">");
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

			writer.Write(" where ");
			writer.Write(gp.Name);
			writer.Write(" : ");

			for (var i = 0; i < gp.Constraints.Count; i++)
			{
				if (i > 0)
					writer.Write(", ");

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
				writer.Write("class?");
				break;
			case ClassConstraint:
				writer.Write("class");
				break;
			case StructConstraint:
				writer.Write("struct");
				break;
			case UnmanagedConstraint:
				writer.Write("unmanaged");
				break;
			case NotNullConstraint:
				writer.Write("notnull");
				break;
			case NewConstraint:
				writer.Write("new()");
				break;
			case DefaultConstraint:
				writer.Write("default");
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
		writer.Write(open);
		for (var i = 0; i < parameters.Count; i++)
		{
			if (i > 0)
				writer.Write(", ");

			WriteParameter(parameters[i]);
		}
		writer.Write(close);
	}

	/// <summary>
	/// Writes a single parameter declaration.
	/// </summary>
	private void WriteParameter(Parameter parameter)
	{
		if (parameter.Modifier != ParameterModifier.None)
		{
			writer.Write(parameter.Modifier switch
			{
				ParameterModifier.Ref => "ref",
				ParameterModifier.Out => "out",
				ParameterModifier.In => "in",
				ParameterModifier.Params => "params",
				_ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter.Modifier,
					"Unknown parameter modifier."),
			});
			writer.Write(" ");
		}

		WriteTypeRef(parameter.Type);
		writer.Write(" ");
		writer.Write(parameter.Name);
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

		writer.Write(" : ");

		if (hasBase)
		{
			WriteTypeRef(extends!);
			if (hasInterfaces)
				writer.Write(", ");
		}

		var first = true;
		foreach (var iface in implements)
		{
			if (!first)
				writer.Write(", ");

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

		writer.Write(" : ");

		var first = true;
		foreach (var baseType in extends)
		{
			if (!first)
				writer.Write(", ");

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
		if (settings.LanguageVersion < requiredVersion)
			throw new InvalidOperationException(
				$"{featureName} require C# {requiredVersion} or later, " +
				$"but the target language version is C# {settings.LanguageVersion}.");
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

		var attr = Attribute.GetCustomAttribute(memberInfo, typeof(RequiresLanguageVersionAttribute));
		if (attr is RequiresLanguageVersionAttribute rlva)
			RequireLanguageVersion(rlva.Major, $"'{memberName}' access modifier");
	}
}
