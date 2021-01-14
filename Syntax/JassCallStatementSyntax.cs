// ------------------------------------------------------------------------------
// <copyright file="JassCallStatementSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassCallStatementSyntax : IStatementSyntax, ICustomScriptAction
    {
        public JassCallStatementSyntax(JassIdentifierNameSyntax identifierName, JassArgumentListSyntax arguments)
        {
            IdentifierName = identifierName;
            Arguments = arguments;
        }

        public JassCallStatementSyntax(string name, params IExpressionSyntax[] arguments)
        {
            IdentifierName = new JassIdentifierNameSyntax(name);
            Arguments = new JassArgumentListSyntax(arguments);
        }

        public JassIdentifierNameSyntax IdentifierName { get; init; }

        public JassArgumentListSyntax Arguments { get; init; }

        public bool Equals(IStatementSyntax? other) => other is JassCallStatementSyntax s && IdentifierName.Equals(s.IdentifierName) && Arguments.Equals(s.Arguments);

        public bool Equals(ICustomScriptAction? other) => other is JassCallStatementSyntax s && IdentifierName.Equals(s.IdentifierName) && Arguments.Equals(s.Arguments);
    }
}