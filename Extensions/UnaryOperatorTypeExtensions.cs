﻿// ------------------------------------------------------------------------------
// <copyright file="UnaryOperatorTypeExtensions.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using War3Net.CodeAnalysis.Jass.Syntax;

namespace War3Net.CodeAnalysis.Jass.Extensions
{
    public static class UnaryOperatorTypeExtensions
    {
        public static string GetString(this UnaryOperatorType unaryOperator)
        {
            return unaryOperator switch
            {
                UnaryOperatorType.Plus => "+",
                UnaryOperatorType.Minus => "-",
                UnaryOperatorType.Not => "not",
            };
        }
    }
}