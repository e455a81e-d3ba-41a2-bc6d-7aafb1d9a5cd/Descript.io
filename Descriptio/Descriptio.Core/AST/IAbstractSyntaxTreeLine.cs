using System;
using System.Collections.Generic;
using System.Text;

namespace Descriptio.Core.AST
{
    public interface IAbstractSyntaxTreeLine
    {

        IAbstractSyntaxTreeLine Next { get; }

        IAbstractSyntaxTreeLine SetNext(IAbstractSyntaxTreeLine next);
    }
}
