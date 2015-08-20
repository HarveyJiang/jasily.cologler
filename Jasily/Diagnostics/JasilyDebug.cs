﻿using System;
using System.Diagnostics;

namespace Jasily.Diagnostics
{
    public static class JasilyDebug
    {
        [Conditional("DEBUG")]
        public static void Assert(Func<bool> condition)
        {
            Debug.Assert(condition());
        }
        [Conditional("DEBUG")]
        public static void Assert(Func<bool> condition, string message)
        {
            Debug.Assert(condition(), message);
        }
        [Conditional("DEBUG")]
        public static void Assert(Func<bool> condition, Func<string> message)
        {
            Debug.Assert(condition(), message());
        }

        [Conditional("DEBUG")]
        public static void WriteLine(Func<string> message)
        {
            Debug.WriteLine(message());
        }
        [Conditional("DEBUG")]
        public static void WriteLine(Func<object> value)
        {
            Debug.WriteLine(value());
        }

        [Conditional("DEBUG")]
        public static void WriteLineIf(Func<bool> condition, string message)
        {
            Debug.WriteLineIf(condition(), message);
        }
        [Conditional("DEBUG")]
        public static void WriteLineIf(Func<bool> condition, Func<string> message)
        {
            Debug.WriteLineIf(condition(), message());
        }

        #region assert type

        [Conditional("DEBUG")]
        public static void AssertType<T>(this object obj)
        {
            AssertType<T>(obj.GetType());
        }
        [Conditional("DEBUG")]
        public static void AssertType<T>(this Type type)
        {
            Debug.Assert(type == typeof(T), $"assert type failed. {type.FullName} not {typeof(T).FullName}");
        }

        #endregion
    }
}