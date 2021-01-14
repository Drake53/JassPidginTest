// ------------------------------------------------------------------------------
// <copyright file="JassVariableDeclaratorSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassVariableDeclaratorSyntax : IVariableDeclarator
    {
        public JassVariableDeclaratorSyntax(JassTypeSyntax type, JassIdentifierNameSyntax identifierName, JassEqualsValueClauseSyntax? value)
        {
            Type = type;
            IdentifierName = identifierName;
            Value = value;
        }

        public JassTypeSyntax Type { get; init; }

        public JassIdentifierNameSyntax IdentifierName { get; init; }

        public JassEqualsValueClauseSyntax? Value { get; init; }

        public bool Equals(IVariableDeclarator? other) => other is JassVariableDeclaratorSyntax v && Type.Equals(v.Type) && IdentifierName.Equals(v.IdentifierName) && (ReferenceEquals(Value, v.Value) || Value?.Equals(v.Value) == true);
    }
}