using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class CodeBlock : IAbstractSyntaxTreeBlock, IEquatable<CodeBlock>
    {
        public CodeBlock(string language, IImmutableList<string> lines, IAbstractSyntaxTreeBlock next = null)
        {
            Language = language;
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
            Next = next;
        }
        public CodeBlock(string language, IEnumerable<string> lines, IAbstractSyntaxTreeBlock next = null)
        {
            Language = language;
            Lines = ImmutableList.CreateRange(lines ?? throw new ArgumentNullException(nameof(lines)));
            Next = next;
        }

        public IAbstractSyntaxTreeBlock Next { get; }

        public IImmutableList<string> Lines { get; }

        public string Language { get; }

        public virtual IAbstractSyntaxTreeBlock SetNext(IAbstractSyntaxTreeBlock newNext)
            => new CodeBlock(Language, Lines, newNext);

        public virtual CodeBlock SetLines(IImmutableList<string> newLines) => new CodeBlock(Language, newLines, Next);

        public virtual CodeBlock SetLanguage(string newLanguage) => new CodeBlock(newLanguage, Lines, Next);

        public virtual void Accept(IAbstractSyntaxTreeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public virtual bool Equals(CodeBlock other)
            => ReferenceEquals(this, other)
               || !(other is null)
               && Equals(Next, other.Next)
               && Lines.IsEquivalentTo(other.Lines)
               && string.Equals(Language, other.Language);

        public override bool Equals(object obj) => obj is CodeBlock other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Next?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Lines?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Language?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
