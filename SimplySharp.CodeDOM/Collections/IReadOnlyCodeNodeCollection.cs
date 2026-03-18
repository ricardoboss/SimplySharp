using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Collections;

public interface IReadOnlyCodeNodeCollection<out T> : IReadOnlyCollection<T> where T : ICodeNode;
