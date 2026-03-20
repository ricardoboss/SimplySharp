using SimplySharp.CodeDOM;
using SimplySharp.CodeDOM.Nodes;
using SimplySharp.CodeDOM.Types;
using SimplySharp.CodeGen;

//================== DOM ==================

var echoMethod = new MethodNode
{
	ReturnType = TypeRef.Void,
	Name = "Echo",
	Parameters =
	{
		new(TypeRef.String, "message"),
	},
};

var fooType = new ClassType
{
	Name = "Foo",
	Nodes = { echoMethod },
};

var example = new CodeNamespace
{
	Name = "Example",
	Types = { fooType },
};

var root = new CodeNamespace
{
	Name = "SimplySharp",
	Children = { example },
};

var ws = new CodeWorkspace
{
	Namespaces = { root },
};

//================== GEN ==================

var settings = CodeWriteSettings.Default;

var gen = new CSharpCodeWriter(settings);

await gen.VisitWorkspaceAsync(ws).ConfigureAwait(false);

var code = gen.ToString();

await File.WriteAllTextAsync("Foo.cs", code).ConfigureAwait(false);

//================= PARSE =================

// TODO

//================= DIFF ==================

// TODO
