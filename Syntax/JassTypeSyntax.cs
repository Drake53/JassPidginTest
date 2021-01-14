// ------------------------------------------------------------------------------
// <copyright file="JassTypeSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassTypeSyntax : IEquatable<JassTypeSyntax>
    {
        public static readonly JassTypeSyntax Code = new JassTypeSyntax("code");
        public static readonly JassTypeSyntax Handle = new JassTypeSyntax("handle");
        public static readonly JassTypeSyntax Integer = new JassTypeSyntax("integer");
        public static readonly JassTypeSyntax Real = new JassTypeSyntax("real");
        public static readonly JassTypeSyntax Boolean = new JassTypeSyntax("boolean");
        public static readonly JassTypeSyntax String = new JassTypeSyntax("string");
        public static readonly JassTypeSyntax Nothing = new JassTypeSyntax("");

        public JassTypeSyntax(string type)
        {
            Type = type;
        }

        public string Type { get; init; }

        public bool Equals(JassTypeSyntax? other) => other is not null && string.Equals(Type, other.Type, StringComparison.Ordinal);
    }
}