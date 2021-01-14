// ------------------------------------------------------------------------------
// <copyright file="Parsers.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Pidgin;

using War3Net.CodeAnalysis.Jass.Extensions;
using War3Net.CodeAnalysis.Jass.Syntax;

using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassPidginParser
    {
        private static readonly Parser<char, string> EscapeChar = OneOf(
            Char('"').ThenReturn("\\\""),
            Char('r').ThenReturn("\\r"),
            Char('n').ThenReturn("\\n"),
            Char('t').ThenReturn("\\t"),
            Char('\\').ThenReturn("\\\\"))
            .Labelled("escape character");

        [Obsolete]
        private static Parser<char, T> Tok<T>(Parser<char, T> token) => Try(token).SkipWhitespaces();

        [Obsolete]
        private static Parser<char, string> Tok(string token) => Tok(String(token));

        private static Parser<char, Unit> Keyword(string keyword) => Keyword(keyword, Unit.Value);
        private static Parser<char, T> Keyword<T>(string keyword, T result) => Try(String(keyword).AssertNotFollowedByLetterOrDigitOrUnderscore()).SkipWhitespaces().ThenReturn(result).Labelled($"'{keyword}' keyword");

        private static readonly Parser<char, char> LetterOrDigitOrUnderscore = Token(c => char.IsLetterOrDigit(c) || c == '_').Labelled("letter or digit or underscore");
        private static readonly Parser<char, string> Comment = Tok("//").Then(AnyCharExcept('\n').Many()).Select(chars => new string(chars.ToArray()));
        private static readonly Parser<char, char> Newline = Comment.Optional().Then(Char('\n').SkipWhitespaces()).Labelled("newline");
        private static readonly Parser<char, IEnumerable<char>> Newlines = Newline.AtLeastOnce().Labelled("newlines");

        private static readonly Parser<char, JassIdentifierNameSyntax> IdentifierNameParser = GetIdentifierNameParser();

        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryAddExpressionParser = GetBinaryExpressionParser(Tok("+").ThenReturn(BinaryOperatorType.Add));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinarySubtractExpressionParser = GetBinaryExpressionParser(Tok("-").ThenReturn(BinaryOperatorType.Subtract));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryMultiplicationExpressionParser = GetBinaryExpressionParser(Tok("*").ThenReturn(BinaryOperatorType.Multiplication));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryDivisionExpressionParser = GetBinaryExpressionParser(Tok("/").ThenReturn(BinaryOperatorType.Division));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryGreaterThanExpressionParser = GetBinaryExpressionParser(Tok(">").ThenReturn(BinaryOperatorType.GreaterThan));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryLessThanExpressionParser = GetBinaryExpressionParser(Tok("<").ThenReturn(BinaryOperatorType.LessThan));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryEqualsExpressionParser = GetBinaryExpressionParser(Tok("==").ThenReturn(BinaryOperatorType.Equals));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryNotEqualsExpressionParser = GetBinaryExpressionParser(Tok("!=").ThenReturn(BinaryOperatorType.NotEquals));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryGreaterOrEqualExpressionParser = GetBinaryExpressionParser(Tok(">=").ThenReturn(BinaryOperatorType.GreaterOrEqual));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryLessOrEqualExpressionParser = GetBinaryExpressionParser(Tok("<=").ThenReturn(BinaryOperatorType.LessOrEqual));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryAndExpressionParser = GetBinaryExpressionParser(Keyword("and", BinaryOperatorType.And));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax, IExpressionSyntax>> BinaryOrExpressionParser = GetBinaryExpressionParser(Keyword("or", BinaryOperatorType.Or));

        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax>> UnaryPlusExpressionParser = GetUnaryExpressionParser(Tok("+").ThenReturn(UnaryOperatorType.Plus));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax>> UnaryMinusExpressionParser = GetUnaryExpressionParser(Tok("-").ThenReturn(UnaryOperatorType.Minus));
        private static readonly Parser<char, Func<IExpressionSyntax, IExpressionSyntax>> UnaryNotExpressionParser = GetUnaryExpressionParser(Keyword("not").ThenReturn(UnaryOperatorType.Not));

        private static readonly Parser<char, IExpressionSyntax> FunctionReferenceExpressionParser = GetFunctionReferenceExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> DecimalLiteralExpressionParser = GetDecimalLiteralExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> OctalLiteralExpressionParser = GetOctalLiteralExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> HexadecimalLiteralExpressionParser = GetHexadecimalLiteralExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> FourCCLiteralExpressionParser = GetFourCCLiteralExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> RealLiteralExpressionParser = GetRealLiteralExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> BooleanLiteralExpressionParser = GetBooleanLiteralExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> StringLiteralExpressionParser = GetStringLiteralExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> NullLiteralExpressionParser = GetNullLiteralExpressionParser();
        private static readonly Parser<char, IExpressionSyntax> VariableReferenceExpressionParser = GetVariableReferenceExpressionParser();

        private static readonly Parser<char, IExpressionSyntax> ExpressionParser = GetExpressionParser();

        private static readonly Parser<char, JassArgumentListSyntax> ArgumentListParser = GetArgumentListParser(ExpressionParser);
        private static readonly Parser<char, JassEqualsValueClauseSyntax> EqualsValueClauseParser = GetEqualsValueClauseParser();
        private static readonly Parser<char, JassTypeSyntax> TypeParser = GetTypeParser();
        private static readonly Parser<char, IVariableDeclarator> VariableDeclaratorParser = GetVariableDeclaratorParser();

        private static readonly Parser<char, IStatementSyntax> EmptyStatementParser = GetEmptyStatementParser();
        private static readonly Parser<char, IStatementSyntax> LocalVariableDeclarationStatementParser = GetLocalVariableDeclarationStatementParser();
        private static readonly Parser<char, IStatementSyntax> SetStatementParser = GetSetStatementParser();
        private static readonly Parser<char, IStatementSyntax> CallStatementParser = GetCallStatementParser();
        private static readonly Parser<char, IStatementSyntax> ExitStatementParser = GetExitStatementParser();
        private static readonly Parser<char, IStatementSyntax> ReturnStatementParser = GetReturnStatementParser();
        private static readonly Parser<char, IStatementSyntax> StatementParser = GetStatementParser();

        private static readonly Parser<char, JassParameterListSyntax> ParameterListParser = GetParameterListParser();
        private static readonly Parser<char, JassStatementListSyntax> StatementListParser = GetStatementListParser();
        private static readonly Parser<char, JassFunctionDeclarationSyntax> FunctionDeclarationParser = GetFunctionDeclarationParser();
        private static readonly Parser<char, JassFunctionSyntax> FunctionParser = GetFunctionParser();

        private static readonly Parser<char, JassGlobalDeclarationSyntax> GlobalDeclarationParser = GetGlobalDeclarationParser();

        private static readonly Parser<char, ICustomScriptAction> EmptyCustomScriptActionParser = GetEmptyCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> LocalVariableDeclarationCustomScriptActionParser = GetLocalVariableDeclarationCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> SetCustomScriptActionParser = GetSetCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> CallCustomScriptActionParser = GetCallCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> IfCustomScriptActionParser = GetIfCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> ElseIfCustomScriptActionParser = GetElseIfCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> ElseCustomScriptActionParser = GetElseCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> EndIfCustomScriptActionParser = GetEndIfCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> LoopCustomScriptActionParser = GetLoopCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> EndLoopCustomScriptActionParser = GetEndLoopCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> ExitCustomScriptActionParser = GetExitCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> ReturnCustomScriptActionParser = GetReturnCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> FunctionCustomScriptActionParser = GetFunctionCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> EndFunctionCustomScriptActionParser = GetEndFunctionCustomScriptActionParser();
        private static readonly Parser<char, ICustomScriptAction> CustomScriptActionParser = GetCustomScriptActionParser();

        private static readonly Parser<char, IDeclaration> TypeDeclarationParser = GetTypeDeclarationParser();
        private static readonly Parser<char, IDeclaration> GlobalDeclarationListParser = GetGlobalDeclarationListParser();
        private static readonly Parser<char, IDeclaration> NativeFunctionDeclarationParser = GetNativeFunctionDeclarationParser();
        private static readonly Parser<char, IDeclaration> DeclarationParser = GetDeclarationParser();

        private static readonly Parser<char, JassCompilationUnitSyntax> CompilationUnitParser = GetCompilationUnitParser();

        private static readonly Parser<char, Unit> TrimWhitespaces = Token(@char => char.IsWhiteSpace(@char) && @char != '\n').Many().IgnoreResult();

        private static readonly Parser<char, IExpressionSyntax> _expressionParser = TrimWhitespaces.Then(ExpressionParser.Before(End));
        private static readonly Parser<char, ICustomScriptAction> _customScriptActionParser = TrimWhitespaces.Then(CustomScriptActionParser.Before(Comment.Optional().Then(End)));
        [Obsolete]
        private static readonly Parser<char, IStatementSyntax> _statementParser = TrimWhitespaces.Then(StatementParser.Before(End));
        private static readonly Parser<char, JassFunctionSyntax> _functionParser = TrimWhitespaces.Then(FunctionParser.Before(End));
        private static readonly Parser<char, JassCompilationUnitSyntax> _compilationUnitParser = TrimWhitespaces.Then(CompilationUnitParser.Before(End));
    }
}