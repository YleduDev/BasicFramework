using UnityEditor;
using System.IO;

namespace Framework
{
	[InitializeOnLoad]
	public class ResKitAssetsMenu
	{
		public const   string AssetBundlesOutputPath       = "AssetBundles";
		private const string Mark_AssetBundle   = "Assets/@ResKit - AssetBundle Mark";

		static ResKitAssetsMenu()
		{
			Selection.selectionChanged = OnSelectionChanged;
		}

		public static void OnSelectionChanged()
		{
			var path = EditorUtils.GetSelectedPathOrFallback();
			if (!string.IsNullOrEmpty(path))
			{
				Menu.SetChecked(Mark_AssetBundle, Marked(path));
			}
		}

		public static bool Marked(string path)
		{
			var ai = AssetImporter.GetAtPath(path);
			var dir = new DirectoryInfo(path);
            //return string.Equals(ai.assetBundleName, dir.Name.Replace(".", "_").ToLower());
            return string.Equals(ai.assetBundleName, dir.Name.GetFileNameWithoutExtend().ToLower());
        }

		public static void MarkAB(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				var ai = AssetImporter.GetAtPath(path);
				var dir = new DirectoryInfo(path);

				if (Marked(path))
				{
					Menu.SetChecked(Mark_AssetBundle, false);
					ai.assetBundleName = null;
				}
				else
				{
					Menu.SetChecked(Mark_AssetBundle, true);
                    //ai.assetBundleName = dir.Name.Replace(".", "_");
                    ai.assetBundleName = dir.Name.GetFileNameWithoutExtend();
                }

				AssetDatabase.RemoveUnusedAssetBundleNames();
			}
		}


		[MenuItem(Mark_AssetBundle)]
		public static void MarkPTABDir()
		{
			var path = EditorUtils.GetSelectedPathOrFallback();
			MarkAB(path);
		}


	}
}