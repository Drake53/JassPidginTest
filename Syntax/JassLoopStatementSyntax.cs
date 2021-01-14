// ------------------------------------------------------------------------------
// <copyright file="JassLoopStatementSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassLoopStatementSyntax : IStatementSyntax
    {
        public JassLoopStatementSyntax(JassStatementListSyntax body)
        {
            Body = body;
        }

        public JassLoopStatementSyntax(params IStatementSyntax[] body)
        {
            Body = new JassStatementListSyntax(body);
        }

        public JassStatementListSyntax Body { get; init; }

        public bool Equals(IStatementSyntax? other) => other is JassLoopStatementSyntax s && Body.Equals(s.Body);
    }
}