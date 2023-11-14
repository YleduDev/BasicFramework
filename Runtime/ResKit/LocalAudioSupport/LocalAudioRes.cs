namespace  Framework
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Networking;


    public static class LocalAudioResUtil
    {
        public static string ToLocalAudioResName(this string selfFilePath)
        {
            return string.Format("LocalAudio:{0}", selfFilePath);
        }


        public static bool IsLocalAudioPath(this string name)
        {
            return name.ToLower().StartsWith("localaudio:");
        }
    }
    
    /// <summary>
    /// 本地音频加载器
    /// </summary>
    public class LocalAudioRes : Res
    {
        private string mFullPath;
        private string mHashCode;
        private UnityWebRequest webRequest = null;

        public static LocalAudioRes Allocate(string name,string originalName)
        {
            var res = SafeObjectPool<LocalAudioRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;

                if (originalName.IsLocalAudioPath())
                    originalName = originalName.Substring(11);

                res.SetUrl(originalName);
            }
            return res;
        }

        public void SetDownloadProgress(int totalSize, int download)
        {

        }

        

        public bool NeedDownload
        {
            get { return RefCount > 0; }
        }

        public string Url
        {
            get { return mFullPath; }
        }

        public int FileSize
        {
            get { return 1; }
        }

        public void SetUrl(string url)
        {            
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            mFullPath = url;
            mHashCode = string.Format("Photo_{0}", mFullPath.GetHashCode());
        }


        public override bool LoadSync()
        {
            return false;
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

            DoLoadWork();
        }

        private void DoLoadWork()
        {
            State = ResState.Loading;

            OnDownLoadResult(true);

            //检测本地文件是否存在
            /*
            if (!File.Exists(LocalResPath))
            {
                ResDownloader.S.AddDownloadTask(this);
            }
            else
            {
                OnDownLoadResult(true);
            }
            */
        }

        protected override void OnReleaseRes()
        {
            if (mAsset != null)
            {
                GameObject.Destroy(mAsset);
                mAsset = null;
            }
        }

        public override void Recycle2Cache()
        {
            SafeObjectPool<LocalAudioRes>.Instance.Recycle(this);
        }

        public override void OnRecycled()
        {

        }

        public void DeleteOldResFile()
        {
            //throw new NotImplementedException();
        }

        public void OnDownLoadResult(bool result)
        {
            if (!result)
            {
                OnResLoadFaild();
                return;
            }

            if (RefCount <= 0)
            {
                State = ResState.Waiting;
                return;
            }

            ResMgr.Instance.PushIEnumeratorTask(this);
        }

        //完全的WWW方式,Unity 帮助管理纹理缓存，并且效率貌似更高
        // TODO:persistantPath 用 read
        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + mFullPath, AudioType.MPEG);
            
            yield return webRequest.SendWebRequest();

            if (webRequest.result== UnityWebRequest.Result.ConnectionError)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }


            if (!webRequest.isDone)
            {
                OnResLoadFaild();
                finishCallback();

                webRequest.Dispose();
                webRequest = null;

                yield break;
            }

            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();

                webRequest.Dispose();
                webRequest = null;
                yield break;
            }
            mAsset = DownloadHandlerAudioClip.GetContent(webRequest);

            webRequest.Dispose();
            webRequest = null;

            State = ResState.Ready;

            finishCallback();
        }

        protected override float CalculateProgress()
        {
            if (webRequest == null)
            {
                return 0;
            }

            return webRequest.downloadProgress;
        }

    }
}
