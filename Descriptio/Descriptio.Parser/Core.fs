namespace Descriptio.Parser
open Descriptio.Core.AST

module public Core =
    type public ILexer<'TIn, 'TOut> =
        abstract member Lex : input:'TIn -> 'TOut option

    type public IParser<'T> =
        abstract member Parse : input:'T -> IAbstractSyntaxTreeBlock option
