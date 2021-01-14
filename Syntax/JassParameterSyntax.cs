// ------------------------------------------------------------------------------
// <copyright file="JassParameterSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassParameterSyntax : IEquatable<JassParameterSyntax>
    {
        public JassParameterSyntax(JassTypeSyntax type, JassIdentifierNameSyntax identifierName)
        {
            Type = type;
            IdentifierName = identifierName;
        }

        public JassParameterSyntax(string type, string name)
        {
            Type = new JassTypeSyntax(type);
            IdentifierName = new JassIdentifierNameSyntax(name);
        }

        public JassTypeSyntax Type { get; init; }

        public JassIdentifierNameSyntax IdentifierName { get; init; }

        public bool Equals(JassParameterSyntax? other) => other is not null && Type.Equals(other.Type) && IdentifierName.Equals(other.IdentifierName);
    }
}