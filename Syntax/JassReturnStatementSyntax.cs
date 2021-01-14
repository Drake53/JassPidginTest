// ------------------------------------------------------------------------------
// <copyright file="JassReturnStatementSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassReturnStatementSyntax : IStatementSyntax, ICustomScriptAction
    {
        public JassReturnStatementSyntax(IExpressionSyntax? value = null)
        {
            Value = value;
        }

        public IExpressionSyntax? Value { get; init; }

        public bool Equals(IStatementSyntax? other) => other is JassReturnStatementSyntax s && (ReferenceEquals(Value, s.Value) || Value?.Equals(s.Value) == true);

        public bool Equals(ICustomScriptAction? other) => other is JassReturnStatementSyntax s && (ReferenceEquals(Value, s.Value) || Value?.Equals(s.Value) == true);
    }
}