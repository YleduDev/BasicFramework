using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Example;
using Framework.DownLoad;
using UniRx;


namespace Framework
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDownloadFile
    {
        /// <summary>
        /// 获得下载封装对象
        /// </summary>
        DownloadFile GetDownFile();

        /// <summary>
        /// 获得下载文件的全路径
        /// </summary>
        /// <returns></returns>
        string GetFilePath();


        void OnProgress(float progress);

        void OnPause();

        void OnResume();
    }

    public class DownLoadMgr : MonoSingleton<DownLoadMgr>
    {

        //下载器 间隔事件调用（类似 update）
        private const float IntervalTime = 1f;

        /// <summary>/// 视频下载缓存 key 表示：下载地址/// </summary>
        private Dictionary<IDownloadFile, DownloadFile> cacheTask = new Dictionary<IDownloadFile, DownloadFile>();


        //当前下载个数
        private int CurDownLoadNum
        {
            get { return cacheTask==null?0:cacheTask.Count; }
        }

        /// <summary>/// 正在下载的/// </summary>
        private int CurDownLoadingNum
        {
            get
            {
                if (cacheTask == null) return 0;
                var itmes = cacheTask.Where(i => i.Value != null && i.Value.IsDownloading).ToList();
                return itmes.Count;
            }
        }

        public override void OnSingletonInit()
        {
            //订阅 每隔 IntervalTime 时间 调用一次（update 间隔太过频繁）
            Observable.Interval(TimeSpan.FromSeconds(IntervalTime)).Subscribe(OnUpdate).AddTo(transform);
        }

        #region 增删停

        /// <summary>/// 添加到下载队列/// </summary>
        private DownloadFile AddTask(IDownloadFile file)
        {
            DownloadFile downloadFile = null;
            if (!cacheTask.ContainsKey(file))
            {
                downloadFile = file.GetDownFile();
                cacheTask.Add(file, downloadFile);
            }
            else
            {
                Log.I("下载队列中存在相同的下载地址");
                downloadFile=cacheTask[file];
            }

            return downloadFile;
        }
           
        #endregion

        #region Public

        public DownloadFile GetDownLoad(IDownloadFile file)
        {
            DownloadFile downfile = null;

            cacheTask.TryGetValue(file, out downfile);

            return downfile;
        }

        public void StartDownLoad(IDownloadFile file)
        {
            if (file != null)
            {
                var down = AddTask(file);
                down.Download();
            }
            
        }
        //暂停 
        public void PauseTask(IDownloadFile downloadUrl)
        {
            if (cacheTask.ContainsKey(downloadUrl))
            {
                //如果正在下载
                if (cacheTask[downloadUrl].IsDownloading)
                {
                    cacheTask[downloadUrl].Close();
                    downloadUrl.OnPause();
                }
            }
        }

        // 继续
        public void ResumeTask(IDownloadFile downloadUrl)
        {
            if (cacheTask.ContainsKey(downloadUrl))
            {
                if (!cacheTask[downloadUrl].IsDownloading)
                {
                    cacheTask[downloadUrl].Resume();
                    downloadUrl.OnResume();
                }
            }
        }
        //移除下载任务 删除缓存文件
        public bool RemoveTask(IDownloadFile downloadUrl)
        {
            if (cacheTask.ContainsKey(downloadUrl))
            {
                cacheTask[downloadUrl].Close();
                cacheTask[downloadUrl].DeleteTempFile();
                cacheTask.Remove(downloadUrl);
                return true;
            }
            return false;
        }

        #endregion

        void OnUpdate(long l)
        {
            cacheTask.Where(I => cacheTask.Count > 0 &&  I.Value != null && I.Value.IsDownloading).ForEach(item => item.Key.OnProgress(item.Value.Process));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            cacheTask.Where(I => cacheTask.Count > 0 && I.Value != null && I.Value.IsDownloading).ForEach(item => PauseTask(item.Key));
        }
    }
}