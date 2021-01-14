// ------------------------------------------------------------------------------
// <copyright file="JassPidginParser.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using Pidgin;
using System.Collections.Immutable;
using War3Net.CodeAnalysis.Jass.Syntax;
using static Pidgin.Parser;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassPidginTokenParser
    {
        private static Parser<SyntaxToken, ICustomScriptAction> GetEmptyCustomScriptActionParser()
        {
            return Lookahead(Newline).ThenReturn((ICustomScriptAction)JassEmptyStatementSyntax.Value);
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetLocalVariableDeclarationCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.LocalKeyword).Then(VariableDeclaratorParser)
                .Select(declarator => (ICustomScriptAction)new JassLocalVariableDeclarationStatementSyntax(declarator))
                .Labelled("local variable declaration custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetSetCustomScriptActionParser()
        {
            return Map(
                (id, indexer, equals) => (ICustomScriptAction)new JassSetStatementSyntax(id, indexer.GetValueOrDefault(), equals),
                Keyword(SyntaxTokenType.SetKeyword).Then(IdentifierNameParser),
                Token(SyntaxTokenType.SquareBracketOpenSymbol).Then(ExpressionParser).Before(Token(SyntaxTokenType.SquareBracketCloseSymbol)).Optional(),
                EqualsValueClauseParser)
                .Labelled("set custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetCallCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.CallKeyword).Then(IdentifierNameParser).Then(
                Token(SyntaxTokenType.ParenthesisOpenSymbol).Then(ArgumentListParser).Before(Token(SyntaxTokenType.ParenthesisCloseSymbol)),
                (id, arguments) => (ICustomScriptAction)new JassCallStatementSyntax(id, arguments))
                .Labelled("call custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetIfCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.IfKeyword).Then(ExpressionParser).Before(Keyword(SyntaxTokenType.ThenKeyword))
                .Select(expression => (ICustomScriptAction)new JassIfCustomScriptAction(expression))
                .Labelled("if custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetElseIfCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.ElseifKeyword).Then(ExpressionParser).Before(Keyword(SyntaxTokenType.ThenKeyword))
                .Select(expression => (ICustomScriptAction)new JassElseIfCustomScriptAction(expression))
                .Labelled("elseif custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetElseCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.ElseKeyword, (ICustomScriptAction)JassElseCustomScriptAction.Value)
                .Labelled("else custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetEndIfCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.EndifKeyword, (ICustomScriptAction)JassEndIfCustomScriptAction.Value)
                .Labelled("endif custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetLoopCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.LoopKeyword, (ICustomScriptAction)JassLoopCustomScriptAction.Value)
                .Labelled("loop custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetEndLoopCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.EndloopKeyword, (ICustomScriptAction)JassEndLoopCustomScriptAction.Value)
                .Labelled("endloop custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetExitCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.ExitwhenKeyword).Then(ExpressionParser)
                .Select(expression => (ICustomScriptAction)new JassExitStatementSyntax(expression))
                .Labelled("exit custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetReturnCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.ReturnKeyword).Then(ExpressionParser.Optional())
                .Select(expression => (ICustomScriptAction)new JassReturnStatementSyntax(expression.GetValueOrDefault()))
                .Labelled("return custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetFunctionCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.FunctionKeyword).Then(FunctionDeclarationParser)
                .Select(functionDeclaration => (ICustomScriptAction)new JassFunctionCustomScriptAction(functionDeclaration))
                .Labelled("function custom script action");
        }

        private static Parser<SyntaxToken, ICustomScriptAction> GetEndFunctionCustomScriptActionParser()
        {
            return Keyword(SyntaxTokenType.EndfunctionKeyword, (ICustomScriptAction)JassEndFunctionCustomScriptAction.Value)
                .Labelled("endfunction custom script action");
        }

        private static Parser<SyntaxToken, IVariableDeclarator> GetVariableDeclaratorParser()
        {
            return OneOf(
                Map(
                    (type, id, value) => (IVariableDeclarator)new JassVariableDeclaratorSyntax(type, id, value.GetValueOrDefault()),
                    TypeParser,
                    IdentifierNameParser,
                    EqualsValueClauseParser.Optional()),
                Map(
                    (type, _, id) => (IVariableDeclarator)new JassArrayDeclaratorSyntax(type, id),
                    TypeParser,
                    Keyword(SyntaxTokenType.ArrayKeyword),
                    IdentifierNameParser));
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetEmptyStatementParser()
        {
            return Lookahead(Newline).ThenReturn((IStatementSyntax)JassEmptyStatementSyntax.Value);
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetLocalVariableDeclarationStatementParser()
        {
            return Keyword(SyntaxTokenType.LocalKeyword).Then(VariableDeclaratorParser)
                .Select(declarator => (IStatementSyntax)new JassLocalVariableDeclarationStatementSyntax(declarator))
                .Labelled("local variable declaration");
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetSetStatementParser()
        {
            return Map(
                (id, indexer, equals) => (IStatementSyntax)new JassSetStatementSyntax(id, indexer.GetValueOrDefault(), equals),
                Keyword(SyntaxTokenType.SetKeyword).Then(IdentifierNameParser),
                Token(SyntaxTokenType.SquareBracketOpenSymbol).Then(ExpressionParser).Before(Token(SyntaxTokenType.SquareBracketCloseSymbol)).Optional(),
                EqualsValueClauseParser)
                .Labelled("set statement");
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetCallStatementParser()
        {
            return Keyword(SyntaxTokenType.CallKeyword).Then(IdentifierNameParser).Then(
                Token(SyntaxTokenType.ParenthesisOpenSymbol).Then(ArgumentListParser).Before(Token(SyntaxTokenType.ParenthesisCloseSymbol)),
                (id, arguments) => (IStatementSyntax)new JassCallStatementSyntax(id, arguments))
                .Labelled("call statement");
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetIfStatementParser(Parser<SyntaxToken, IStatementSyntax> statementParser)
        {
            return Map(
                (condition, statementList, elseIfStatements, elseClause, _) => (IStatementSyntax)new JassIfStatementSyntax(condition, statementList, elseIfStatements.ToImmutableArray(), elseClause.GetValueOrDefault()),
                Keyword(SyntaxTokenType.IfKeyword).Then(ExpressionParser).Before(Keyword(SyntaxTokenType.ThenKeyword)).Before(Newline),
                GetStatementListParser(statementParser),
                GetElseIfStatementParser(statementParser).Many(),
                GetElseClauseParser(statementParser).Optional(),
                Keyword(SyntaxTokenType.EndifKeyword))
                .Labelled("if statement");
        }

        private static Parser<SyntaxToken, JassElseIfStatementSyntax> GetElseIfStatementParser(Parser<SyntaxToken, IStatementSyntax> statementParser)
        {
            return Map(
                (condition, statementList) => new JassElseIfStatementSyntax(condition, statementList),
                Keyword(SyntaxTokenType.ElseifKeyword).Then(ExpressionParser).Before(Keyword(SyntaxTokenType.ThenKeyword)).Before(Newline),
                GetStatementListParser(statementParser))
                .Labelled("elseif statement");
        }

        private static Parser<SyntaxToken, JassElseClauseSyntax> GetElseClauseParser(Parser<SyntaxToken, IStatementSyntax> statementParser)
        {
            return Keyword(SyntaxTokenType.ElseKeyword).Before(Newline).Then(GetStatementListParser(statementParser))
                .Select(statementList => new JassElseClauseSyntax(statementList))
                .Labelled("else clause");
        }

        private static Parser<SyntaxToken, JassEqualsValueClauseSyntax> GetEqualsValueClauseParser()
        {
            return Token(SyntaxTokenType.Assignment).Then(ExpressionParser)
                .Select(expression => new JassEqualsValueClauseSyntax(expression))
                .Labelled("equals value clause");
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetLoopStatementParser(Parser<SyntaxToken, IStatementSyntax> statementParser)
        {
            return Keyword(SyntaxTokenType.LoopKeyword).Then(Newline).Then(GetStatementListParser(statementParser)).Before(Keyword(SyntaxTokenType.EndloopKeyword))
                .Select(statementList => (IStatementSyntax)new JassLoopStatementSyntax(statementList))
                .Labelled("loop statement");
        }

        private static Parser<SyntaxToken, JassStatementListSyntax> GetStatementListParser(Parser<SyntaxToken, IStatementSyntax> statementParser)
        {
            return statementParser.Before(Newline).Many()
                .Select(statements => new JassStatementListSyntax(statements.ToImmutableArray()))
                ;
        }

        private static Parser<SyntaxToken, JassStatementListSyntax> GetStatementListParser()
        {
            return StatementParser.Before(Newline).Many()
                .Select(statements => new JassStatementListSyntax(statements.ToImmutableArray()))
                .Labelled("statement list");
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetExitStatementParser()
        {
            return Keyword(SyntaxTokenType.ExitwhenKeyword).Then(ExpressionParser)
                .Select(expression => (IStatementSyntax)new JassExitStatementSyntax(expression))
                .Labelled("exit statement");
        }

        private static Parser<SyntaxToken, IStatementSyntax> GetReturnStatementParser()
        {
            return Keyword(SyntaxTokenType.ReturnKeyword).Then(ExpressionParser.Optional())
                .Select(expression => (IStatementSyntax)new JassReturnStatementSyntax(expression.GetValueOrDefault()))
                .Labelled("return statement");
        }



        private static Parser<SyntaxToken, JassTypeSyntax> GetReferenceTypeParser()
        {
            return OneOf(
                Keyword(SyntaxTokenType.HandleKeyword).ThenReturn(JassTypeSyntax.Handle),
                IdentifierNameParser.Map(id => new JassTypeSyntax(id.Name)))
                .Labelled("reference type");
        }

        private static Parser<SyntaxToken, JassTypeSyntax> GetTypeParser()
        {
            return OneOf(
                Keyword(SyntaxTokenType.CodeKeyword).ThenReturn(JassTypeSyntax.Code),
                Keyword(SyntaxTokenType.HandleKeyword).ThenReturn(JassTypeSyntax.Handle),
                Keyword(SyntaxTokenType.IntegerKeyword).ThenReturn(JassTypeSyntax.Integer),
                Keyword(SyntaxTokenType.RealKeyword).ThenReturn(JassTypeSyntax.Real),
                Keyword(SyntaxTokenType.BooleanKeyword).ThenReturn(JassTypeSyntax.Boolean),
                Keyword(SyntaxTokenType.StringKeyword).ThenReturn(JassTypeSyntax.String),
                IdentifierNameParser.Map(id => new JassTypeSyntax(id.Name)))
                .Labelled("type");
        }

        private static Parser<SyntaxToken, JassParameterListSyntax> GetParameterListParser()
        {
            return TypeParser.Then(IdentifierNameParser, (type, id) => new JassParameterSyntax(type, id)).Separated(Token(SyntaxTokenType.Comma))
                .Select(parameters => new JassParameterListSyntax(parameters.ToImmutableArray()))
                .Labelled("parameter list");
        }

        private static Parser<SyntaxToken, JassFunctionDeclarationSyntax> GetFunctionDeclarationParser()
        {
            return Map(
                (id, parameterList, returnType) => new JassFunctionDeclarationSyntax(id, parameterList, returnType),
                IdentifierNameParser.Before(Keyword(SyntaxTokenType.TakesKeyword)),
                Keyword(SyntaxTokenType.NothingKeyword).ThenReturn(JassParameterListSyntax.Empty).Or(ParameterListParser),
                Keyword(SyntaxTokenType.ReturnsKeyword).Then(Keyword(SyntaxTokenType.NothingKeyword, JassTypeSyntax.Nothing).Or(TypeParser)))
                .Labelled("function declaration");
        }
    }
}