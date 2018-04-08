using System;

namespace Descriptio.Core.AST
{
    public class TitleAst : IAbstractSyntaxTreeBlock, IEquatable<TitleAst>
    {
        public TitleAst(string text, int level = 1, IAbstractSyntaxTreeBlock next = null)
        {
            Text = string.IsNullOrEmpty(text) ? throw new ArgumentNullException(nameof(text)) : text;
            Level = level <= 0 ? throw new ArgumentOutOfRangeException(nameof(level)) : level;
            Next = next;
        }

        public IAbstractSyntaxTreeBlock Next { get; }

        public string Text { get; }

        public int Level { get; }

        public IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock next)
            => new TitleAst(Text, Level, next);

        public void Accept(IAbstractSyntaxTreeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public IAbstractSyntaxTreeBlock SetText(string text) => new TitleAst(text, Level, Next);

        public IAbstractSyntaxTreeBlock SetLevel(int level) => new TitleAst(Text, level, Next);

        public bool Equals(TitleAst other) => !(other is null) && Text == other.Text && Level == other.Level;

        public override bool Equals(object obj) => obj is TitleAst ast && Equals(ast);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Next != null ? Next.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Level;
                return hashCode;
            }
        }
    }
}
