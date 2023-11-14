namespace  Framework
{
    using UnityEngine;
    using System.Collections;

    public enum InternalResNamePrefixType
    {
        Url, // resources://
        Folder, // Resources/
    }
    public static class ResourcesResUtil
    {
        public static string ToResourcesResName(this string selfFilePath)
        {
            return string.Format("Resources/{0}", selfFilePath);
        }


        public static bool IsResourcesPath(this string name)
        {
            return name.ToLower().StartsWith("Resources/");
        }
    }

    public class ResourcesRes : Res
    {
        private ResourceRequest mResourceRequest;

        private string mPath;

        public static ResourcesRes Allocate(string name, InternalResNamePrefixType prefixType,string OriginalAssetName)
        {
            var res = SafeObjectPool<ResourcesRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
            }

            if (prefixType == InternalResNamePrefixType.Url)
            {
                res.mPath = OriginalAssetName.Substring("resources://".Length);
            }
            else
            {
                res.mPath = OriginalAssetName.Substring("Resources/".Length);
            }

            return res;
        }

        public override bool LoadSync()
        {
            if (!CheckLoadAble())
            {
                return false;
            }

            if (string.IsNullOrEmpty(mAssetName))
            {
                return false;
            }

            State = ResState.Loading;

            
            if (AssetType != null)
            {
                mAsset = Resources.Load(mPath,AssetType);
            }
            else
            {
                mAsset = Resources.Load(mPath);
            }
            
            
            if (mAsset == null)
            {
                Log.E("Failed to Load Asset From Resources:" + mPath);
                OnResLoadFaild();
                return false;
            }

            State = ResState.Ready;
            return true;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }

            if (string.IsNullOrEmpty(mAssetName))
            {
                return;
            }

            State = ResState.Loading;

            ResMgr.Instance.PushIEnumeratorTask(this);
        }

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            ResourceRequest resourceRequest = null;

            if (AssetType != null)
            {
                resourceRequest = Resources.LoadAsync(mPath, AssetType);
            }
            else
            {
                resourceRequest = Resources.LoadAsync(mPath);
            }

            mResourceRequest = resourceRequest;
            yield return resourceRequest;
            mResourceRequest = null;

            if (!resourceRequest.isDone)
            {
                Log.E("Failed to Load Resources:" + mAssetName);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            mAsset = resourceRequest.asset;

            State = ResState.Ready;

            finishCallback();
        }

        public override void Recycle2Cache()
        {
            SafeObjectPool<ResourcesRes>.Instance.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            if (mResourceRequest == null)
            {
                return 0;
            }

            return mResourceRequest.progress;
        }

        public override string ToString()
        {
            return string.Format("Type:Resources {1}", AssetName, base.ToString());
        }
    }
}