using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System.IO;

public interface IGetPersistentDataPath
{
    string GetPersistentDataPath();
}
public class TrainingHall : IGetPersistentDataPath
{
    public string GetPersistentDataPath()
    {
        return Application.persistentDataPath + "/u3dres/TrainingHall/";
    }
}


public static class DownLoadSetting
{

    private static IGetPersistentDataPath pPath;
    public static IGetPersistentDataPath PerPath
    {
        get
        {
            if (pPath == null) pPath = new TrainingHall();
            return pPath;
        }
    }

    /// <summary>
    /// 获得文件在沙盒路径的全地址 不包含 文件本身
    /// 如 path + "SystemMusic/" 
    /// (没有. 后缀 默认是AB)
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetPersistentPath(this string fileName)
    {
        if (fileName.IsNullOrEmpty()) return null;

        string FileExtendName = fileName.GetFileExtendName();
        string path = PerPath.GetPersistentDataPath();
        if (FileExtendName.IsNullOrEmpty())
        {
            //没有后缀，放入到AB
            //Debug.LogError(string.Format("文件名称不符合规范FileName:{0} 默认放入AB文件夹", fileName));
            return path + "AssetBundles/";
        }
        else
            switch (FileExtendName)
            {
                case ".asset": path = path + "AssetBundles/"; break;
                case ".ogg":
                case ".mp3":
                case ".wav": path = path + "SystemMusic/"; break;
                case ".mp4":
                case ".avi":
                case ".rmvb":
                case ".wmv": path = path + "SystemVoide/"; break;
                case ".txt":
                case ".xml":
                case ".mid":
                case ".json": path = path + "SystemTxt/"; break;
                case ".png":
                case ".jpg": path = path + "SystemTexture/"; break;

                default: path = path + "Undefined/"; break;
            }
        return path;
    }
    /// <summary>
    /// 获得文件在沙盒路径的全地址 包含 fileName 自身  (如果文件名没有 .后缀 默认是AB资源)
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetFilePathInPersistentPath(this string fileName)
    {
        return GetPersistentPath(fileName) + fileName.GetFileName();
    }
    /// <summary>
    /// 沙盒路径是否有文件 没有. 后缀 默认是AB
    /// </summary>
    /// <param name="fileNameWihtExt"></param>
    /// <returns></returns>
    public static bool ExistsInPersistentPath(this string fileName)
    {
        string path = GetFilePathInPersistentPath(fileName);
        return File.Exists(path);
    }
}
