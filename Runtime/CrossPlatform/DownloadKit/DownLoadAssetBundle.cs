using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
namespace Framework.DownLoad
{
    /// <summary>
    /// AB ���ط�װ����
    /// </summary>
    public class DownLoader : DownLoadItem
    {
        public UnityWebRequest unityWeb;

        public DownLoader(string url, string path) : base(url, path)
        {

        }

        public DownLoader(string url, string path, string fileName, string id) : base(url, path)
        {
            if (id != null)
                this.id = id;
            if (fileName != null)
                m_FileName = fileName;
        }

        public DownLoader(string url, string path,string id) : base(url, path)
        {
            if (id != null)
                this.id = id;
        }

        public override IEnumerator Download(Action Completed = null, Action<string> error = null)
        {
            if (!Directory.Exists(m_SavePath))
                Directory.CreateDirectory(m_SavePath);   //���������ַ�ļ���


            //HttpWebRequest httpWebRequest
            //������������
            unityWeb = UnityWebRequest.Get(m_Url);
            //�½������ļ����
            DownloadFileHandler downloadFile = new DownloadFileHandler(m_SavePath, m_FileName, this);
            //Ĭ�ϳ�ʱ 60s
            //unityWeb.timeout = 60 * 5 ;
            unityWeb.downloadHandler = downloadFile;
        
            m_CurLength = downloadFile.NowLength;

            //���ÿ�ʼ�����ļ���ʲôλ�ÿ�ʼ
            unityWeb.SetRequestHeader("Range", "bytes=" + m_CurLength + "-");//������Ҫ

            yield return unityWeb.SendWebRequest();


            //�����쳣
            if (unityWeb != null && (unityWeb.isNetworkError || unityWeb.isHttpError))
            {
                Debug.LogError("Download Error:" + unityWeb.error);
                Debug.LogError("m_Url:" + m_Url);
                //TODO���� ����ȡ��
                error?.Invoke(unityWeb.error);
            }

            if (downloadFile.IsDone)
            {
                Log.I("������ɣ�");
                Completed?.Invoke();
            }

        }

        public override void Destory()
        {
            Log.I("Destory22222334333");
            if (unityWeb != null)
            {
                unityWeb.Dispose();
                unityWeb = null;
            }
        }

    }
}
