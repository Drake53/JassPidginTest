// ------------------------------------------------------------------------------
// <copyright file="JassPidginTokenParser.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;

using Pidgin;
using Pidgin.Expression;

using War3Net.CodeAnalysis.Jass.Syntax;

using static Pidgin.Parser;
using static Pidgin.Parser<War3Net.CodeAnalysis.Jass.SyntaxToken>;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassPidginTokenParser
    {
        public static IExpressionSyntax ParseExpression(string expression) => _expressionParser.ParseOrThrow(new JassTokenizer(expression, false).Tokenize().ToList());

        public static ICustomScriptAction ParseCustomScriptAction(string customScriptAction) => _customScriptActionParser.ParseOrThrow(new JassTokenizer(customScriptAction, false).Tokenize().ToList());

        [Obsolete]
        public static IStatementSyntax ParseStatement(string statement) => _statementParser.ParseOrThrow(new JassTokenizer(statement, false).Tokenize().ToList());

        public static JassCompilationUnitSyntax ParseCompilationUnit(string compilationUnit) => _compilationUnitParser.ParseOrThrow(new JassTokenizer(compilationUnit, false).Tokenize().ToList());

        public static JassFunctionSyntax ParseFunction(string function) => _functionParser.ParseOrThrow(new JassTokenizer(function, false).Tokenize().ToList());

        private static Parser<SyntaxToken, IExpressionSyntax> GetExpressionParser()
        {
            return Pidgin.Expression.ExpressionParser.Build<SyntaxToken, IExpressionSyntax>(
                expressionParser =>
                (
                    OneOf(
                        // GetParenthesizedExpressionParser(expressionParser),
                        FourCCLiteralExpressionParser,
                        HexadecimalLiteralExpressionParser,
                        RealLiteralExpressionParser,
                        OctalLiteralExpressionParser,
                        DecimalLiteralExpressionParser,
                        BooleanLiteralExpressionParser,
                        StringLiteralExpressionParser,
                        NullLiteralExpressionParser,
                        FunctionReferenceExpressionParser,
#if false
                        GetIdentifierExpressionParser(expressionParser)),
#else
                        GetInvocationExpressionParser(expressionParser),
                        GetArrayReferenceExpressionParser(expressionParser),
                        VariableReferenceExpressionParser,
                        GetParenthesizedExpressionParser(expressionParser)),
#endif
                    new[]
                    {
                        // https://www.hiveworkshop.com/threads/precedence-in-jass.43500/#post-378439
                        // Operator.PostfixChainable(GetInvocationExpressionParser(expressionParser)),
                        Pidgin.Expression.Operator.Prefix(UnaryNotExpressionParser),
                        Pidgin.Expression.Operator.Prefix(UnaryPlusExpressionParser)
                            .And(Pidgin.Expression.Operator.Prefix(UnaryMinusExpressionParser)),
                        Pidgin.Expression.Operator.InfixL(BinaryMultiplicationExpressionParser)
                            .And(Pidgin.Expression.Operator.InfixL(BinaryDivisionExpressionParser)),
                        Pidgin.Expression.Operator.InfixL(BinaryAddExpressionParser)
                            .And(Pidgin.Expression.Operator.InfixL(BinarySubtractExpressionParser)),
                        Pidgin.Expression.Operator.InfixL(BinaryGreaterThanExpressionParser)
                            .And(Pidgin.Expression.Operator.InfixL(BinaryLessThanExpressionParser))
                            .And(Pidgin.Expression.Operator.InfixL(BinaryEqualsExpressionParser))
                            .And(Pidgin.Expression.Operator.InfixL(BinaryNotEqualsExpressionParser))
                            .And(Pidgin.Expression.Operator.InfixL(BinaryGreaterOrEqualExpressionParser))
                            .And(Pidgin.Expression.Operator.InfixL(BinaryLessOrEqualExpressionParser)),
                        Pidgin.Expression.Operator.InfixL(BinaryAndExpressionParser)
                            .And(Pidgin.Expression.Operator.InfixL(BinaryOrExpressionParser)),
                    }));
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetCustomScriptActionParser()
        {
            return OneOf(
                EmptyCustomScriptActionParser,
                LocalVariableDeclarationCustomScriptActionParser,

                SetCustomScriptActionParser,
                CallCustomScriptActionParser,
                IfCustomScriptActionParser,
                ElseIfCustomScriptActionParser,
                ElseCustomScriptActionParser,
                EndIfCustomScriptActionParser,
                LoopCustomScriptActionParser,
                EndLoopCustomScriptActionParser,
                ExitCustomScriptActionParser,
                ReturnCustomScriptActionParser,
                FunctionCustomScriptActionParser,
                EndFunctionCustomScriptActionParser);
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetStatementParser()
        {
            return Pidgin.Expression.ExpressionParser.Build<SyntaxToken, IStatementSyntax>(
                statementParser =>
                (
                    OneOf(
                        EmptyStatementParser,
                        LocalVariableDeclarationStatementParser,

                        SetStatementParser,
                        CallStatementParser,
                        GetIfStatementParser(statementParser),
                        GetLoopStatementParser(statementParser),
                        ExitStatementParser,
                        ReturnStatementParser),
                    Array.Empty<OperatorTableRow<SyntaxToken, IStatementSyntax>>()));
        }

        private static Parser<SyntaxToken, IDeclaration> GetDeclarationParser()
        {
            return OneOf(
                TypeDeclarationParser,
                GlobalDeclarationListParser,
                NativeFunctionDeclarationParser)
                .Before(Newline.AtLeastOnce());
        }

        private static Parser<SyntaxToken, JassFunctionSyntax> GetFunctionParser()
        {
            return Map(
                (declaration, body) => new JassFunctionSyntax(declaration, body),
                Keyword(SyntaxTokenType.ConstantKeyword).Optional().Then(Keyword(SyntaxTokenType.FunctionKeyword)).Then(FunctionDeclarationParser).Before(Newline),
                StatementListParser.Before(Keyword(SyntaxTokenType.EndfunctionKeyword)));
        }

        private static Parser<SyntaxToken, JassCompilationUnitSyntax> GetCompilationUnitParser()
        {
            return Newline.Many().Then(DeclarationParser.Many()).Then(FunctionParser.Many(),
                (declarations, functions) => new JassCompilationUnitSyntax(declarations.ToImmutableArray(), functions.ToImmutableArray()))
                .Before(End);
        }
    }
}