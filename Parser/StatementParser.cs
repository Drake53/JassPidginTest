// ------------------------------------------------------------------------------
// <copyright file="JassPidginParser.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using Pidgin;
using System.Collections.Immutable;
using War3Net.CodeAnalysis.Jass.Extensions;
using War3Net.CodeAnalysis.Jass.Syntax;
using static Pidgin.Parser;

namespace War3Net.CodeAnalysis.Jass
{
    public partial class JassPidginParser
    {
        private static Parser<char, ICustomScriptAction> GetEmptyCustomScriptActionParser()
        {
            return Lookahead(Char('\n')).ThenReturn((ICustomScriptAction)JassEmptyStatementSyntax.Value);
        }

        private static Parser<char, ICustomScriptAction> GetLocalVariableDeclarationCustomScriptActionParser()
        {
            return Keyword("local").Then(VariableDeclaratorParser)
                .Select(declarator => (ICustomScriptAction)new JassLocalVariableDeclarationStatementSyntax(declarator))
                .Labelled("local variable declaration custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetSetCustomScriptActionParser()
        {
            return Map(
                (id, indexer, equals) => (ICustomScriptAction)new JassSetStatementSyntax(id, indexer.GetValueOrDefault(), equals),
                Keyword("set").Then(IdentifierNameParser),
                Char('[').SkipWhitespaces().Then(ExpressionParser).Before(Char(']').SkipWhitespaces()).Optional(),
                EqualsValueClauseParser)
                .Labelled("set custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetCallCustomScriptActionParser()
        {
            return Keyword("call").Then(IdentifierNameParser).Then(
                Char('(').SkipWhitespaces().Then(ArgumentListParser).Before(Char(')').SkipWhitespaces()),
                (id, arguments) => (ICustomScriptAction)new JassCallStatementSyntax(id, arguments))
                .Labelled("call custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetIfCustomScriptActionParser()
        {
            return Keyword("if").Then(ExpressionParser).Before(Keyword("then"))
                .Select(expression => (ICustomScriptAction)new JassIfCustomScriptAction(expression))
                .Labelled("if custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetElseIfCustomScriptActionParser()
        {
            return Keyword("elseif").Then(ExpressionParser).Before(Keyword("then"))
                .Select(expression => (ICustomScriptAction)new JassElseIfCustomScriptAction(expression))
                .Labelled("elseif custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetElseCustomScriptActionParser()
        {
            return Keyword("else", (ICustomScriptAction)JassElseCustomScriptAction.Value)
                .Labelled("else custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetEndIfCustomScriptActionParser()
        {
            return Keyword("endif", (ICustomScriptAction)JassEndIfCustomScriptAction.Value)
                .Labelled("endif custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetLoopCustomScriptActionParser()
        {
            return Keyword("loop", (ICustomScriptAction)JassLoopCustomScriptAction.Value)
                .Labelled("loop custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetEndLoopCustomScriptActionParser()
        {
            return Keyword("endloop", (ICustomScriptAction)JassEndLoopCustomScriptAction.Value)
                .Labelled("endloop custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetExitCustomScriptActionParser()
        {
            return Keyword("exitwhen").Then(ExpressionParser)
                .Select(expression => (ICustomScriptAction)new JassExitStatementSyntax(expression))
                .Labelled("exit custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetReturnCustomScriptActionParser()
        {
            return Keyword("return").Then(ExpressionParser.Optional())
                .Select(expression => (ICustomScriptAction)new JassReturnStatementSyntax(expression.GetValueOrDefault()))
                .Labelled("return custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetFunctionCustomScriptActionParser()
        {
            return Keyword("function").Then(FunctionDeclarationParser)
                .Select(functionDeclaration => (ICustomScriptAction)new JassFunctionCustomScriptAction(functionDeclaration))
                .Labelled("function custom script action");
        }

        private static Parser<char, ICustomScriptAction> GetEndFunctionCustomScriptActionParser()
        {
            return Keyword("endfunction", (ICustomScriptAction)JassEndFunctionCustomScriptAction.Value)
                .Labelled("endfunction custom script action");
        }

        private static Parser<char, IVariableDeclarator> GetVariableDeclaratorParser()
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
                    Keyword("array"),
                    IdentifierNameParser));
        }

        private static Parser<char, IStatementSyntax> GetEmptyStatementParser()
        {
            return Lookahead(Char('\n')).ThenReturn((IStatementSyntax)JassEmptyStatementSyntax.Value);
        }

        private static Parser<char, IStatementSyntax> GetLocalVariableDeclarationStatementParser()
        {
            return Keyword("local").Then(VariableDeclaratorParser)
                .Select(declarator => (IStatementSyntax)new JassLocalVariableDeclarationStatementSyntax(declarator))
                .Labelled("local variable declaration");
        }

        private static Parser<char, IStatementSyntax> GetSetStatementParser()
        {
            return Map(
                (id, indexer, equals) => (IStatementSyntax)new JassSetStatementSyntax(id, indexer.GetValueOrDefault(), equals),
                Keyword("set").Then(IdentifierNameParser),
                Char('[').SkipWhitespaces().Then(ExpressionParser).Before(Char(']').SkipWhitespaces()).Optional(),
                EqualsValueClauseParser)
                .Labelled("set statement");
        }

        private static Parser<char, IStatementSyntax> GetCallStatementParser()
        {
            return Keyword("call").Then(IdentifierNameParser).Then(
                Char('(').SkipWhitespaces().Then(ArgumentListParser).Before(Char(')').SkipWhitespaces()),
                (id, arguments) => (IStatementSyntax)new JassCallStatementSyntax(id, arguments))
                .Labelled("call statement");
        }

        private static Parser<char, IStatementSyntax> GetIfStatementParser(Parser<char, IStatementSyntax> statementParser)
        {
            return Map(
                (condition, statementList, elseIfStatements, elseClause, _) => (IStatementSyntax)new JassIfStatementSyntax(condition, statementList, elseIfStatements.ToImmutableArray(), elseClause.GetValueOrDefault()),
                Keyword("if").Then(ExpressionParser).SkipWhitespaces().Before(Keyword("then")).Before(Newline),
                GetStatementListParser(statementParser),
                GetElseIfStatementParser(statementParser).Many(),
                GetElseClauseParser(statementParser).Optional(),
                Keyword("endif"))
                .Labelled("if statement");
        }

        private static Parser<char, JassElseIfStatementSyntax> GetElseIfStatementParser(Parser<char, IStatementSyntax> statementParser)
        {
            return Map(
                (condition, statementList) => new JassElseIfStatementSyntax(condition, statementList),
                Keyword("elseif").Then(ExpressionParser).SkipWhitespaces().Before(Keyword("then")).Before(Newline),
                GetStatementListParser(statementParser))
                .Labelled("elseif statement");
        }

        private static Parser<char, JassElseClauseSyntax> GetElseClauseParser(Parser<char, IStatementSyntax> statementParser)
        {
            return Keyword("else").Before(Newline).Then(GetStatementListParser(statementParser))
                .Select(statementList => new JassElseClauseSyntax(statementList))
                .Labelled("else clause");
        }

        private static Parser<char, JassEqualsValueClauseSyntax> GetEqualsValueClauseParser()
        {
            return Char('=').SkipWhitespaces().Then(ExpressionParser)
                .Select(expression => new JassEqualsValueClauseSyntax(expression))
                .Labelled("equals value clause");
        }

        private static Parser<char, IStatementSyntax> GetLoopStatementParser(Parser<char, IStatementSyntax> statementParser)
        {
            return Keyword("loop").Then(Newline).Then(GetStatementListParser(statementParser)).Before(Keyword("endloop"))
                .Select(statementList => (IStatementSyntax)new JassLoopStatementSyntax(statementList))
                .Labelled("loop statement");
        }

        private static Parser<char, JassStatementListSyntax> GetStatementListParser(Parser<char, IStatementSyntax> statementParser)
        {
            return statementParser.Before(Newline).Many()
                .Select(statements => new JassStatementListSyntax(statements.ToImmutableArray()))
                ;
        }

        private static Parser<char, JassStatementListSyntax> GetStatementListParser()
        {
            return StatementParser.Before(Newline).Many()
                .Select(statements => new JassStatementListSyntax(statements.ToImmutableArray()))
                .Labelled("statement list");
        }

        private static Parser<char, IStatementSyntax> GetExitStatementParser()
        {
            return Keyword("exitwhen").Then(ExpressionParser)
                .Select(expression => (IStatementSyntax)new JassExitStatementSyntax(expression))
                .Labelled("exit statement");
        }

        private static Parser<char, IStatementSyntax> GetReturnStatementParser()
        {
            return Keyword("return").Then(ExpressionParser.Optional())
                .Select(expression => (IStatementSyntax)new JassReturnStatementSyntax(expression.GetValueOrDefault()))
                .Labelled("return statement");
        }



        private static Parser<char, JassTypeSyntax> GetTypeParser()
        {
            return OneOf(
                Keyword("code").ThenReturn(JassTypeSyntax.Code),
                Keyword("handle").ThenReturn(JassTypeSyntax.Handle),
                Keyword("integer").ThenReturn(JassTypeSyntax.Integer),
                Keyword("real").ThenReturn(JassTypeSyntax.Real),
                Keyword("boolean").ThenReturn(JassTypeSyntax.Boolean),
                Keyword("string").ThenReturn(JassTypeSyntax.String),
                IdentifierNameParser.Map(id => new JassTypeSyntax(id.Name)))
                .Labelled("type");
        }

        private static Parser<char, JassParameterListSyntax> GetParameterListParser()
        {
            return TypeParser.Then(IdentifierNameParser, (type, id) => new JassParameterSyntax(type, id)).Separated(Char(',').SkipWhitespaces())
                .Select(parameters => new JassParameterListSyntax(parameters.ToImmutableArray()))
                .Labelled("parameter list");
        }

        private static Parser<char, JassFunctionDeclarationSyntax> GetFunctionDeclarationParser()
        {
            return Map(
                (id, parameterList, returnType) => new JassFunctionDeclarationSyntax(id, parameterList, returnType),
                IdentifierNameParser.Before(Keyword("takes")),
                Keyword("nothing").ThenReturn(JassParameterListSyntax.Empty).Or(ParameterListParser),
                Keyword("returns").Then(Keyword("nothing", JassTypeSyntax.Nothing).Or(TypeParser)))
                .Labelled("function declaration");
        }
    }
}