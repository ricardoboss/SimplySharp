namespace SimplySharp.CodeGen.Test;

public class SourceWriterTests
{
	[Test]
	public void Write_PrependsIndent_OnFirstWriteOfLine()
	{
		var writer = new SourceWriter();
		writer.Indent();
		writer.Write("hello");

		Assert.That(writer.ToString(), Does.StartWith("\thello"));
	}

	[Test]
	public void Write_DoesNotDuplicateIndent_OnSecondWriteSameLine()
	{
		var writer = new SourceWriter();
		writer.Indent();
		writer.Write("a");
		writer.Write("b");

		Assert.That(writer.ToString(), Does.StartWith("\tab"));
	}

	[Test]
	public void WriteLine_AppendsConfiguredLineEnding()
	{
		var settings = new CodeWriteSettings { LineEnding = "\r\n", InsertFinalNewline = false };
		var writer = new SourceWriter(settings);
		writer.WriteLine("hello");

		Assert.That(writer.ToString(), Is.EqualTo("hello\r\n"));
	}

	[Test]
	public void BlankWriteLine_AppendsOnlyLineEnding()
	{
		var settings = new CodeWriteSettings { InsertFinalNewline = false };
		var writer = new SourceWriter(settings);
		writer.WriteLine("a");
		writer.WriteLine();
		writer.WriteLine("b");

		Assert.That(writer.ToString(), Is.EqualTo("a\n\nb\n"));
	}

	[Test]
	public void Indent_Outdent_AdjustsLevel()
	{
		var writer = new SourceWriter();
		writer.Indent();
		writer.Indent();
		writer.WriteLine("deep");
		writer.Outdent();
		writer.Write("less");

		var result = writer.ToString();
		Assert.That(result, Does.Contain("\t\tdeep"));
		Assert.That(result, Does.Contain("\tless"));
	}

	[Test]
	public void Outdent_DoesNotGoBelowZero()
	{
		var writer = new SourceWriter();
		writer.Outdent();
		writer.Write("safe");

		Assert.That(writer.ToString(), Does.StartWith("safe"));
	}

	[Test]
	public void ToString_AppendsFinalNewline_WhenConfigured()
	{
		var settings = new CodeWriteSettings { InsertFinalNewline = true };
		var writer = new SourceWriter(settings);
		writer.Write("hello");

		Assert.That(writer.ToString(), Is.EqualTo("hello\n"));
	}

	[Test]
	public void ToString_DoesNotDuplicateFinalNewline_WhenAlreadyPresent()
	{
		var settings = new CodeWriteSettings { InsertFinalNewline = true };
		var writer = new SourceWriter(settings);
		writer.WriteLine("hello");

		Assert.That(writer.ToString(), Is.EqualTo("hello\n"));
	}

	[Test]
	public void ToString_OmitsFinalNewline_WhenNotConfigured()
	{
		var settings = new CodeWriteSettings { InsertFinalNewline = false };
		var writer = new SourceWriter(settings);
		writer.Write("hello");

		Assert.That(writer.ToString(), Is.EqualTo("hello"));
	}

	[Test]
	public void ToString_ReturnsEmpty_WhenNothingWritten()
	{
		var writer = new SourceWriter();

		Assert.That(writer.ToString(), Is.EqualTo(""));
	}

	[Test]
	public void CustomIndentStyle_IsUsed()
	{
		var settings = new CodeWriteSettings { IndentStyle = "  " };
		var writer = new SourceWriter(settings);
		writer.Indent();
		writer.Write("x");

		Assert.That(writer.ToString(), Does.StartWith("  x"));
	}

	[Test]
	public void CustomLineEnding_UsedInOutput()
	{
		var settings = new CodeWriteSettings { LineEnding = "\r\n", InsertFinalNewline = false };
		var writer = new SourceWriter(settings);
		writer.WriteLine("a");
		writer.WriteLine("b");

		Assert.That(writer.ToString(), Is.EqualTo("a\r\nb\r\n"));
	}

	[Test]
	public void DefaultConstructor_UsesDefaultSettings()
	{
		var writer = new SourceWriter();
		writer.Indent();
		writer.WriteLine("x");

		var result = writer.ToString();
		Assert.That(result, Does.StartWith("\tx"));
		Assert.That(result, Does.EndWith("\n"));
	}

	[Test]
	public void FinalNewline_WithCrLf()
	{
		var settings = new CodeWriteSettings { LineEnding = "\r\n", InsertFinalNewline = true };
		var writer = new SourceWriter(settings);
		writer.Write("end");

		Assert.That(writer.ToString(), Is.EqualTo("end\r\n"));
	}

	[Test]
	public void FinalNewline_NotDuplicated_WithCrLf()
	{
		var settings = new CodeWriteSettings { LineEnding = "\r\n", InsertFinalNewline = true };
		var writer = new SourceWriter(settings);
		writer.WriteLine("end");

		Assert.That(writer.ToString(), Is.EqualTo("end\r\n"));
	}
}
