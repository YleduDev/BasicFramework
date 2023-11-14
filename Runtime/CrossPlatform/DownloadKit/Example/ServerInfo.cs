using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
namespace Example
{
    [System.Serializable]
    public class ServerInfo
    {
        public VersionInfo[] GameVersion;
    }

    //当前游戏版本对应的所有补丁
    [System.Serializable]
    public class VersionInfo
    {
        public string Version;
        public Pathces[] Pathces;
    }

    //一个总补丁包
    [System.Serializable]
    public class Pathces
    {
        public string Version;

        public string Des;

        public List<Patch> Files;
    }

    /// <summary>
    /// 单个补丁包
    /// </summary>
    [System.Serializable]
    public class Patch
    {

        public string id;
        public int fisheyeAngle;
        public long expireTime; //到期时间
        public long trialDuation;   //试看时间
        public int payType;        //播放类型


        public string Name;

        public string Url;

        public string Platform;

        public string Md5;

        public long Size;
    }
}
