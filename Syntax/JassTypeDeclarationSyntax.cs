// ------------------------------------------------------------------------------
// <copyright file="JassTypeDeclarationSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassTypeDeclarationSyntax : IDeclaration
    {
        public JassTypeDeclarationSyntax(JassIdentifierNameSyntax identifierName, JassTypeSyntax baseType)
        {
            IdentifierName = identifierName;
            BaseType = baseType;
        }

        public JassIdentifierNameSyntax IdentifierName { get; init; }

        public JassTypeSyntax BaseType { get; init; }

        public bool Equals(IDeclaration? other) => other is JassTypeDeclarationSyntax d && IdentifierName.Equals(d.IdentifierName) && BaseType.Equals(d.BaseType);
    }
}