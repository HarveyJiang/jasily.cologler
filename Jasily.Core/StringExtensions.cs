﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        #region encoding & decoding

        /// <summary>
        /// return UTF-8 encoding
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// get bytes using special encoding
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string text, Encoding encoding) => encoding.GetBytes(text);

        /// <summary>
        /// get bytes using DefaultEncoding
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string text) => text.GetBytes(DefaultEncoding);

        public static string UrlEncode(this string str) => Net.WebUtility.UrlEncode(str);

        public static string UrlDecode(this string str) => Net.WebUtility.UrlDecode(str);

        #endregion

        #region is

        /// <summary>
        /// return String.IsNullOrEmpty(text)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string text) => String.IsNullOrEmpty(text);

        /// <summary>
        /// return String.IsNullOrWhiteSpace(text)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string text) => String.IsNullOrWhiteSpace(text);

        #endregion

        #region throw

        /// <summary>
        /// throw if current text is null or empty.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ThrowIfNullOrEmpty(this string text, string paramName, string message = null)
        {
            if (text == null)
            {
                if (message == null)
                    throw new ArgumentNullException(paramName);
                else
                    throw new ArgumentNullException(paramName, message);
            }
            else if (text.Length == 0)
            {
                if (message == null)
                    throw new ArgumentException(paramName);
                else
                    throw new ArgumentException(paramName, message);
            }


            return text;
        }

        #endregion

        #region split

        public static string[] Split(this string text, string separator, StringSplitOptions options = StringSplitOptions.None)
        {
            return text.Split(new[] { separator }, options);
        }

        public static string[] Split(this string text, string separator, int count, StringSplitOptions options = StringSplitOptions.None)
        {
            return text.Split(new [] { separator }, count, options);
        }

        #endregion

        #region parse

        public static int? TryParseInt32(string s)
        {
            int n;
            return int.TryParse(s, out n) ? (int?) n : null;
        }
        public static int? TryParseInt32(string s, NumberStyles style, IFormatProvider provider)
        {
            int n;
            return int.TryParse(s, style, provider, out n) ? (int?)n : null;
        }

        public static long? TryParseInt64(string s)
        {
            long n;
            return long.TryParse(s, out n) ? (long?)n : null;
        }
        public static long? TryParseInt64(string s, NumberStyles style, IFormatProvider provider)
        {
            long n;
            return long.TryParse(s, style, provider, out n) ? (long?)n : null;
        }

        public static double? TryParseDouble(string s)
        {
            double n;
            return double.TryParse(s, out n) ? (long?)n : null;
        }
        public static double? TryParseDouble(string s, NumberStyles style, IFormatProvider provider)
        {
            double n;
            return double.TryParse(s, style, provider, out n) ? (double?)n : null;
        }

        #endregion

        #region other

        /// <summary>
        /// use spliter to join texts. default value was '\r\n'
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static string AsLines(this IEnumerable<string> texts, string spliter = null)
            => String.Join(spliter ?? Environment.NewLine, texts);

        /// <summary>
        /// use spliter to split text. default value was '\r\n' or '\r' or '\n'
        /// </summary>
        /// <param name="text"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static IEnumerable<string> AsLines(this string text, string spliter = null)
            => spliter == null
            ? text?.Split(new [] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
            : text?.Split(new [] { spliter }, StringSplitOptions.None);

        /// <summary>
        /// repeat this like ( string * int ) in python
        /// </summary>
        /// <param name="str"></param>
        /// <param name="count"></param>
        /// <exception cref="System.ArgumentOutOfRangeException">count &lt; 0</exception>
        /// <returns></returns>
        public static string Repeat(this string str, int count)
            => str == null ? null : string.Concat(Enumerable.Repeat(str, count));

        /// <summary>
        /// return first line from text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FirstLine(this string text)
        {
            if (text == null)
                return null;
            
            var index = text.IndexOf('\n');

            if (index == -1)
            {
                return text;
            }
            else
            {
                if (index > 0 && text[index - 1] == '\r')
                {
                    return text.Substring(0, index - 1);
                }
                else
                {
                    return text.Substring(0, index);
                }
            }
        }

        #endregion

        public static string TrimStart(this string str, params string[] trimStrings)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (trimStrings == null || trimStrings.Length == 0)
                return str;

            if (trimStrings.Any(z => z.IsNullOrEmpty()))
                throw new ArgumentException();

            string start;
            while ((start = trimStrings.FirstOrDefault(z => str.StartsWith(z))) != null)
            {
                str = str.Substring(start.Length);
            }
            return str;
        }

        public static string AfterFirst(this string str, string spliter)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (spliter == null) throw new ArgumentNullException(nameof(spliter));

            var index = str.IndexOf(spliter, StringComparison.Ordinal);
            return index < 1 ? str : str.Substring(index + 1);
        }
        public static string AfterFirst(this string str, char spliter)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var index = str.IndexOf(spliter);
            return index < 1 ? str : str.Substring(index + 1);
        }
        public static string AfterFirst(this string str, params char[] spliter)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var index = str.LastIndexOfAny(spliter);
            return index < 1 ? str : str.Substring(index + 1);
        }

        public static string AfterLast(this string str, string spliter)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var index = str.LastIndexOf(spliter);
            return index < 1 ? str : str.Substring(index + 1);
        }
        public static string AfterLast(this string str, char spliter)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var index = str.LastIndexOf(spliter);
            return index < 1 ? str : str.Substring(index + 1);
        }
        public static string AfterLast(this string str, params char[] spliter)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var index = str.LastIndexOfAny(spliter);
            return index < 1 ? str : str.Substring(index + 1);
        }

        public static string Childs(this string str, int? leftIndex = null, int? rightIndex = null)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            return (new JasilyStringChild()).Result(str, leftIndex, rightIndex);
        }

        private struct JasilyStringChild : IChild<string, string>
        {
            public int Count(string source) => source.Length;

            public string Empty => string.Empty;

            public string GetChilds(string source, int leftIndex, int rightIndex)
                => source.Substring(leftIndex, rightIndex - leftIndex);
        }
    }

    internal interface IChild<TSource, TResult>
    {
        int Count(TSource source);

        TResult Empty { get; }

        TResult GetChilds(TSource source, int leftIndex, int rightIndex);
    }

    internal static class IChildExtensions
    {
        public static TResult Result<TSource, TResult>(this IChild<TSource, TResult> child,
            TSource source, int? leftIndex, int? rightIndex)
        {
            var li = child.Count(source) - 1;
            if (li == -1) return child.Empty;

            var l = leftIndex ?? 0;
            if (l < 0) l = li + l;
            if (l < 0) l = 0;
            if (l > li) return child.Empty;

            var r = rightIndex ?? li;
            if (r < 0) r = li + r;
            if (r < 0) r = 0;
            if (l >= r) return child.Empty;

            return child.GetChilds(source, l, r);
        }
    }
}
