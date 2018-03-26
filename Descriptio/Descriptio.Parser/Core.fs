namespace Descriptio.Parser

open Descriptio.Core.AST

module public Core =
    type public IParser<'T> =
        abstract member Parse : 'T -> IAbstractSyntaxTree