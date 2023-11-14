using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SceneDownLoadItem : DownloadFile
{
    public SceneDownLoadItem(string url, string filePath) : base(url,filePath)
    {
        this.isOpenImpose = false;
    }

}
