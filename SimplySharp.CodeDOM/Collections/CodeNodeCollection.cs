using System.Collections.ObjectModel;
using SimplySharp.CodeDOM.Nodes;

namespace SimplySharp.CodeDOM.Collections;

public class CodeNodeCollection<T> : Collection<T>, ICodeNodeCollection<T> where T : ICodeNode;
