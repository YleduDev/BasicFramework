using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework.DownLoad
{
    /// <summary>
    /// 下载封装抽象类，可以自定义下载，示例AssetBulidItem
    /// </summary>
    public abstract class DownLoadItem
    {
        /// <summary>
        /// 网络资源URL路径
        /// </summary>
        protected string m_Url;

        public string id;
        
        public string Url
        {
            get { return m_Url; }
        }
        /// <summary>
        /// 资源下载存放路径，不包含文件么
        /// </summary>
        protected string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
        }
        /// <summary>
        /// 文件名，不包含后缀
        /// </summary>
        protected string m_FileNameWithoutExt;
        public string FileNameWithoutExt
        {
            get { return m_FileNameWithoutExt; }
        }
        /// <summary>
        /// 文件后缀
        /// </summary>
        protected string m_FileExt;
        public string FileExt
        {
            get { return m_FileExt; }
        }
        /// <summary>
        /// 文件名，包含后缀
        /// </summary>
        protected string m_FileName;
        public string FileName
        {
            get { return m_FileName; }
        }
        /// <summary>
        /// 下载文件全路径，路径+文件名+后缀
        /// </summary>
        protected string m_SaveFilePath;
        public string SaveFilePath
        {
            get { return m_SaveFilePath; }
        }
        /// <summary>
        /// 原文件大小
        /// </summary>
        protected long m_FileLength;
        public long FileLength
        {
            get { return m_FileLength; }
            set { m_FileLength = value; }
        }
        /// <summary>
        /// 当前下载的大小
        /// </summary>
        protected long m_CurLength;
        public long CurLength
        {
            get { return m_CurLength; }
            set { m_CurLength = value; }
        }


        /// <summary>
        /// 当前下载的速度
        /// </summary>
        protected float m_Speed;
        public float DownSpeed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        ///// <summary>
        ///// 是否开始下载
        ///// </summary>
        //protected bool m_StartDownLoad;
        //public bool StartDownLoad
        //{
        //    get { return m_StartDownLoad; }
        //}

        public DownLoadItem(string url, string path)
        {
            m_Url = url;
            m_SavePath = path.EndsWith("/")?path.Substring(0,path.Length-1): path;
            //m_StartDownLoad = false;
            var filePath = new Uri(url).AbsolutePath;
            m_FileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            m_FileExt = Path.GetExtension(filePath);
            m_FileName = string.Format("{0}{1}", m_FileNameWithoutExt, m_FileExt);
            m_SaveFilePath = string.Format("{0}/{1}{2}", m_SavePath, m_FileNameWithoutExt, m_FileExt);
        }

        public virtual IEnumerator Download(Action Completed = null, Action<string> OnError = null)
        {
            yield return null;
        }




        /// <summary>
        /// 获取下载进度
        /// </summary>
        /// <returns></returns>
        public float Process
        {
            get
            {
                if (m_FileLength == 0) return 0f;
                else
                    return (float)m_CurLength / m_FileLength;
            }
        }


        public abstract void Destory();
    }
}
