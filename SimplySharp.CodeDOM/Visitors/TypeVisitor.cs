using SimplySharp.CodeDOM.Types;

namespace SimplySharp.CodeDOM.Visitors;

public class TypeVisitor : ITypeVisitor
{
	public virtual Task VisitAsync(ClassType classType, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
