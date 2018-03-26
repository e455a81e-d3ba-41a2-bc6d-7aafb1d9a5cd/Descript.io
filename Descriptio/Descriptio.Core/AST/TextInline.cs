using System;
using System.Collections.Generic;
using System.Text;
using Descriptio.Core.Extensions;

namespace Descriptio.Core.AST
{
    public abstract class TextInline
    {
        protected TextInline(string text)
        {
            Text = text.ThrowIfNullOrEmpty(nameof(text));
        }

        public string Text { get; }

        public abstract TextInline SetText(string text);
    }
}
