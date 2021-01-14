// ------------------------------------------------------------------------------
// <copyright file="JassCommentSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassCommentSyntax : IEquatable<JassCommentSyntax>
    {
        public JassCommentSyntax(string comment)
        {
            Comment = comment;
        }

        public string Comment { get; init; }

        public bool Equals(JassCommentSyntax? other) => other is not null && string.Equals(Comment, other.Comment, StringComparison.Ordinal);
    }
}