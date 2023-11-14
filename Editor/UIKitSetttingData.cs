using System;
using System.IO;

using UnityEngine;

namespace Framework
{
	using UnityEditor;

	[Serializable]
	public class UIKitSettingData
	{
		static string mConfigSavedDir
		{
			get { return (Application.dataPath + "/FrameworkData/").CreateDirIfNotExists() + "ProjectConfig/"; }
		}

		private const string mConfigSavedFileName = "ProjectConfig.json";

		public string Namespace;

		public string UIScriptDir = "/Scripts/UI";

		public string UIPrefabDir = "/Art/UIPrefab";

		public string DefaultViewControllerScriptDir = "/Scripts/Game";
		
		public string DefaultViewControllerPrefabDir = "/Art/Prefab";
		
		public bool IsDefaultNamespace
		{
			get { return Namespace == "Framework.Example"; }
		}
		
		public static string GetScriptsPath()
		{
			return Load().UIScriptDir;
		}
		
		public static string GetProjectNamespace()
		{
			return Load().Namespace;
		}
		
		public static UIKitSettingData Load()
		{
			mConfigSavedDir.CreateDirIfNotExists();

			if (!File.Exists(mConfigSavedDir + mConfigSavedFileName))
			{
				using (var fileStream = File.Create(mConfigSavedDir + mConfigSavedFileName))
				{
					fileStream.Close();
				}
			}

			var frameworkConfigData =
				JsonUtility.FromJson<UIKitSettingData>(File.ReadAllText(mConfigSavedDir + mConfigSavedFileName));

			if (frameworkConfigData == null || string.IsNullOrEmpty(frameworkConfigData.Namespace))
			{
				frameworkConfigData = new UIKitSettingData {Namespace = "Framework.Example"};
			}

			return frameworkConfigData;
		}

		public void Save()
		{
			File.WriteAllText(mConfigSavedDir + mConfigSavedFileName,JsonUtility.ToJson(this));
			AssetDatabase.Refresh();
		}
	}
}