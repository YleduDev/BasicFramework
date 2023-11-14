using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Framework.DownLoad;
namespace Example
{
    public static class HotSetting
    {
        public static string DownLoadPath = "";
    }

    public class DownLoadItemExample : MonoBehaviour
    {


        private void Start()
        {
            //通过服务器 获得补丁数据

            //对比补丁数据获得List<Patch>  m_DownLoadList
            m_DownLoadList = new List<Patch>() { new Patch() { }, new Patch() { } };

            //开启下载
            StartDownLoadAB(() => Log.I("下载完成"), m_DownLoadList);
        }


        //所有热更的东西
        private Dictionary<string, Patch> m_HotFixDic = new Dictionary<string, Patch>();
        //所有需要下载的东西
        private List<Patch> m_DownLoadList = new List<Patch>();
        //所有需要下载的东西的Dic,缓存做校验等逻辑
        private Dictionary<string, Patch> m_DownLoadDic = new Dictionary<string, Patch>();
        //服务器上的资源名对应的MD5，用于下载后MD5校验
        private Dictionary<string, string> m_DownLoadMD5Dic = new Dictionary<string, string>();
        /// <summary>  储存已经下载的资源 </summary>
        public List<Patch> m_AlreadyDownList = new List<Patch>();
        //文件下载出错回调
        public Action<string> ItemError;

        //下载完成回调
        public Action LoadOver;
        //是否开始下载
        public bool StartDownload = false;
        //尝试重新下载次数
        private int m_TryDownCount = 0;
        private const int DOWNLOADCOUNT = 1;

        // 需要下载的资源总个数
        public int LoadFileCount { get; set; } = 0;
        // 需要下载资源的总大小 KB
        public float LoadSumSize { get; set; } = 0;

        //当前正在下载的资源
        private DownLoader m_CurDownload = null;

        /// <summary>
        /// 获取下载进度
        /// </summary>
        /// <returns></returns>
        public float GetProgress()
        {
            //  if (LoadSumSize <= 0) return 1;
            return GetLoadSize() / LoadSumSize;
        }

        /// <summary>
        /// 获取已经下载总大小
        /// </summary>
        /// <returns></returns>
        public float GetLoadSize()
        {
            float alreadySize = m_AlreadyDownList.Sum(x => x.Size);
            float curAlreadySize = 0;
            if (m_CurDownload != null)
            {
                Patch patch = FindPatchByGamePath(m_CurDownload.FileName);
                if (patch != null && !m_AlreadyDownList.Contains(patch))
                {
                    curAlreadySize = m_CurDownload.Process * patch.Size;
                }
            }
            return alreadySize + curAlreadySize;
        }

        public float GetDownSpeed()
        {
            if (m_CurDownload != null) return m_CurDownload.DownSpeed;
            return 0;
        }

        /// <summary>
        /// 下载数组
        /// </summary>
        /// <param name="complted"></param>
        /// <param name="allPatch"></param>
        /// <returns></returns>
        public IEnumerator StartDownLoadAB(Action complted, List<Patch> allPatch = null)
        {
            m_AlreadyDownList.Clear();
            StartDownload = true;
            if (allPatch == null)
            {
                allPatch = m_DownLoadList;
            }

            if (!Directory.Exists(HotSetting.DownLoadPath))
            {
                Directory.CreateDirectory(HotSetting.DownLoadPath);
            }

            List<DownLoader> downLoadAssetBundles = new List<DownLoader>();
            foreach (Patch patch in allPatch)
            {
                downLoadAssetBundles.Add(new DownLoader(patch.Url, HotSetting.DownLoadPath));
            }

            for (int i = 0; i < downLoadAssetBundles.Count; i++)
            {
                DownLoader downLoad = downLoadAssetBundles[i];

                m_CurDownload = downLoad;
                yield return StartCoroutine(downLoad.Download(
                  () =>
                  {
                      Patch patch = FindPatchByGamePath(downLoad.FileName);
                      if (patch != null)
                      {
                          m_AlreadyDownList.Add(patch);
                      }
                      downLoad.Destory();
                  },
             err => { }
            ));
            }
            //MD5码校验,如果校验没通过，自动重新下载没通过的文件，重复下载计数，达到一定次数后，反馈某某文件下载失败
            VerifyMD5(downLoadAssetBundles, complted);

        }

        /// <summary>
        /// 单个下载
        /// </summary>
        /// <param name="complted"></param>
        /// <param name="allPatch"></param>
        /// <returns></returns>
        public IEnumerator DownLoadAB(Action complted, List<Patch> allPatch = null)
        {
            List<DownLoader> downLoadAssetBundles = new List<DownLoader>();
            foreach (Patch patch in allPatch)
            {
                downLoadAssetBundles.Add(new DownLoader(patch.Url, HotSetting.DownLoadPath));
            }

            for (int i = 0; i < downLoadAssetBundles.Count; i++)
            {
                DownLoader downLoad = downLoadAssetBundles[i];

                m_CurDownload = downLoad;
                yield return StartCoroutine(downLoad.Download(
                  () =>
                  {
                      Patch patch = FindPatchByGamePath(downLoad.FileName);
                      if (patch != null)
                      {
                          m_AlreadyDownList.Add(patch);
                      }
                      downLoad.Destory();
                  },
             null
            ));
            }
            //MD5码校验,如果校验没通过，自动重新下载没通过的文件，重复下载计数，达到一定次数后，反馈某某文件下载失败
            VerifyMD5(downLoadAssetBundles, complted);

        }

        /// <summary>/// 验证MD5/// </summary>
        /// <param name="downLoadAssets">需要下载的数据</param>
        /// <param name="callBack"></param>
        void VerifyMD5(List<DownLoader> downLoadAssets, Action callBack)
        {
            List<Patch> downLoadList = new List<Patch>();
            foreach (DownLoader downLoad in downLoadAssets)
            {
                string md5 = "";
                if (m_DownLoadMD5Dic.TryGetValue(downLoad.FileName, out md5))
                {
                    var dowloadFileMd5 = MD5Manager.Instance.BuildFileMd5(downLoad.SaveFilePath);
                    Debug.Log($"保存文件地址：{downLoad.SaveFilePath} \r\n 下载文件地址Md5:{downLoad.Url} 服务器资源MD5:{md5}, downLoadMD5{dowloadFileMd5}");
                    if (dowloadFileMd5 != md5)
                    {
                        Debug.Log(string.Format("此文件{0}MD5校验失败，即将重新下载", downLoad.FileName));
                        Patch patch = FindPatchByGamePath(downLoad.FileName);
                        if (patch != null)
                        {
                            downLoadList.Add(patch);
                        }
                    }
                }
            }

            if (downLoadList.Count <= 0)
            {
                m_DownLoadMD5Dic.Clear();
                if (callBack != null)
                {
                    StartDownload = false;
                    callBack();
                }
                if (LoadOver != null)
                {
                    LoadOver();
                }
            }
            else
            {
                if (m_TryDownCount >= DOWNLOADCOUNT)
                {
                    string allName = "";
                    StartDownload = false;
                    foreach (Patch patch in downLoadList)
                    {
                        allName += patch.Name + ";";
                    }
                    Debug.LogError("资源重复下载4次MD5校验都失败，请检查资源" + allName);
                    if (ItemError != null)
                    {
                        ItemError(allName);
                    }
                }
                else
                {
                    m_TryDownCount++;
                    m_DownLoadMD5Dic.Clear();
                    foreach (Patch patch in downLoadList)
                    {
                        m_DownLoadMD5Dic.Add(patch.Name, patch.Md5);
                    }
                    //自动重新下载校验失败的文件
                    StartCoroutine(DownLoadAB(callBack, downLoadList));
                }
            }
        }

        /// <summary>/// 根据名字查找对象的热更Patch/// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Patch FindPatchByGamePath(string name)
        {
            Patch patch = null;
            m_DownLoadDic.TryGetValue(name, out patch);
            return patch;
        }

        /*
         public IEnumerator StartDownLoadAB(Action complted, List<string> Urls = null)
        {
            Debug.Log("BeginDownLoad");
            m_AlreadyDownList.Clear();
            StartDownload = true;
            //下载
            if (Urls != null || Urls.Count >= 0)
            {
                ALLLoadSize = Urls.Count;
                Queue<DownLoader> downLoadAssetBundles = new Queue<DownLoader>();
                foreach (string url in Urls)
                {
                    string downLoadPath = url.GetPersistentPath();

                    if (!Directory.Exists(downLoadPath))
                    {
                        Directory.CreateDirectory(downLoadPath);
                    }
                    //文件校验
                    if (!Verify(url))
                    {
                        downLoadAssetBundles.Enqueue(new DownLoader(url, downLoadPath));
                    }
                    else
                    {
                        m_AlreadyDownList.Add(url.GetFileName());
                    }
                }

                bool downLoadStop = false;
                while (downLoadAssetBundles.Count>0&&!downLoadStop)
                {
                    DownLoader downLoad = downLoadAssetBundles.Peek();
                    m_CurDownload = downLoad;
                    yield return SingleMonoBehaviour.Instance.StartCoroutine(downLoad.Download(
                      () =>
                      {
                          downLoadAssetBundles.Dequeue();
                          m_AlreadyDownList.Add(downLoad.FileName);
                          downLoad.Destory();
                      },
                 err =>
                 {
                     Debug.LogError(err);
                     //暂停下载
                     downLoadStop = true;
                     if (ILUIKit.GetPanel<UITraniningCommonPopPanel>() == null)
                         UITraniningCommonPopPanel.OpenNetPop(
                             () =>
                             {
                                 ILUIKit.ClosePanel("resources/" + "UISystem/TraningHall/UITraniningDownLoadPanel");
                                 mData.backAction?.Invoke();
                             },
                             () =>
                             {
                                 if (downloadCoroutine != null)
                                 {
                                     SingleMonoBehaviour.Instance.StopCoroutine(downloadCoroutine);
                                 }
                                 SingleMonoBehaviour.Instance.StartCoroutine(StartDownLoadAB(complted, Urls));
                             }, err
                             );
                 }
                ));
                }
            }
            //
           if(m_AlreadyDownList.Count== ALLLoadSize) complted?.Invoke();
            StartDownload = false;
        }
         */
    }
}