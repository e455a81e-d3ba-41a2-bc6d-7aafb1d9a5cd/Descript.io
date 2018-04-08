using System;
using System.Collections.Generic;
using System.Text;

namespace Descriptio.Core.AST
{
    public interface IAbstractSyntaxTreeInlineVisitor
    {
        void Visit(CleanTextInline cleanTextInline);
        void Visit(CodeTextInline codeTextInline);
        void Visit(EmphasisTextInline emphasisTextInline);
        void Visit(HyperlinkInline hyperlinkInline);
        void Visit(ImageInline imageInline);
        void Visit(StrongTextInline strongTextInline);
        void Visit(TextInline textInline);
    }
}
