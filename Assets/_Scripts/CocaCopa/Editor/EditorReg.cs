using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.EditorUtils {
    public static class EditorReg {
        private const string Prefix = "CocaCopa.EditorReg";

        private static string MakeKey(Object target, string namePart) {
            if (target == null) {
                return $"{Prefix}.NULL.{namePart}";
            }

            var gid = GlobalObjectId.GetGlobalObjectIdSlow(target);
            return $"{Prefix}.{gid}.{namePart}";
        }

        /// <summary>
        /// Loads a boolean value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="defaultValue">The value returned if no preference exists.</param>
        /// <returns>The stored boolean value or the default value.</returns>
        public static bool Load(Object target, string namePart, bool defaultValue = false) {
            return EditorPrefs.GetBool(MakeKey(target, namePart), defaultValue);
        }

        /// <summary>
        /// Saves a boolean value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="value">The value to store.</param>
        public static void Save(Object target, string namePart, bool value) {
            EditorPrefs.SetBool(MakeKey(target, namePart), value);
        }

        /// <summary>
        /// Loads an integer value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="defaultValue">The value returned if no preference exists.</param>
        /// <returns>The stored integer value or the default value.</returns>
        public static int Load(Object target, string namePart, int defaultValue = 0) {
            return EditorPrefs.GetInt(MakeKey(target, namePart), defaultValue);
        }

        /// <summary>
        /// Saves an integer value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="value">The value to store.</param>
        public static void Save(Object target, string namePart, int value) {
            EditorPrefs.SetInt(MakeKey(target, namePart), value);
        }

        /// <summary>
        /// Loads a float value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="defaultValue">The value returned if no preference exists.</param>
        /// <returns>The stored float value or the default value.</returns>
        public static float Load(Object target, string namePart, float defaultValue = 0f) {
            return EditorPrefs.GetFloat(MakeKey(target, namePart), defaultValue);
        }

        /// <summary>
        /// Saves a float value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="value">The value to store.</param>
        public static void Save(Object target, string namePart, float value) {
            EditorPrefs.SetFloat(MakeKey(target, namePart), value);
        }

        /// <summary>
        /// Loads a string value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="defaultValue">The value returned if no preference exists.</param>
        /// <returns>The stored string value or the default value.</returns>
        public static string Load(Object target, string namePart, string defaultValue = "") {
            return EditorPrefs.GetString(MakeKey(target, namePart), defaultValue);
        }

        /// <summary>
        /// Saves a string value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="value">The value to store.</param>
        public static void Save(Object target, string namePart, string value) {
            EditorPrefs.SetString(MakeKey(target, namePart), value ?? string.Empty);
        }

        /// <summary>
        /// Loads a Vector3 value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="defaultValue">The value returned if no preference exists.</param>
        /// <returns>The stored Vector3 value or the default value.</returns>
        public static Vector3 Load(Object target, string namePart, Vector3 defaultValue) {
            var raw = EditorPrefs.GetString(MakeKey(target, namePart), string.Empty);
            if (string.IsNullOrEmpty(raw)) {
                return defaultValue;
            }

            var parts = raw.Split(',');
            if (parts.Length != 3) {
                return defaultValue;
            }

            if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y) &&
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z)) {
                return new Vector3(x, y, z);
            }

            return defaultValue;
        }

        /// <summary>
        /// Saves a Vector3 value associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        /// <param name="value">The value to store.</param>
        public static void Save(Object target, string namePart, Vector3 value) {
            var serialized = string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1},{2}",
                value.x,
                value.y,
                value.z
            );

            EditorPrefs.SetString(MakeKey(target, namePart), serialized);
        }

        /// <summary>
        /// Loads a Color value associated with a specific Unity object instance.
        /// Stored as "r,g,b,a" using invariant culture.
        /// </summary>
        public static Color Load(Object target, string namePart, Color defaultValue) {
            var raw = EditorPrefs.GetString(MakeKey(target, namePart), string.Empty);
            if (string.IsNullOrEmpty(raw)) {
                return defaultValue;
            }

            var parts = raw.Split(',');
            if (parts.Length != 4) {
                return defaultValue;
            }

            if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var r) &&
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var g) &&
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var b) &&
                float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var a)) {
                return new Color(r, g, b, a);
            }

            return defaultValue;
        }

        /// <summary>
        /// Saves a Color value associated with a specific Unity object instance.
        /// Stored as "r,g,b,a" using invariant culture.
        /// </summary>
        public static void Save(Object target, string namePart, Color value) {
            var serialized = string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1},{2},{3}",
                value.r,
                value.g,
                value.b,
                value.a
            );

            EditorPrefs.SetString(MakeKey(target, namePart), serialized);
        }

        /// <summary>
        /// Deletes a stored editor preference associated with a specific Unity object instance.
        /// </summary>
        /// <param name="target">The Unity object used to generate a stable editor preference key.</param>
        /// <param name="namePart">The unique name segment of the preference key.</param>
        public static void Delete(Object target, string namePart) {
            EditorPrefs.DeleteKey(MakeKey(target, namePart));
        }
    }
}
