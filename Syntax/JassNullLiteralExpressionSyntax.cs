﻿// ------------------------------------------------------------------------------
// <copyright file="JassNullLiteralExpressionSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassNullLiteralExpressionSyntax : IExpressionSyntax
    {
        public static readonly JassNullLiteralExpressionSyntax Null = new JassNullLiteralExpressionSyntax();

        private JassNullLiteralExpressionSyntax()
        {
        }

        public bool Equals(IExpressionSyntax? other) => other is JassNullLiteralExpressionSyntax;

        public override string ToString() => "null";
    }
}