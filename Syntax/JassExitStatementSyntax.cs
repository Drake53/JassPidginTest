// ------------------------------------------------------------------------------
// <copyright file="JassExitStatementSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassExitStatementSyntax : IStatementSyntax, ICustomScriptAction
    {
        public JassExitStatementSyntax(IExpressionSyntax condition)
        {
            Condition = condition;
        }

        public IExpressionSyntax Condition { get; init; }

        public bool Equals(IStatementSyntax? other) => other is JassExitStatementSyntax s && Condition.Equals(s.Condition);

        public bool Equals(ICustomScriptAction? other) => other is JassExitStatementSyntax s && Condition.Equals(s.Condition);
    }
}