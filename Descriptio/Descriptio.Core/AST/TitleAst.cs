using System;

namespace Descriptio.Core.AST
{
    public class TitleAst : IAbstractSyntaxTreeBlock
    {
        public TitleAst(string text, int level = 0, IAbstractSyntaxTreeBlock next = null)
        {
            Text = string.IsNullOrEmpty(text) ? throw new ArgumentNullException(nameof(text)) : text;
            Level = level < 0 ? throw new ArgumentOutOfRangeException(nameof(level)) : level;
            Next = next;
        }

        public IAbstractSyntaxTreeBlock Next { get; }

        public string Text { get; }

        public int Level { get; }

        public IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next)
            => new TitleAst(Text, Level, next);

        public IAbstractSyntaxTreeBlock SetText(string text) => new TitleAst(text, Level, Next);

        public IAbstractSyntaxTreeBlock SetLevel(int level) => new TitleAst(Text, level, Next);
    }
}
