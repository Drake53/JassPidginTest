// ------------------------------------------------------------------------------
// <copyright file="JassIfStatementSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassIfStatementSyntax : IStatementSyntax
    {
        public JassIfStatementSyntax(IExpressionSyntax condition, JassStatementListSyntax body, ImmutableArray<JassElseIfStatementSyntax> elseIfStatements, JassElseClauseSyntax? elseClause)
        {
            Condition = condition;
            Body = body;
            ElseIfStatements = elseIfStatements;
            ElseClause = elseClause;
        }

        public JassIfStatementSyntax(IExpressionSyntax condition, params IStatementSyntax[] body)
        {
            Condition = condition;
            Body = new JassStatementListSyntax(body);
            ElseIfStatements = ImmutableArray.Create<JassElseIfStatementSyntax>();
            ElseClause = null;
        }

        public JassIfStatementSyntax(IExpressionSyntax condition, JassStatementListSyntax body)
        {
            Condition = condition;
            Body = body;
            ElseIfStatements = ImmutableArray.Create<JassElseIfStatementSyntax>();
            ElseClause = null;
        }

        public JassIfStatementSyntax(IExpressionSyntax condition, JassStatementListSyntax body, params JassElseIfStatementSyntax[] elseIfStatements)
        {
            Condition = condition;
            Body = body;
            ElseIfStatements = elseIfStatements.ToImmutableArray();
            ElseClause = null;
        }

        public IExpressionSyntax Condition { get; init; }

        public JassStatementListSyntax Body { get; init; }

        public ImmutableArray<JassElseIfStatementSyntax> ElseIfStatements { get; init; }

        public JassElseClauseSyntax? ElseClause { get; init; }

        public bool Equals(IStatementSyntax? other) => other is JassIfStatementSyntax s && Condition.Equals(s.Condition) && Body.Equals(s.Body) && ElseIfStatements.SequenceEqual(s.ElseIfStatements) && (ReferenceEquals(ElseClause, s.ElseClause) || ElseClause?.Equals(s.ElseClause) == true);
    }
}