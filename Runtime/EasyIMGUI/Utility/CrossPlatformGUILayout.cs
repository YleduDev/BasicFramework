#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Scripts
{
    public class CrossPlatformGUILayout
    {
        public static string PasswordField(string value, GUIStyle style, params GUILayoutOption[] options)
        {
            if (Application.isPlaying)
            {
                return GUILayout.PasswordField(value, '*', style, options);
            }
            else
            {
#if UNITY_EDITOR
                return EditorGUILayout.PasswordField(value, style, options);
#endif
                return null;
            }
        }

        public static string TextField(string value, GUIStyle style, params GUILayoutOption[] options)
        {
            if (Application.isPlaying)
            {
                return GUILayout.TextField(value, style, options);
            }
            else
            {
#if UNITY_EDITOR
                return EditorGUILayout.TextField(value, style, options);
#endif
                return null;
            }
        }

        public static string TextArea(string value, GUIStyle style, params GUILayoutOption[] options)
        {
            if (Application.isPlaying)
            {
                return GUILayout.TextArea(value, style, options);

            }
            else
            {
#if UNITY_EDITOR
                return EditorGUILayout.TextArea(value, style, options);
#endif
                return null;
            }
        }
    }
}