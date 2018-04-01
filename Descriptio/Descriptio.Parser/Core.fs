namespace Descriptio.Parser.Core
open Descriptio.Core.AST

type public ILexer<'TIn, 'TOut> =
    abstract member Lex : input:'TIn -> 'TOut option

type public IParser<'T> =
    abstract member Parse : input:'T -> IAbstractSyntaxTree option