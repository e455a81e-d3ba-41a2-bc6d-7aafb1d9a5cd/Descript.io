using System.IO;
using Descriptio.Core.AST;

namespace Descriptio.Transform
{
    public interface IFormatter
    {
        void Transform(IAbstractSyntaxTreeBlock abstractSyntaxTreeBlock, Stream stream);
    }
}
