using System.Text;

namespace SimplySharp.CodeGen;

/// <summary>
/// A low-level text output helper that manages indentation, line endings, and whitespace
/// conventions according to a <see cref="CodeWriteSettings"/> instance.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="SourceWriter"/> is the single place that knows about indentation levels and line
/// termination. Higher-level emitters (e.g. <see cref="CSharpCodeWriter"/>) call
/// <see cref="Write"/>, <see cref="WriteLine(string)"/>, <see cref="Indent"/>, and
/// <see cref="Outdent"/> without worrying about how indentation or newlines are rendered.
/// </para>
/// </remarks>
/// <param name="settings">The formatting settings to use. If <see langword="null"/>,
/// <see cref="CodeWriteSettings.Default"/> is used.</param>
public class SourceWriter(CodeWriteSettings? settings = null)
{
	private readonly CodeWriteSettings _settings = settings ?? CodeWriteSettings.Default;
	private readonly StringBuilder _sb = new();
	private int _indentLevel;
	private bool _lineHasContent;

	/// <summary>
	/// Increases the indentation level by one.
	/// </summary>
	public void Indent() => _indentLevel++;

	/// <summary>
	/// Decreases the indentation level by one. Does nothing if the level is already zero.
	/// </summary>
	public void Outdent()
	{
		if (_indentLevel > 0)
			_indentLevel--;
	}

	/// <summary>
	/// Appends text to the output. If this is the first write on the current line, the
	/// configured indentation is prepended automatically.
	/// </summary>
	/// <param name="text">The text to append.</param>
	public void Write(string text)
	{
		if (!_lineHasContent)
		{
			for (var i = 0; i < _indentLevel; i++)
				_sb.Append(_settings.IndentStyle);

			_lineHasContent = true;
		}

		_sb.Append(text);
	}

	/// <summary>
	/// Appends text followed by a line ending to the output.
	/// </summary>
	/// <param name="text">The text to append before the line ending.</param>
	public void WriteLine(string text)
	{
		Write(text);
		_sb.Append(_settings.LineEnding);
		_lineHasContent = false;
	}

	/// <summary>
	/// Appends a blank line (just a line ending) to the output.
	/// </summary>
	public void WriteLine()
	{
		_sb.Append(_settings.LineEnding);
		_lineHasContent = false;
	}

	/// <summary>
	/// Returns the accumulated output text. If <see cref="CodeWriteSettings.InsertFinalNewline"/>
	/// is <see langword="true"/> and the output does not already end with a line ending, one is
	/// appended.
	/// </summary>
	/// <returns>The generated source text.</returns>
	public override string ToString()
	{
		if (_settings.InsertFinalNewline && _sb.Length > 0 && !EndsWith(_settings.LineEnding))
			return _sb.ToString() + _settings.LineEnding;

		return _sb.ToString();
	}

	/// <summary>
	/// Checks whether the internal buffer ends with the specified suffix.
	/// </summary>
	private bool EndsWith(string suffix)
	{
		if (_sb.Length < suffix.Length)
			return false;

		var offset = _sb.Length - suffix.Length;
		for (var i = 0; i < suffix.Length; i++)
		{
			if (_sb[offset + i] != suffix[i])
				return false;
		}

		return true;
	}
}
