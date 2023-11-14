
using System;
using System.Diagnostics;
using UnityEngine;

namespace Framework
{
        
    public class AssetBundleSceneRes : AssetRes
    {
        public static AssetBundleSceneRes Allocate(string name,string OwnerBundle)
        {
            AssetBundleSceneRes res = SafeObjectPool<AssetBundleSceneRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
                res.OwnerBundleName = OwnerBundle;
                res.InitAssetBundleName();
            }
            return res;
        }

        public AssetBundleSceneRes(string assetName) : base(assetName)
        {

        }

        public AssetBundleSceneRes()
        {

        }

        public override bool LoadSync()
        {
            if (!CheckLoadAble())
            {
                return false;
            }

            if (string.IsNullOrEmpty(AssetBundleName))
            {
                return false;
            }

            var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName,null,typeof(AssetBundle));
            
            var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);

            resSearchKeys.Recycle2Cache();

            if (abR == null || abR.AssetBundle == null)
            {
              
                return false;
            }


            State = ResState.Ready;
            return true;
        }

        public override void LoadAsync()
        {
            LoadSync();
        }


        public override void Recycle2Cache()
        {
            SafeObjectPool<AssetBundleSceneRes>.Instance.Recycle(this);
        }
    }
}