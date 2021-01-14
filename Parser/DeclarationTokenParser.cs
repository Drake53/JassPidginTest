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
        private static Parser<SyntaxToken, JassGlobalDeclarationSyntax> GetGlobalDeclarationParser()
        {
            return Keyword(SyntaxTokenType.ConstantKeyword).Then(Map((type, id, value) => (IVariableDeclarator)new JassVariableDeclaratorSyntax(type, id, value), TypeParser, IdentifierNameParser, EqualsValueClauseParser))
                .Or(VariableDeclaratorParser)
                .Before(Newline.AtLeastOnce())
                .Select(declarator => new JassGlobalDeclarationSyntax(declarator));
        }

        private static Parser<SyntaxToken, IDeclaration> GetTypeDeclarationParser()
        {
            return Keyword(SyntaxTokenType.TypeKeyword).Then(IdentifierNameParser).Then(
                Keyword(SyntaxTokenType.ExtendsKeyword).Then(ReferenceTypeParser),
                (@new, @base) => (IDeclaration)new JassTypeDeclarationSyntax(@new, @base));
        }

        private static Parser<SyntaxToken, IDeclaration> GetGlobalDeclarationListParser()
        {
            return Keyword(SyntaxTokenType.GlobalsKeyword).Then(Newline.AtLeastOnce()).Then(GlobalDeclarationParser.Many()).Before(Keyword(SyntaxTokenType.EndglobalsKeyword))
                .Select<IDeclaration>(globals => new JassGlobalDeclarationListSyntax(globals.ToImmutableArray()));
        }

        private static Parser<SyntaxToken, IDeclaration> GetNativeFunctionDeclarationParser()
        {
            return Keyword(SyntaxTokenType.ConstantKeyword).Optional().Then(Keyword(SyntaxTokenType.NativeKeyword)).Then(FunctionDeclarationParser)
                .Select<IDeclaration>(functionDeclaration => new JassNativeFunctionDeclarationSyntax(functionDeclaration));
        }
    }
}