// ------------------------------------------------------------------------------
// <copyright file="JassElseIfStatementSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassElseIfStatementSyntax
    {
        public JassElseIfStatementSyntax(IExpressionSyntax condition, JassStatementListSyntax body)
        {
            Condition = condition;
            Body = body;
        }

        public IExpressionSyntax Condition { get; init; }

        public JassStatementListSyntax Body { get; init; }

        public bool Equals(JassElseIfStatementSyntax? other) => other is not null && Condition.Equals(other.Condition) && Body.Equals(other.Body);
    }
}