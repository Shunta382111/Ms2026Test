using UnityEngine;
using Unity.VisualScripting;
using System;
using System.Diagnostics;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    public static class DebugEx 
    {

        [Conditional("UNITY_EDITOR")]
        public static void Log(object msg) => UnityEngine.Debug.Log(msg);
        [Conditional("UNITY_EDITOR")]
        public static void LogError(object msg) => UnityEngine.Debug.LogError(msg);
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object msg) => UnityEngine.Debug.LogWarning(msg);

        /// <summary>
        /// Nullではないかチェック
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertSome<T>(T obj, object msg) where T : class 
            => TryAssertion(obj != null, msg);

        /// <summary>
        /// Nullなのかチェック
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertNull<T>(T obj, object msg) where T : class 
            => TryAssertion(obj == null, msg);

        /// <summary>
        /// 等比較
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertEq<T>(T lhs, T rhs, object msg)
        => TryAssertion(Equals(lhs, rhs), $"{msg} : {lhs} != {rhs}");

        /// <summary>
        /// 不等比較
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertNe<T>(T lhs, T rhs, object msg)
            => TryAssertion(!Equals(lhs, rhs), $"{msg} : {lhs} == {rhs}");

        /// <summary>
        /// 真偽値チェック
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertTrue(bool condition, object msg)
            => TryAssertion(condition, msg);

        /// <summary>
        /// 真偽値チェック
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertFalse(bool condition, object msg)
            => TryAssertion(!condition, msg);

        /// <summary>
        /// <= 大小比較　[IComparable制約]
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertGt<T>(T lhs, T rhs, object msg) where T : IComparable<T>
            => TryAssertion(lhs.CompareTo(rhs) > 0, $"{msg} : {lhs} <= {rhs}");

        /// <summary>
        /// >= 大小比較　[IComparable制約]
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertLt<T>(T lhs, T rhs, object msg) where T : IComparable<T>
            => TryAssertion(lhs.CompareTo(rhs) < 0, $"{msg} : {lhs} >= {rhs}");

        /// <summary>
        /// < 大小比較　[IComparable制約]
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertGe<T>(T lhs, T rhs, object msg) where T : IComparable<T>
            => TryAssertion(lhs.CompareTo(rhs) >= 0, $"{msg} : {lhs} < {rhs}");

        /// <summary>
        /// > 大小比較　[IComparable制約]
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void AssertLe<T>(T lhs, T rhs, object msg) where T : IComparable<T>
            => TryAssertion(lhs.CompareTo(rhs) <= 0, $"{msg} : {lhs} > {rhs}");

        /// <summary>
        /// Nullではないかチェック
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectSome<T>(T obj, object msg) where T : class 
            => TryExpect(obj != null, msg);

        /// <summary>
        /// Nullなのかチェック
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectNull<T>(T obj, object msg) where T : class 
            => TryExpect(obj == null, msg);

        /// <summary>
        /// 等比較
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectEq<T>(T lhs, T rhs, object msg)
        => TryExpect(Equals(lhs, rhs), $"{msg} : {lhs} != {rhs}");

        /// <summary>
        /// 不等比較
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectNe<T>(T lhs, T rhs, object msg)
            => TryExpect(!Equals(lhs, rhs), $"{msg} : {lhs} == {rhs}");

        /// <summary>
        /// 真偽値チェック
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectTrue(bool condition, object msg)
            => TryExpect(condition, msg);

        /// <summary>
        /// 真偽値チェック
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectFalse(bool condition, object msg)
            => TryExpect(!condition, msg);

        /// <summary>
        /// <= 大小比較　[IComparable制約]
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectGt<T>(T lhs, T rhs, object msg) where T : IComparable<T>
            => TryExpect(lhs.CompareTo(rhs) > 0, $"{msg} : {lhs} <= {rhs}");

        /// <summary>
        /// >= 大小比較　[IComparable制約]
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectLt<T>(T lhs, T rhs, object msg) where T : IComparable<T>
            => TryExpect(lhs.CompareTo(rhs) < 0, $"{msg} : {lhs} >= {rhs}");

        /// <summary>
        /// < 大小比較　[IComparable制約]
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectGe<T>(T lhs, T rhs, object msg) where T : IComparable<T>
            => TryExpect(lhs.CompareTo(rhs) >= 0, $"{msg} : {lhs} < {rhs}");

        /// <summary>
        /// > 大小比較　[IComparable制約]
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ExpectLe<T>(T lhs, T rhs, object msg) where T : IComparable<T>
            => TryExpect(lhs.CompareTo(rhs) <= 0, $"{msg} : {lhs} > {rhs}");

        /// <summary>
        /// アサートを出す
        /// </summary>
        /// <param name="msg"></param>
        [Conditional("UNITY_EDITOR")]
        public static void Assertion(object msg)
        {
            LogError(msg);
            Exit();
        }





        [Conditional("UNITY_EDITOR")]
        private static void TryExpect(bool isExpect,object msg)
        {
            if (isExpect) return;
            LogError(msg);
        }


        [Conditional("UNITY_EDITOR")]
        private static void TryAssertion(bool isExpect,object msg)
        {
            if (isExpect) return;
            Assertion(msg);
        }

        [Conditional("UNITY_EDITOR")]
        private static void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
