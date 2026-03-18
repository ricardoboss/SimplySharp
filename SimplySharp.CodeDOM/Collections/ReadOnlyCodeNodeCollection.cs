using System.Collections.ObjectModel;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Collections;

public class ReadOnlyCodeNodeCollection<T>(IList<T> list) : ReadOnlyCollection<T>(list), IReadOnlyCodeNodeCollection<T>
	where T : ICodeNode;
