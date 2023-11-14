#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace  Framework
{
    public class ConfigFileUtility
    {
        public static string AssetPath2Name(string assetPath)
        {
            var startIndex = assetPath.LastIndexOf("/") + 1;

            var endIndex = assetPath.LastIndexOf(".");
            if (endIndex > 0)
            {
                var length = endIndex - startIndex;
                return assetPath.Substring(startIndex, length).ToLower();
            }

            return assetPath.Substring(startIndex).ToLower();
        }
    }
}