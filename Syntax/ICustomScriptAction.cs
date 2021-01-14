// ------------------------------------------------------------------------------
// <copyright file="ICustomScriptAction.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;

namespace War3Net.CodeAnalysis.Jass.Syntax
{
    public interface ICustomScriptAction : IEquatable<ICustomScriptAction>
    {
    }
    public interface IDeclaration : IEquatable<IDeclaration>
    {
    }
    public class JassGlobalDeclarationListSyntax : IDeclaration
    {
        public JassGlobalDeclarationListSyntax(ImmutableArray<JassGlobalDeclarationSyntax> globals)
        {
            Globals = globals;
        }

        public ImmutableArray<JassGlobalDeclarationSyntax> Globals { get; init; }

        public bool Equals(IDeclaration? other) => other is JassGlobalDeclarationListSyntax d && Globals.SequenceEqual(d.Globals);
    }
    public class JassGlobalDeclarationSyntax : IEquatable<JassGlobalDeclarationSyntax>
    {
        public JassGlobalDeclarationSyntax(IVariableDeclarator declarator)
        {
            Declarator = declarator;
        }

        public IVariableDeclarator Declarator { get; init; }

        public bool Equals(JassGlobalDeclarationSyntax? other) => other is not null && Declarator.Equals(other.Declarator);
    }
    public class JassNativeFunctionDeclarationSyntax : IDeclaration
    {
        public JassNativeFunctionDeclarationSyntax(JassFunctionDeclarationSyntax functionDeclaration)
        {
            FunctionDeclaration = functionDeclaration;
        }

        public JassFunctionDeclarationSyntax FunctionDeclaration { get; init; }

        public bool Equals(IDeclaration? other) => other is JassNativeFunctionDeclarationSyntax d && FunctionDeclaration.Equals(d.FunctionDeclaration);
    }
    public class JassCompilationUnitSyntax : IEquatable<JassCompilationUnitSyntax>
    {
        public JassCompilationUnitSyntax(ImmutableArray<IDeclaration> declarations, ImmutableArray<JassFunctionSyntax> functions)
        {
            Declarations = declarations;
            Functions = functions;
        }

        public ImmutableArray<IDeclaration> Declarations { get; init; }

        public ImmutableArray<JassFunctionSyntax> Functions { get; init; }

        public bool Equals(JassCompilationUnitSyntax? other) => other is not null && Declarations.SequenceEqual(other.Declarations) && Functions.SequenceEqual(other.Functions);
    }
}