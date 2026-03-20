namespace SimplySharp.CodeGen;

/// <summary>
/// Configuration for code output formatting. Controls indentation style, line endings,
/// target language version, and other whitespace conventions used by <see cref="SourceWriter"/>.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="LanguageVersion"/> property controls which C# language features the emitter
/// may use. Version-gated features (e.g. file-scoped namespaces, <c>record struct</c>,
/// <c>file</c> access modifier) are emitted only when the target version is high enough.
/// Use <see cref="CSharpLanguageVersion"/> for well-known version constants.
/// </para>
/// <para>
/// Additional formatting properties exist as extension points for future <c>.editorconfig</c>
/// integration.
/// </para>
/// </remarks>
public record CodeWriteSettings
{
	/// <summary>
	/// Gets a default instance with tab indentation, <c>\n</c> line endings, a final newline,
	/// and the latest language version.
	/// </summary>
	public static CodeWriteSettings Default { get; } = new();

	/// <summary>
	/// Gets the string used for one level of indentation. Defaults to a single tab character.
	/// </summary>
	public string IndentStyle { get; init; } = "\t";

	/// <summary>
	/// Gets the line ending sequence. Defaults to <c>\n</c>.
	/// </summary>
	public string LineEnding { get; init; } = "\n";

	/// <summary>
	/// Gets a value indicating whether a trailing newline is appended at the end of the output.
	/// Defaults to <see langword="true"/>.
	/// </summary>
	public bool InsertFinalNewline { get; init; } = true;

	/// <summary>
	/// Gets the target C# language version as a major version number (e.g. 10, 11, 12).
	/// Defaults to <see cref="CSharpLanguageVersion.Latest"/>, which enables all language features.
	/// Use constants from <see cref="CSharpLanguageVersion"/> for well-known versions.
	/// </summary>
	public int LanguageVersion { get; init; } = CSharpLanguageVersion.Latest;
}
