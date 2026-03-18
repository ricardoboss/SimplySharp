using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Collections;

public interface ICodeNodeCollection<T> : ICollection<T> where T : ICodeNode;
