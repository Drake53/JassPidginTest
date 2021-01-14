// ------------------------------------------------------------------------------
// <copyright file="ExpressionTokenParser.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;

using Pidgin;

using War3Net.CodeAnalysis.Jass.Extensions;
using War3Net.CodeAnalysis.Jass.Syntax;
using War3Net.Common.Extensions;

using static Pidgin.Parser;
using static Pidgin.Parser<War3Net.CodeAnalysis.Jass.SyntaxToken>;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassPidginTokenParser
    {
        private static Parser<SyntaxToken, JassIdentifierNameSyntax> GetIdentifierNameParser()
        {
            return Token(SyntaxTokenType.AlphanumericIdentifier)
                .Select(token => new JassIdentifierNameSyntax(token.TokenValue))
                .Labelled("identifier name");
        }

        private static Parser<SyntaxToken, JassArgumentListSyntax> GetArgumentListParser(Parser<SyntaxToken, IExpressionSyntax> expressionParser)
        {
            return expressionParser.Separated(Token(SyntaxTokenType.Comma))
                .Select(arguments => new JassArgumentListSyntax(arguments.ToImmutableArray()));
        }

        private static Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> GetBinaryExpressionParser(Parser<SyntaxToken, BinaryOperatorType> operatorParser)
        {
            return operatorParser.Select<Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>>(@operator => (left, right) => new JassBinaryExpressionSyntax(@operator, left, right));
        }

        private static Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax>> GetUnaryExpressionParser(Parser<SyntaxToken, UnaryOperatorType> operatorParser)
        {
            return operatorParser.Select<Func<IExpressionSyntax, IExpressionSyntax>>(@operator => expression => new JassUnaryExpressionSyntax(@operator, expression));
        }

#if true
        private static Parser<SyntaxToken, IExpressionSyntax> GetInvocationExpressionParser(Parser<SyntaxToken, IExpressionSyntax> expressionParser)
        {
            return Try(IdentifierNameParser.Before(Token(SyntaxTokenType.ParenthesisOpenSymbol)))
                .Then(GetArgumentListParser(expressionParser).Before(Token(SyntaxTokenType.ParenthesisCloseSymbol)), (id, arguments) => (IExpressionSyntax)new JassInvocationExpressionSyntax(id, arguments))
                .Labelled("invocation expression");
        }
#else
        private static Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax>> GetInvocationExpressionParser(Parser<SyntaxToken, IExpressionSyntax> expressionParser)
        {
            return Char('(').SkipWhitespaces().Then(GetArgumentListParser(expressionParser)).Before(Char(')'))
                .Select<Func<IExpressionSyntax, IExpressionSyntax>>(arguments => expression => expression is JassVariableReferenceExpressionSyntax e
                    ? new JassInvocationExpressionSyntax(e.IdentifierName, arguments)
                    : new JassInvocationExpressionSyntax(new JassIdentifierNameSyntax("INVALID"), arguments))
                .Labelled("invocation expression");
        }
#endif

        private static Parser<SyntaxToken, IExpressionSyntax> GetArrayReferenceExpressionParser(Parser<SyntaxToken, IExpressionSyntax> expressionParser)
        {
            return Try(IdentifierNameParser
                .Before(Token(SyntaxTokenType.SquareBracketOpenSymbol)))
                .Then(expressionParser, (id, indexer) => (IExpressionSyntax)new JassArrayReferenceExpressionSyntax(id, indexer))
                .Before(Token(SyntaxTokenType.SquareBracketCloseSymbol))
                .Labelled("array reference");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetFunctionReferenceExpressionParser()
        {
            return Keyword(SyntaxTokenType.FunctionKeyword).Then(IdentifierNameParser)
                .Select<IExpressionSyntax>(name => new JassFunctionReferenceExpressionSyntax(name))
                .Labelled("function reference");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetDecimalLiteralExpressionParser()
        {
            return Token(SyntaxTokenType.DecimalNumber)
                .Then(token => int.TryParse(token.TokenValue, out var value)
                    ? Return((IExpressionSyntax)new JassDecimalLiteralExpressionSyntax(value))
                    : Fail<IExpressionSyntax>($"{token.TokenValue} is not a valid decimal number"))
                .Labelled("decimal literal");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetOctalLiteralExpressionParser()
        {
            return Token(SyntaxTokenType.OctalNumber)
                .Then(token => token.TokenValue.TryParseOctal(out var value)
                    ? Return((IExpressionSyntax)new JassOctalLiteralExpressionSyntax(value))
                    : Fail<IExpressionSyntax>($"{token.TokenValue} is not a valid octal number"))
                .Labelled("octal literal");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetHexadecimalLiteralExpressionParser()
        {
            return Token(SyntaxTokenType.HexadecimalNumber)
                .Then(token => token.TokenValue.Substring(token.TokenValue[0] == '$' ? 1 : 2).TryParseHexadecimal(out var value)
                    ? Return((IExpressionSyntax)new JassHexadecimalLiteralExpressionSyntax(value))
                    : Fail<IExpressionSyntax>($"{token.TokenValue} is not a valid hexadecimal number"))
                .Labelled("hexadecimal literal");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetFourCCLiteralExpressionParser()
        {
            return Token(SyntaxTokenType.FourCCNumber)
                .Then(token => token.TokenValue.TryFromRawcode(out var value)
                    ? Return((IExpressionSyntax)new JassFourCCLiteralExpressionSyntax(value))
                    : Fail<IExpressionSyntax>($"{token.TokenValue} is not a valid fourCC number"))
                .Between(Token(SyntaxTokenType.SingleQuote))
                .Labelled("fourCC literal");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetRealLiteralExpressionParser()
        {
            return Token(SyntaxTokenType.RealNumber)
                .Then(token => float.TryParse(token.TokenValue, out var value)
                    ? Return((IExpressionSyntax)new JassRealLiteralExpressionSyntax(value))
                    : Fail<IExpressionSyntax>($"{token.TokenValue} is not a valid real number"))
                .Labelled("real literal");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetBooleanLiteralExpressionParser()
        {
            return Keyword<IExpressionSyntax>(SyntaxTokenType.TrueKeyword, JassBooleanLiteralExpressionSyntax.True)
                .Or(Keyword<IExpressionSyntax>(SyntaxTokenType.FalseKeyword, JassBooleanLiteralExpressionSyntax.False))
                .Labelled("boolean literal");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetStringLiteralExpressionParser()
        {
            return Token(SyntaxTokenType.String).Between(Token(SyntaxTokenType.DoubleQuotes))
                .Select<IExpressionSyntax>(token => new JassStringLiteralExpressionSyntax(token.TokenValue))
                .Labelled("string literal");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetNullLiteralExpressionParser()
        {
            return Keyword<IExpressionSyntax>(SyntaxTokenType.NullKeyword, JassNullLiteralExpressionSyntax.Null)
                .Labelled("null literal");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetParenthesizedExpressionParser(Parser<SyntaxToken, IExpressionSyntax> expressionParser)
        {
            return Token(SyntaxTokenType.ParenthesisOpenSymbol).Then(expressionParser).Before(Token(SyntaxTokenType.ParenthesisCloseSymbol))
                .Select<IExpressionSyntax>(expression => new JassParenthesizedExpressionSyntax(expression))
                .Labelled("parenthesized expression");
        }

        private static Parser<SyntaxToken, IExpressionSyntax> GetVariableReferenceExpressionParser()
        {
            return IdentifierNameParser
                .Select<IExpressionSyntax>(name => new JassVariableReferenceExpressionSyntax(name))
                .Labelled("variable reference");
        }
    }
}