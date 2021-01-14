// ------------------------------------------------------------------------------
// <copyright file="JassPidginParser.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;

using Pidgin;
using Pidgin.Expression;

using War3Net.CodeAnalysis.Jass.Syntax;

using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassPidginParser
    {
        public static IExpressionSyntax ParseExpression(string expression) => _expressionParser.ParseOrThrow(expression);

        public static ICustomScriptAction ParseCustomScriptAction(string customScriptAction) => _customScriptActionParser.ParseOrThrow(customScriptAction);

        [Obsolete]
        public static IStatementSyntax ParseStatement(string statement) => _statementParser.ParseOrThrow(statement);

        public static JassCompilationUnitSyntax ParseCompilationUnit(string compilationUnit) => _compilationUnitParser.ParseOrThrow(compilationUnit);

        public static JassFunctionSyntax ParseFunction(string function) => _functionParser.ParseOrThrow(function);

        private static Parser<char, IExpressionSyntax> GetExpressionParser()
        {
            return Pidgin.Expression.ExpressionParser.Build<char, IExpressionSyntax>(
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
                        Operator.Prefix(UnaryNotExpressionParser),
                        Operator.Prefix(UnaryPlusExpressionParser)
                            .And(Operator.Prefix(UnaryMinusExpressionParser)),
                        Operator.InfixL(BinaryMultiplicationExpressionParser)
                            .And(Operator.InfixL(BinaryDivisionExpressionParser)),
                        Operator.InfixL(BinaryAddExpressionParser)
                            .And(Operator.InfixL(BinarySubtractExpressionParser)),
                        Operator.InfixL(BinaryGreaterThanExpressionParser)
                            .And(Operator.InfixL(BinaryLessThanExpressionParser))
                            .And(Operator.InfixL(BinaryEqualsExpressionParser))
                            .And(Operator.InfixL(BinaryNotEqualsExpressionParser))
                            .And(Operator.InfixL(BinaryGreaterOrEqualExpressionParser))
                            .And(Operator.InfixL(BinaryLessOrEqualExpressionParser)),
                        Operator.InfixL(BinaryAndExpressionParser)
                            .And(Operator.InfixL(BinaryOrExpressionParser)),
                    }));
        }

        private static Parser<char, ICustomScriptAction> GetCustomScriptActionParser()
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

        private static Parser<char, IStatementSyntax> GetStatementParser()
        {
            return Pidgin.Expression.ExpressionParser.Build<char, IStatementSyntax>(
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
                    Array.Empty<OperatorTableRow<char, IStatementSyntax>>()));
        }

        private static Parser<char, IDeclaration> GetDeclarationParser()
        {
            return OneOf(
                TypeDeclarationParser,
                GlobalDeclarationListParser,
                NativeFunctionDeclarationParser)
                .Before(Newlines);
        }

        private static Parser<char, JassFunctionSyntax> GetFunctionParser()
        {
            return Map(
                (declaration, body) => new JassFunctionSyntax(declaration, body),
                Keyword("constant").Optional().Then(Keyword("function")).Then(FunctionDeclarationParser).Before(Newline),
                StatementListParser.Before(Keyword("endfunction")));
        }

        private static Parser<char, JassCompilationUnitSyntax> GetCompilationUnitParser()
        {
            return Newlines.Optional().Then(DeclarationParser.Many()).Then(FunctionParser.Many(),
                (declarations, functions) => new JassCompilationUnitSyntax(declarations.ToImmutableArray(), functions.ToImmutableArray()))
                .Before(End);
        }
    }
}