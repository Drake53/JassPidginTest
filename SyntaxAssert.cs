// ------------------------------------------------------------------------------
// <copyright file="SyntaxAssert.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

#nullable enable

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pidgin;

using War3Net.CodeAnalysis.Jass.Syntax;

namespace War3Net.CodeAnalysis.Jass.Tests
{
    public static class SyntaxAssert
    {
        public static void AreEqual(IExpressionSyntax? expected, IExpressionSyntax? actual)
        {
            Assert.IsTrue(ReferenceEquals(expected, actual) || expected?.Equals(actual) == true, GetAssertFailedMessage(expected, actual));
        }

        public static void ExpressionThrowsException(string expression, bool useTokenParser)
        {
            var message = new BoxedString();
            if (useTokenParser)
            {
                try
                {
                    message.String = GetSyntaxDisplayString(JassPidginTokenParser.ParseExpression(expression));
                }
                catch (Exception e) when (e is not ParseException && e is not InvalidDataException) // InvalidDataException is thrown by tokenizer.
                {
                    Assert.Fail("\r\n{0}", message);
                }
                catch { }
            }
            else
            {
                Assert.ThrowsException<ParseException>(() => message.String = GetSyntaxDisplayString(JassPidginParser.ParseExpression(expression)), "\r\n{0}", message);
            }
        }

        private static string GetAssertFailedMessage(object? expected, object? actual)
        {
            var expectedString = expected?.ToString();
            var actualString = actual?.ToString();
            var expectedType = expected?.GetType().Name ?? "null";
            var actualType = actual?.GetType().Name ?? "null";
            var isStringCorrect = string.Equals(expectedString, actualString, StringComparison.Ordinal);
            var isTypeCorrect = string.Equals(expectedType, actualType, StringComparison.Ordinal);
            return isStringCorrect == isTypeCorrect
                ? $"\r\nExpected: '{expectedString}'<{expectedType}>.\r\n  Actual: '{actualString}'<{actualType}>"
                : isStringCorrect
                    ? $"\r\nExpected: <{expectedType}>.\r\n  Actual: <{actualType}>."
                    : $"\r\nExpected: '{expectedString}'.\r\n  Actual: '{actualString}'.";
        }

        private static string GetSyntaxDisplayString(object? obj)
        {
            return obj is null ? "<null>" : $"'{obj}'<{obj.GetType().Name}>";
        }

        private class BoxedString
        {
            public string String { get; set; }

            public override string ToString() => String;
        }
    }
}