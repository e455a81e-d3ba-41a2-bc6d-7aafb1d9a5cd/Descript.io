using System;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    [System.Diagnostics.DebuggerDisplay("[{" + nameof(Text) + "}] ({" + nameof(Href) + "} {" + nameof(Title) + "})")]
    public class HyperlinkInline : TextInline, IEquatable<HyperlinkInline>
    {

        public HyperlinkInline(string text, string href, string title = null) : base(text)
        {
            Href = href.ThrowIfNullOrEmpty(nameof(href));
            Title = title;
        }

        public string Href { get; }

        public string Title { get; }

        public override TextInline SetText(string text) => new HyperlinkInline(text, Href, Title);

        public virtual HyperlinkInline SetHref(string href) => new HyperlinkInline(Text, href, Title);

        public virtual HyperlinkInline SetTitle(string title) => new HyperlinkInline(Text, Href, title);

        public virtual bool Equals(HyperlinkInline other)
            => ReferenceEquals(this, other)
            || string.Equals(Text, other.Text)
            && string.Equals(Href, other.Href)
            && string.Equals(Title, other.Title);

        public override bool Equals(object obj) => obj is HyperlinkInline other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Text?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Href?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Title?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override void Accept(IAbstractSyntaxTreeInlineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
