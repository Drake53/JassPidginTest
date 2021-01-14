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
    public partial class JassPidginParser
    {
        private static Parser<char, JassGlobalDeclarationSyntax> GetGlobalDeclarationParser()
        {
            return Keyword("constant").Then(Map((type, id, value) => (IVariableDeclarator)new JassVariableDeclaratorSyntax(type, id, value), TypeParser, IdentifierNameParser, EqualsValueClauseParser))
                .Or(VariableDeclaratorParser)
                .Before(Newlines)
                .Select(declarator => new JassGlobalDeclarationSyntax(declarator));
        }

        private static Parser<char, IDeclaration> GetTypeDeclarationParser()
        {
            return Keyword("type").Then(IdentifierNameParser).Then(
                Keyword("extends").Then(TypeParser),
                (@new, @base) => (IDeclaration)new JassTypeDeclarationSyntax(@new, @base));
        }

        private static Parser<char, IDeclaration> GetGlobalDeclarationListParser()
        {
            return Keyword("globals").Then(Newlines).Then(GlobalDeclarationParser.Many()).Before(Keyword("endglobals"))
                .Select<IDeclaration>(globals => new JassGlobalDeclarationListSyntax(globals.ToImmutableArray()));
        }

        private static Parser<char, IDeclaration> GetNativeFunctionDeclarationParser()
        {
            return Keyword("constant").Optional().Then(Keyword("native")).Then(FunctionDeclarationParser)
                .Select<IDeclaration>(functionDeclaration => new JassNativeFunctionDeclarationSyntax(functionDeclaration));
        }
    }
}