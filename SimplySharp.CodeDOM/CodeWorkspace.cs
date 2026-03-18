using SimplySharp.CodeDOM.Collections;

namespace SimplySharp.CodeDOM;

public class CodeWorkspace
{
	public CodeWorkspace()
	{
		Namespaces = new(this);
	}

	public CodeNamespaceCollection Namespaces { get; }
}
