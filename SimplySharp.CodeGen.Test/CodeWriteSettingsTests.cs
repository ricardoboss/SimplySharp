namespace SimplySharp.CodeGen.Test;

public class CodeWriteSettingsTests
{
	[Test]
	public void Default_HasExpectedValues()
	{
		var settings = CodeWriteSettings.Default;

		Assert.That(settings.IndentStyle, Is.EqualTo("\t"));
		Assert.That(settings.LineEnding, Is.EqualTo("\n"));
		Assert.That(settings.InsertFinalNewline, Is.True);
		Assert.That(settings.LanguageVersion, Is.EqualTo(CSharpLanguageVersion.Latest));
	}

	[Test]
	public void CustomValues_ArePreserved()
	{
		var settings = new CodeWriteSettings
		{
			IndentStyle = "    ",
			LineEnding = "\r\n",
			InsertFinalNewline = false,
			LanguageVersion = CSharpLanguageVersion.CSharp9,
		};

		Assert.That(settings.IndentStyle, Is.EqualTo("    "));
		Assert.That(settings.LineEnding, Is.EqualTo("\r\n"));
		Assert.That(settings.InsertFinalNewline, Is.False);
		Assert.That(settings.LanguageVersion, Is.EqualTo(CSharpLanguageVersion.CSharp9));
	}
}
