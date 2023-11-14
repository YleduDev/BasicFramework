using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace  Framework
{
    public class AssetBundleSettings
    {

        public static bool LoadAssetResFromStreamingAssetsPath
        {
            get { return PlayerPrefs.GetInt("LoadResFromStreamingAssetsPath", 1) == 1; }
            set { PlayerPrefs.SetInt("LoadResFromStreamingAssetsPath", value ? 1 : 0); }
        }

        static AssetBundleManifest manifest;
        public static string[] GetAllDependenciesByName(string name)
        {
            string[] dependencies = null;
            if(name.IsNullOrEmpty()) return null;
            if(manifest == null)
            {
                var url = AssetBundleManifestName2Url();
                if(url.IsNullOrEmpty()) return dependencies;
                var bundle = AssetBundle.LoadFromFile(url);
                if(bundle != null) manifest= bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest != null) dependencies= manifest.GetAllDependencies(name);
            }
            else
            {
                dependencies= manifest.GetAllDependencies(name);
            }
            if(dependencies != null&& dependencies.Length>=0) return dependencies;
            return null;
        }


        #region AssetBundle 相关

        public static string ABName2PersistentDataPath(string abName)
        {
            string retUrl = AssetBundlePathHelper.PersistentDataPath + "AssetBundles/" +
                               AssetBundlePathHelper.GetPlatformName() + "/" + abName;
            return retUrl;
        }

        public static string AssetBundleUrl2Name(string url)
        {
            string retName = null;
            string parren = AssetBundlePathHelper.StreamingAssetsPath + "AssetBundles/" +
                            AssetBundlePathHelper.GetPlatformName() + "/";
            retName = url.Replace(parren, "");

            parren = AssetBundlePathHelper.PersistentDataPath + "AssetBundles/" +
                     AssetBundlePathHelper.GetPlatformName() + "/";
            retName = retName.Replace(parren, "");
            return retName;
        }

        public static string AssetBundleName2Url(string name)
        {
            if (name.IsNotNullAndEmpty()) name= name.Trim();
            string retUrl = AssetBundlePathHelper.PersistentDataPath + "AssetBundles/" +
                            AssetBundlePathHelper.GetPlatformName() + "/" + name;

            if (File.Exists(retUrl))
            {
                return retUrl;
            }

            retUrl = AssetBundlePathHelper.StreamingAssetsPath + "AssetBundles/" +
                           AssetBundlePathHelper.GetPlatformName() + "/" + name;
            if (File.Exists(retUrl))
            {
                return retUrl;
            }

            return retUrl;
        }


        public static string AssetBundleManifestName2Url()
        {
            //if (File.Exists(name))
            //{
            //    return name;
            //}
            string retUrl = AssetBundlePathHelper.PersistentDataPath + "AssetBundles/" +
                            AssetBundlePathHelper.GetPlatformName() + "/" + AssetBundlePathHelper.GetPlatformName();

            if (File.Exists(retUrl))
            {
                return retUrl;
            }



            retUrl = AssetBundlePathHelper.StreamingAssetsPath + "AssetBundles/" +
                           AssetBundlePathHelper.GetPlatformName() + "/" + AssetBundlePathHelper.GetPlatformName();

            if (File.Exists(retUrl))
            {
                return retUrl;
            }

            //retUrl = VocalMusicManager.GetInstance().GetVpcalMusicFulllocaPath() + name;
            //retUrl = VocalMusicManager.GetInstance().GetLocalPath(VocalMusicResType.AssetBundle) + name;

            // if (File.Exists(retUrl)) return  retUrl;

            return retUrl;
            //return AssetBundlePathHelper.StreamingAssetsPath + "AssetBundles/" +
            //       AssetBundlePathHelper.GetPlatformName() + "/" + name;
        }

        //导出目录

        /// <summary>
        /// AssetBundle存放路径
        /// </summary>
        public static string RELATIVE_AB_ROOT_FOLDER
        {
            get { return "/AssetBundles/" + AssetBundlePathHelper.GetPlatformName() + "/"; }
        }

        #endregion
    }
}