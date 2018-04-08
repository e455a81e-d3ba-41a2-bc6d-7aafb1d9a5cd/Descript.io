using System;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public class ImageInline : IAbstractSyntaxTreeInline, IEquatable<ImageInline>
    {
        public ImageInline(string alt, string src, string title = null)
        {
            Alt = alt.ThrowIfNullOrEmpty(nameof(alt));
            Src = src.ThrowIfNullOrEmpty(nameof(src));
            Title = title;
        }

        public string Alt { get; }

        public string Src { get; }

        public string Title { get; }

        public virtual ImageInline SetAlt(string alt) => new ImageInline(alt, Src, Title);

        public virtual ImageInline SetSrc(string src) => new ImageInline(Alt, src, Title);

        public virtual ImageInline SetTitle(string title) => new ImageInline(Alt, Src, title);

        public virtual bool Equals(ImageInline other) =>
            ReferenceEquals(this, other)
            || string.Equals(Alt, other.Alt)
            && string.Equals(Src, other.Src)
            && string.Equals(Title, other.Title);

        public override bool Equals(object obj) => obj is ImageInline img && Equals(img);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Alt?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Src?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Title?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public void Accept(IAbstractSyntaxTreeInlineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

