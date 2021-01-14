// ------------------------------------------------------------------------------
// <copyright file="JassFunctionSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassFunctionSyntax : IEquatable<JassFunctionSyntax>
    {
        public JassFunctionSyntax(JassFunctionDeclarationSyntax declaration, JassStatementListSyntax body)
        {
            Declaration = declaration;
            Body = body;
        }

        public JassFunctionDeclarationSyntax Declaration { get; init; }

        public JassStatementListSyntax Body { get; init; }

        public bool Equals(JassFunctionSyntax? other) => other is not null && Declaration.Equals(other.Declaration) && Body.Equals(other.Body);
    }
}