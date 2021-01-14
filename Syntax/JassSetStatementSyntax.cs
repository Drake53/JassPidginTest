// ------------------------------------------------------------------------------
// <copyright file="JassSetStatementSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassSetStatementSyntax : IStatementSyntax, ICustomScriptAction
    {
        public JassSetStatementSyntax(JassIdentifierNameSyntax identifierName, IExpressionSyntax? indexer, JassEqualsValueClauseSyntax value)
        {
            IdentifierName = identifierName;
            Indexer = indexer;
            Value = value;
        }

        public JassSetStatementSyntax(string name, IExpressionSyntax value)
        {
            IdentifierName = new JassIdentifierNameSyntax(name);
            Indexer = null;
            Value = new JassEqualsValueClauseSyntax(value);
        }

        public JassSetStatementSyntax(string name, IExpressionSyntax indexer, IExpressionSyntax value)
        {
            IdentifierName = new JassIdentifierNameSyntax(name);
            Indexer = indexer;
            Value = new JassEqualsValueClauseSyntax(value);
        }

        public JassIdentifierNameSyntax IdentifierName { get; init; }

        public IExpressionSyntax? Indexer { get; init; }

        public JassEqualsValueClauseSyntax Value { get; init; }

        public bool Equals(IStatementSyntax? other) => other is JassSetStatementSyntax s && IdentifierName.Equals(s.IdentifierName) && (ReferenceEquals(Indexer, s.Indexer) || Indexer?.Equals(s.Indexer) == true) && Value.Equals(s.Value);

        public bool Equals(ICustomScriptAction? other) => other is JassSetStatementSyntax s && IdentifierName.Equals(s.IdentifierName) && (ReferenceEquals(Indexer, s.Indexer) || Indexer?.Equals(s.Indexer) == true) && Value.Equals(s.Value);
    }
}