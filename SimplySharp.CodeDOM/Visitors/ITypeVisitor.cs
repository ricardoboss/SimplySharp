using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Visitors;

public interface ITypeVisitor
{
	Task VisitAsync(ClassType classType, CancellationToken cancellationToken);
}
