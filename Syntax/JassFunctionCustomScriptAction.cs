// ------------------------------------------------------------------------------
// <copyright file="JassFunctionCustomScriptAction.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public class JassFunctionCustomScriptAction : ICustomScriptAction
    {
        public JassFunctionCustomScriptAction(JassFunctionDeclarationSyntax functionDeclaration)
        {
            FunctionDeclaration = functionDeclaration;
        }

        public JassFunctionDeclarationSyntax FunctionDeclaration { get; init; }

        public bool Equals(ICustomScriptAction? other) => other is JassFunctionCustomScriptAction d && FunctionDeclaration.Equals(d.FunctionDeclaration);
    }
}