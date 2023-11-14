namespace  Framework
{
    public class LocalAudioResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.resLoadType == ResLoadType.LocalAudio|| resSearchKeys.AssetName.StartsWith("localaudio:");
            //  return resSearchKeys.AssetName.StartsWith("localaudio:");
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            var resourcesRes = LocalAudioRes.Allocate(resSearchKeys.AssetName,resSearchKeys.OriginalAssetName);
            resourcesRes.AssetType = resSearchKeys.AssetType;
            return resourcesRes;
        }

    }

    public class NetAudioResCreator : IResCreator
    {
        
        public bool Match(ResSearchKeys resSearchKeys)
        {
            if(resSearchKeys.AssetName.StartsWith("netaudio:"))return true;
            switch (resSearchKeys.resLoadType)
            {
                case ResLoadType.NetAudioUNKNOWN:
                case ResLoadType.NetAudioACC:
                case ResLoadType.NetAudioAIFF:
                case ResLoadType.NetAudioIT:
                case ResLoadType.NetAudioMOD:
                case ResLoadType.NetAudioMPEG:
                case ResLoadType.NetAudioOGGVORBIS:
                case ResLoadType.NetAudioS3M:
                case ResLoadType.NetAudioWAV:
                case ResLoadType.NetAudioXM:
                case ResLoadType.NetAudioXMA:
                case ResLoadType.NetAudioVAG:
                case ResLoadType.NetAudioAUDIOQUEUE:
                    return true;
                //TODO ADD


                default:return false;
            }

        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            UnityEngine.AudioType at = UnityEngine.AudioType.MPEG;


            switch (resSearchKeys.resLoadType)
            {
                case ResLoadType.NetAudioUNKNOWN: at = UnityEngine.AudioType.UNKNOWN;break;
                case ResLoadType.NetAudioACC: at = UnityEngine.AudioType.ACC; break;
                case ResLoadType.NetAudioAIFF: at = UnityEngine.AudioType.AIFF; break;
                case ResLoadType.NetAudioIT: at = UnityEngine.AudioType.IT; break;
                case ResLoadType.NetAudioMOD: at = UnityEngine.AudioType.MOD; break;
                case ResLoadType.NetAudioMPEG: at = UnityEngine.AudioType.MPEG; break;
                case ResLoadType.NetAudioOGGVORBIS: at = UnityEngine.AudioType.OGGVORBIS; break;
                case ResLoadType.NetAudioS3M: at = UnityEngine.AudioType.S3M; break;
                case ResLoadType.NetAudioWAV: at = UnityEngine.AudioType.WAV; break;
                case ResLoadType.NetAudioXM: at = UnityEngine.AudioType.XM; break;
                case ResLoadType.NetAudioXMA: at = UnityEngine.AudioType.XMA; break;
                case ResLoadType.NetAudioVAG: at = UnityEngine.AudioType.VAG; break;
                case ResLoadType.NetAudioAUDIOQUEUE: at = UnityEngine.AudioType.AUDIOQUEUE; break;
                //TODO ADD

                default: at = UnityEngine.AudioType.UNKNOWN;break;
            }


            var resourcesRes = NetAudioRes.Allocate(resSearchKeys.AssetName, resSearchKeys.OriginalAssetName, at);
            resourcesRes.AssetType = resSearchKeys.AssetType;
            return resourcesRes;
        }

    }
}