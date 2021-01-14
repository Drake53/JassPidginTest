// ------------------------------------------------------------------------------
// <copyright file="JassParameterListSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassParameterListSyntax : IEquatable<JassParameterListSyntax>
    {
        public static readonly JassParameterListSyntax Empty = new JassParameterListSyntax();

        public JassParameterListSyntax(ImmutableArray<JassParameterSyntax> parameters)
        {
            Parameters = parameters;
        }

        public JassParameterListSyntax(params JassParameterSyntax[] parameters)
        {
            Parameters = parameters.ToImmutableArray();
        }

        public ImmutableArray<JassParameterSyntax> Parameters { get; init; }

        public bool Equals(JassParameterListSyntax? other) => other is not null && Parameters.SequenceEqual(other.Parameters);
    }
}