using System.Collections.Generic;
using UnityEngine;

namespace  Framework
{
    using System;

    public enum ResState
    {
        Waiting = 0,
        Loading = 1,
        Ready   = 2,
    }

    public enum ResLoadType
    {
        AssetBundle = 0,
        ABAsset = 1,
        ABScene = 2,
        Internal = 3,
        NetImageRes = 4,
        LocalImageRes = 5,
        LocalAudio=6,

        NetAudioUNKNOWN = 101,
        NetAudioACC = 102,
        NetAudioAIFF = 103,
        NetAudioIT = 104,
        NetAudioMOD = 105,
        NetAudioMPEG = 106,
        NetAudioOGGVORBIS = 107,
        NetAudioS3M = 108,
        NetAudioWAV = 109,
        NetAudioXM = 110,
        NetAudioXMA = 111,
        NetAudioVAG = 112,
        NetAudioAUDIOQUEUE=113,
    }




    public interface IRes : IRefCounter, IPoolType, IEnumeratorTask
    {
        string AssetName { get; }

        string OwnerBundleName { get; }

        ResState State { get; }

        UnityEngine.Object Asset { get; }

        float Progress { get; }
        Type AssetType { get; set; }

        void RegisteOnResLoadDoneEvent(Action<bool, IRes> listener);
        void UnRegisteOnResLoadDoneEvent(Action<bool, IRes> listener);

        bool UnloadImage(bool flag);

        bool LoadSync();

        void LoadAsync();

        string[] GetDependResList();

        bool IsDependResLoadFinish();

        bool ReleaseRes();
        
    }
}