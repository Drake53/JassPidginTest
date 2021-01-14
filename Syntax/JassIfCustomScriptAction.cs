// ------------------------------------------------------------------------------
// <copyright file="JassIfCustomScriptAction.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassIfCustomScriptAction : ICustomScriptAction
    {
        public JassIfCustomScriptAction(IExpressionSyntax condition)
        {
            Condition = condition;
        }

        public IExpressionSyntax Condition { get; init; }

        public bool Equals(ICustomScriptAction? other) => other is JassIfCustomScriptAction s && Condition.Equals(s.Condition);
    }
}