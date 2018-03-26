using System;

namespace Descriptio.Core.AST
{
    public class TitleAst : IAbstractSyntaxTreeLine
    {
        public TitleAst(string text, int level = 0, IAbstractSyntaxTreeLine next = null)
        {
            Text = string.IsNullOrEmpty(text) ? throw new ArgumentNullException(nameof(text)) : text;
            Level = level < 0 ? throw new ArgumentOutOfRangeException(nameof(level)) : level;
            Next = next;
        }

        public IAbstractSyntaxTreeLine Next { get; }

        public string Text { get; }

        public int Level { get; }

        public IAbstractSyntaxTreeLine SetNext(IAbstractSyntaxTreeLine next)
            => new TitleAst(Text, Level, next);

        public IAbstractSyntaxTreeLine SetText(string text) => new TitleAst(text, Level, Next);

        public IAbstractSyntaxTreeLine SetLevel(int level) => new TitleAst(Text, level, Next);
    }
}
