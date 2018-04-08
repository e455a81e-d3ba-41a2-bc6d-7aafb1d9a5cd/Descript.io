using System;
using System.Collections.Generic;
using System.Text;

namespace Descriptio.Core.AST
{
    public interface IAbstractSyntaxTreeVisitor
    {
        void Visit(TitleAst titleAst);
        void Visit(TextParagraphBlock textParagraphBlock);
        void Visit(EnumerationBlock enumerationBlock);
    }
}
