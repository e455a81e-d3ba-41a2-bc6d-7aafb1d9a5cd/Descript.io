namespace Descriptio.Core.AST
{
    public interface IAbstractSyntaxTreeBlock : IAbstractSyntaxTree
    {

        IAbstractSyntaxTreeBlock Next { get; }

        IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next);

        void Accept(IAbstractSyntaxTreeVisitor visitor);
    }
}
