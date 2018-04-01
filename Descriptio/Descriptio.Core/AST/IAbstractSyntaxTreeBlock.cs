using System;
using System.Collections.Generic;
using System.Text;

namespace Descriptio.Core.AST
{
    public interface IAbstractSyntaxTreeBlock
    {

        IAbstractSyntaxTreeBlock Next { get; }

        IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next);
    }
}
