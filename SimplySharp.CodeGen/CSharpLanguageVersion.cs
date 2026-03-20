namespace SimplySharp.CodeGen;

/// <summary>
/// Well-known C# language version constants for use with
/// <see cref="CodeWriteSettings.LanguageVersion"/>.
/// </summary>
/// <remarks>
/// <para>
/// Each constant corresponds to the major version number of a C# language release.
/// <see cref="CSharpCodeWriter"/> uses the configured version to decide which syntax
/// features to emit (e.g. file-scoped namespaces require <see cref="CSharp10"/>).
/// </para>
/// </remarks>
public static class CSharpLanguageVersion
{
	/// <summary>C# 1.0.</summary>
	public const int CSharp1 = 1;

	/// <summary>C# 2.0 — generics, nullable value types, anonymous methods.</summary>
	public const int CSharp2 = 2;

	/// <summary>C# 3.0 — LINQ, extension methods, lambda expressions.</summary>
	public const int CSharp3 = 3;

	/// <summary>C# 4.0 — dynamic, optional parameters, named arguments.</summary>
	public const int CSharp4 = 4;

	/// <summary>C# 5.0 — async/await.</summary>
	public const int CSharp5 = 5;

	/// <summary>C# 6.0 — expression-bodied members, string interpolation.</summary>
	public const int CSharp6 = 6;

	/// <summary>C# 7.0 — tuples, pattern matching, local functions.</summary>
	public const int CSharp7 = 7;

	/// <summary>C# 8.0 — nullable reference types, default interface methods.</summary>
	public const int CSharp8 = 8;

	/// <summary>C# 9.0 — records, init-only setters, top-level statements.</summary>
	public const int CSharp9 = 9;

	/// <summary>C# 10.0 — file-scoped namespaces, record structs, <c>file</c> access modifier.</summary>
	public const int CSharp10 = 10;

	/// <summary>C# 11.0 — required members, raw string literals.</summary>
	public const int CSharp11 = 11;

	/// <summary>C# 12.0 — primary constructors for classes and structs.</summary>
	public const int CSharp12 = 12;

	/// <summary>C# 13.0 — extension blocks.</summary>
	public const int CSharp13 = 13;

	/// <summary>
	/// A sentinel value that enables all language features. This is the default value for
	/// <see cref="CodeWriteSettings.LanguageVersion"/>.
	/// </summary>
	public const int Latest = int.MaxValue;
}
