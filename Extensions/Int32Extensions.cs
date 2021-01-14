// ------------------------------------------------------------------------------
// <copyright file="Int32Extensions.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Linq;

using War3Net.Common.Extensions;

namespace War3Net.Common.Extensions
{
    public static class Int32Extensions
    {
        public static string ToRawcode(this int value)
        {
            return new string(new[]
            {
                (char)(value & 0x000000FF),
                (char)((value & 0x0000FF00) >> 8),
                (char)((value & 0x00FF0000) >> 16),
                (char)((value & 0xFF000000) >> 24),
            });
        }
    }
}
namespace War3Net.CodeAnalysis.Jass.Extensions
{
    public static class Int32Extensions
    {
        public static bool TryParseOctal(this string s, out int result)
        {
            try
            {
                result = Convert.ToInt32(s, 8);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static bool TryParseHexadecimal(this string s, out int result)
        {
            try
            {
                result = Convert.ToInt32(s, 16);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static bool TryFromRawcode(this string s, out int result)
        {
            if (s.Length != 4 || s.Any(@char => @char == '\n'))
            {
                result = default;
                return false;
            }

            try
            {
                result = s.FromRawcode();
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}