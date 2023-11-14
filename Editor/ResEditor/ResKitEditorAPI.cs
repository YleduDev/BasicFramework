using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class ResKitEditorAPI
    {
        public static void BuildAssetBundles()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
            BuildScript.BuildAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        }

        public static bool SimulationMode
        {
            get { return AssetBundlePathHelper.SimulationMode; }
            set { AssetBundlePathHelper.SimulationMode = value; }
        }

        public static void ForceClearAssetBundles()
        {
            ResKitAssetsMenu.AssetBundlesOutputPath.DeleteDirIfExists();
            (Application.streamingAssetsPath + "/AssetBundles").DeleteDirIfExists();

            AssetDatabase.Refresh();
        }
    }
}