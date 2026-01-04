using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CocaCopa.Core.Collections {
    public static class ListExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> EntriesFromRight<T>(this List<T> list, int count) {
            if (list == null || list.Count == 0 || count <= 0) return new List<T>(0);
            if (count >= list.Count) return new List<T>(list);
            int start = list.Count - count;
            return list.GetRange(start, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> EntriesFromLeft<T>(this List<T> list, int count) {
            if (list == null || list.Count == 0 || count <= 0) return new List<T>(0);
            if (count >= list.Count) return new List<T>(list);
            return list.GetRange(0, count);
        }

        public static List<T> Combine<T>(this IEnumerable<T> first, params IEnumerable<T>[] others) {
            int capacity = 0;
            if (first is ICollection<T> c1) capacity += c1.Count;
            if (others != null)
                for (int i = 0; i < others.Length; i++)
                    if (others[i] is ICollection<T> ci) capacity += ci.Count;

            var result = capacity > 0 ? new List<T>(capacity) : new List<T>();

            AddAll(result, first);
            if (others != null)
                for (int i = 0; i < others.Length; i++)
                    AddAll(result, others[i]);

            return result;

            static void AddAll(List<T> dst, IEnumerable<T> src) {
                if (src == null) return;

                // Fast paths first
                if (src is List<T> l) { dst.AddRange(l); return; }
                if (src is T[] a) {
                    // Avoid AddRange(a) which enumerates; copy with index for arrays
                    for (int i = 0; i < a.Length; i++) dst.Add(a[i]);
                    return;
                }
                if (src is IReadOnlyList<T> rl) {
                    for (int i = 0; i < rl.Count; i++) dst.Add(rl[i]);
                    return;
                }

                // Fallback enumeration (no LINQ)
                foreach (var item in src) dst.Add(item);
            }
        }

        // ---- IsValid overloads (hot paths) ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<T>(this List<T> list) =>
            list != null && list.Count != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<T>(this T[] array) =>
            array != null && array.Length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<TKey, TValue>(this Dictionary<TKey, TValue> dict) =>
            dict != null && dict.Count != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<T>(this ICollection<T> col) =>
            col != null && col.Count != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<TKey, TValue>(this IDictionary<TKey, TValue> dict) =>
            dict != null && dict.Count != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<T>(this IReadOnlyCollection<T> col) =>
            col != null && col.Count != 0;

        // ---- Fallback for anything else enumerable (minimize boxing) ----
        public static bool IsValid<T>(this IEnumerable<T> seq) {
            if (seq == null) return false;

            // Explicit fast paths to avoid boxed enumerators
            if (seq is List<T> l) return l.Count != 0;
            if (seq is T[] a) return a.Length != 0;

            if (seq is ICollection<T> c) return c.Count != 0;
            if (seq is IReadOnlyCollection<T> rc) return rc.Count != 0;

            // Last resort: peek first element (may box if enumerator is a struct; rare path)
            using var e = seq.GetEnumerator();
            return e.MoveNext();
        }
    }
}
