// ------------------------------------------------------------------------------
// <copyright file="TokenParsers.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using Pidgin;
using System;
using War3Net.CodeAnalysis.Jass.Syntax;
using static Pidgin.Parser;
using static Pidgin.Parser<War3Net.CodeAnalysis.Jass.SyntaxToken>;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassPidginTokenParser
    {
        private static readonly Parser<char, string> EscapeChar = OneOf(
            Char('"').ThenReturn("\\\""),
            Char('r').ThenReturn("\\r"),
            Char('n').ThenReturn("\\n"),
            Char('t').ThenReturn("\\t"),
            Char('\\').ThenReturn("\\\\"))
            .Labelled("escape character");

        private static Parser<SyntaxToken, SyntaxToken> Token(SyntaxTokenType tokenType) => Parser<SyntaxToken>.Token(token => token.TokenType == tokenType);

        private static Parser<SyntaxToken, Unit> Keyword(SyntaxTokenType keyword) => Keyword(keyword, Unit.Value);
        private static Parser<SyntaxToken, T> Keyword<T>(SyntaxTokenType keyword, T result) => Token(keyword).ThenReturn(result).Labelled($"'{keyword}' keyword");
        private static Parser<SyntaxToken, T> Operator<T>(SyntaxTokenType @operator, T result) => Token(@operator).ThenReturn(result).Labelled($"'{@operator}' operator");

        private static readonly Parser<SyntaxToken, string> Comment = Token(SyntaxTokenType.DoubleForwardSlash).Then(Token(SyntaxTokenType.Comment)).Select(token => token.TokenValue);
        private static readonly Parser<SyntaxToken, SyntaxToken> Newline = Comment.Optional().Then(Token(SyntaxTokenType.NewlineSymbol)).Labelled("newline");

        private static readonly Parser<SyntaxToken, JassIdentifierNameSyntax> IdentifierNameParser = GetIdentifierNameParser();

        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryAddExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.PlusOperator, BinaryOperatorType.Add));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinarySubtractExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.MinusOperator, BinaryOperatorType.Subtract));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryMultiplicationExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.MultiplicationOperator, BinaryOperatorType.Multiplication));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryDivisionExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.DivisionOperator, BinaryOperatorType.Division));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryGreaterThanExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.GreaterThanOperator, BinaryOperatorType.GreaterThan));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryLessThanExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.LessThanOperator, BinaryOperatorType.LessThan));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryEqualsExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.EqualityOperator, BinaryOperatorType.Equals));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryNotEqualsExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.UnequalityOperator, BinaryOperatorType.NotEquals));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryGreaterOrEqualExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.GreaterOrEqualOperator, BinaryOperatorType.GreaterOrEqual));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryLessOrEqualExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.LessOrEqualOperator, BinaryOperatorType.LessOrEqual));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryAndExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.AndOperator, BinaryOperatorType.And));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryOrExpressionParser = GetBinaryExpressionParser(Operator(SyntaxTokenType.OrOperator, BinaryOperatorType.Or));

        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax>> UnaryPlusExpressionParser = GetUnaryExpressionParser(Operator(SyntaxTokenType.PlusOperator, UnaryOperatorType.Plus));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax>> UnaryMinusExpressionParser = GetUnaryExpressionParser(Operator(SyntaxTokenType.MinusOperator, UnaryOperatorType.Minus));
        private static readonly Parser<SyntaxToken, Func<IExpressionSyntax, IExpressionSyntax>> UnaryNotExpressionParser = GetUnaryExpressionParser(Operator(SyntaxTokenType.NotOperator, UnaryOperatorType.Not));

        private static readonly Parser<SyntaxToken, IExpressionSyntax> FunctionReferenceExpressionParser = GetFunctionReferenceExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> DecimalLiteralExpressionParser = GetDecimalLiteralExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> OctalLiteralExpressionParser = GetOctalLiteralExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> HexadecimalLiteralExpressionParser = GetHexadecimalLiteralExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> FourCCLiteralExpressionParser = GetFourCCLiteralExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> RealLiteralExpressionParser = GetRealLiteralExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> BooleanLiteralExpressionParser = GetBooleanLiteralExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> StringLiteralExpressionParser = GetStringLiteralExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> NullLiteralExpressionParser = GetNullLiteralExpressionParser();
        private static readonly Parser<SyntaxToken, IExpressionSyntax> VariableReferenceExpressionParser = GetVariableReferenceExpressionParser();

        private static readonly Parser<SyntaxToken, IExpressionSyntax> ExpressionParser = GetExpressionParser();

        private static readonly Parser<SyntaxToken, JassArgumentListSyntax> ArgumentListParser = GetArgumentListParser(ExpressionParser);
        private static readonly Parser<SyntaxToken, JassEqualsValueClauseSyntax> EqualsValueClauseParser = GetEqualsValueClauseParser();
        private static readonly Parser<SyntaxToken, JassTypeSyntax> ReferenceTypeParser = GetReferenceTypeParser();
        private static readonly Parser<SyntaxToken, JassTypeSyntax> TypeParser = GetTypeParser();
        private static readonly Parser<SyntaxToken, IVariableDeclarator> VariableDeclaratorParser = GetVariableDeclaratorParser();

        private static readonly Parser<SyntaxToken, IStatementSyntax> EmptyStatementParser = GetEmptyStatementParser();
        private static readonly Parser<SyntaxToken, IStatementSyntax> LocalVariableDeclarationStatementParser = GetLocalVariableDeclarationStatementParser();
        private static readonly Parser<SyntaxToken, IStatementSyntax> SetStatementParser = GetSetStatementParser();
        private static readonly Parser<SyntaxToken, IStatementSyntax> CallStatementParser = GetCallStatementParser();
        private static readonly Parser<SyntaxToken, IStatementSyntax> ExitStatementParser = GetExitStatementParser();
        private static readonly Parser<SyntaxToken, IStatementSyntax> ReturnStatementParser = GetReturnStatementParser();
        private static readonly Parser<SyntaxToken, IStatementSyntax> StatementParser = GetStatementParser();

        private static readonly Parser<SyntaxToken, JassParameterListSyntax> ParameterListParser = GetParameterListParser();
        private static readonly Parser<SyntaxToken, JassStatementListSyntax> StatementListParser = GetStatementListParser();
        private static readonly Parser<SyntaxToken, JassFunctionDeclarationSyntax> FunctionDeclarationParser = GetFunctionDeclarationParser();
        private static readonly Parser<SyntaxToken, JassFunctionSyntax> FunctionParser = GetFunctionParser();

        private static readonly Parser<SyntaxToken, JassGlobalDeclarationSyntax> GlobalDeclarationParser = GetGlobalDeclarationParser();

        private static readonly Parser<SyntaxToken, ICustomScriptAction> EmptyCustomScriptActionParser = GetEmptyCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> LocalVariableDeclarationCustomScriptActionParser = GetLocalVariableDeclarationCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> SetCustomScriptActionParser = GetSetCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> CallCustomScriptActionParser = GetCallCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> IfCustomScriptActionParser = GetIfCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> ElseIfCustomScriptActionParser = GetElseIfCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> ElseCustomScriptActionParser = GetElseCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> EndIfCustomScriptActionParser = GetEndIfCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> LoopCustomScriptActionParser = GetLoopCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> EndLoopCustomScriptActionParser = GetEndLoopCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> ExitCustomScriptActionParser = GetExitCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> ReturnCustomScriptActionParser = GetReturnCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> FunctionCustomScriptActionParser = GetFunctionCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> EndFunctionCustomScriptActionParser = GetEndFunctionCustomScriptActionParser();
        private static readonly Parser<SyntaxToken, ICustomScriptAction> CustomScriptActionParser = GetCustomScriptActionParser();

        private static readonly Parser<SyntaxToken, IDeclaration> TypeDeclarationParser = GetTypeDeclarationParser();
        private static readonly Parser<SyntaxToken, IDeclaration> GlobalDeclarationListParser = GetGlobalDeclarationListParser();
        private static readonly Parser<SyntaxToken, IDeclaration> NativeFunctionDeclarationParser = GetNativeFunctionDeclarationParser();
        private static readonly Parser<SyntaxToken, IDeclaration> DeclarationParser = GetDeclarationParser();

        private static readonly Parser<SyntaxToken, JassCompilationUnitSyntax> CompilationUnitParser = GetCompilationUnitParser();

        private static readonly Parser<SyntaxToken, IExpressionSyntax> _expressionParser = ExpressionParser.Before(End);
        private static readonly Parser<SyntaxToken, ICustomScriptAction> _customScriptActionParser = CustomScriptActionParser.Before(Comment.Optional().Then(End));
        [Obsolete]
        private static readonly Parser<SyntaxToken, IStatementSyntax> _statementParser = StatementParser.Before(End);
        private static readonly Parser<SyntaxToken, JassFunctionSyntax> _functionParser = FunctionParser.Before(End);
        private static readonly Parser<SyntaxToken, JassCompilationUnitSyntax> _compilationUnitParser = CompilationUnitParser.Before(End);
    }
}