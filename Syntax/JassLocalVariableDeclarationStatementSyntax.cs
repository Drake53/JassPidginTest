﻿// ------------------------------------------------------------------------------
// <copyright file="JassLocalVariableDeclarationStatementSyntax.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassLocalVariableDeclarationStatementSyntax : IStatementSyntax, ICustomScriptAction
    {
        public JassLocalVariableDeclarationStatementSyntax(IVariableDeclarator declarator)
        {
            Declarator = declarator;
        }

        public IVariableDeclarator Declarator { get; init; }

        public bool Equals(IStatementSyntax? other) => other is JassLocalVariableDeclarationStatementSyntax s && Declarator.Equals(s.Declarator);

        public bool Equals(ICustomScriptAction? other) => other is JassLocalVariableDeclarationStatementSyntax s && Declarator.Equals(s.Declarator);
    }
}