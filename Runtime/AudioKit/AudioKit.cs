using System;

using UnityEngine;

namespace Framework.Audio
{
    /*
      设计思路 
      背景音乐、长语音 全程各只有一个在播放
      短音效 当前可以有很多个同时播放
     */
    public class AudioKit
    {
        /// <summary>
        /// 音频相关的设置
        /// </summary>
        public readonly static AudioKitSettings Settings = new AudioKitSettings();

        public static AudioPlayer MusicPlayer
        {
            get { return AudioManager.Instance.MusicPlayer; }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="musicName"></param>
        /// <param name="onBeganCallback"></param>
        /// <param name="onEndCallback"></param>
        /// <param name="loop"></param>
        /// <param name="allowMusicOff"></param>
        /// <param name="volume"></param>

        public static void StopMusic()
        {
            AudioManager.Instance.MusicPlayer.Stop();
        }

        public static void PauseMusic()
        {
            AudioManager.Instance.MusicPlayer.Pause();
        }
        /// <summary>
        /// 注意 频繁 切换的话 不建议使用此方法
        /// </summary>
        /// <param name="musicName">全路径 如 ：D:\UnityProject\UnityProject_Git\ai\Assets\AssetBundleExport\Audio\xx.mp3</param>
        /// <param name="loop"></param>
        /// <param name="onBeganCallback"></param>
        /// <param name="onEndCallback"></param>
        /// <param name="allowMusicOff"></param>
        /// <param name="volume"></param>
        public static void PlayMusic(string musicName,string assetBundleName, bool loop = true, System.Action onBeganCallback = null,
      System.Action onEndCallback = null, bool allowMusicOff = true, float volume = -1f)
        {
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = musicName;

            if (!Settings.IsMusicOn.Value && allowMusicOff)
            {
                onBeganCallback.InvokeGracefully();
                onEndCallback.InvokeGracefully();
                return;
            }

            Log.I(">>>>>> Start Play Music");

            // TODO: 需要按照这个顺序去 之后查一下原因
            // 需要先注册事件，然后再play
            MusicPlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback.InvokeGracefully();

                if (volume < 0)
                {
                    MusicPlayer.SetVolume(Settings.MusicVolume.Value);
                }
                else
                {
                    MusicPlayer.SetVolume(volume);
                }

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnStartListener(null);
            });

            MusicPlayer.SetAudio(audioMgr.gameObject, assetBundleName, musicName, loop);

            MusicPlayer.SetOnFinishListener(player =>
            {
                onEndCallback.InvokeGracefully();

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                player.SetOnFinishListener(null);
            });
        }


        public static void ResumeMusic()
        {
            AudioManager.Instance.MusicPlayer.Resume();
        }

        public static AudioPlayer VoicePlayer
        {
            get { return AudioManager.Instance.VoicePlayer; }
        }
        /// <summary>
        /// 注意 频繁 切换的话 不建议使用此方法
        /// </summary>
        /// <param name="voiceName">全路径 如 ：D:\UnityProject\UnityProject_Git\ai\Assets\AssetBundleExport\Audio\xx.mp3</param>
        /// <param name="loop"></param>
        /// <param name="onBeganCallback"></param>
        /// <param name="onEndedCallback"></param>
        public static void PlayVoice(string voiceName,string assetBundleName, System.Action onBeganCallback = null, bool loop = false,
          System.Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;

            audioMgr.CurrentVoiceName = voiceName;

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }

            VoicePlayer.SetOnStartListener(player =>
            {
                onBeganCallback.InvokeGracefully();

                player.SetVolume(Settings.VoiceVolume.Value);

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });

            VoicePlayer.SetAudio(AudioManager.Instance.gameObject, assetBundleName, voiceName, loop);

            VoicePlayer.SetOnFinishListener(musicUnit =>
            {
                onEndedCallback.InvokeGracefully();

                VoicePlayer.SetOnFinishListener(null);
            });
        }

        /// <summary>
        /// 播放本地路径的音频
        /// </summary>
        /// <param name="voiceName">音频本地路径 带上后缀</param>
        /// <param name="onBeganCallback"></param>
        /// <param name="loop"></param>
        /// <param name="onEndedCallback"></param>
        public static void PlayLocalVoice(string voiceName, System.Action onBeganCallback = null, bool loop = false,
         System.Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;

            audioMgr.CurrentVoiceName = voiceName;

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }

            VoicePlayer.SetOnStartListener(player =>
            {
                onBeganCallback.InvokeGracefully();

                player.SetVolume(Settings.VoiceVolume.Value);

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });

            VoicePlayer.SetAudio(AudioManager.Instance.gameObject,"", voiceName.ToLocalAudioResName(), loop);

            VoicePlayer.SetOnFinishListener(musicUnit =>
            {
                onEndedCallback.InvokeGracefully();

                VoicePlayer.SetOnFinishListener(null);
            });
        }


   

        public static void PauseVoice()
        {
            VoicePlayer.Pause();
        }

        public static void ResumeVoice()
        {
            VoicePlayer.Resume();
        }

        public static void StopVoice()
        {
            VoicePlayer.Stop();
        }
   

        public static void StopAllSound()
        {
            AudioManager.Instance.ForEachAllSound(player => player.Stop());

            AudioManager.Instance.ClearAllPlayingSound();
        }

        public static void PuaseAllSound()
        {
            AudioManager.Instance.ForEachAllSound(player => player.Pause());
            
        }
        public static void ResumeAllSound()
        {
            AudioManager.Instance.ForEachAllSound(player => player.Resume());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="soundName">全路径 如 ：D:\UnityProject\UnityProject_Git\ai\Assets\AssetBundleExport\Audio\xx.mp3</param>
        /// <param name="loop"></param>
        /// <param name="callBack"></param>
        /// <param name="customEventId"></param>
        /// <returns></returns>
        public static AudioPlayer PlaySound(string soundName,string assetBundleName=null, Action<AudioPlayer> callBack = null, bool loop = false,
        int customEventId = -1)
        {
            if (!Settings.IsSoundOn.Value) return null;

            var soundPlayer = SafeObjectPool<AudioPlayer>.Instance.Allocate();

            soundPlayer.SetOnStartListener(player =>
            {
                player.SetVolume(Settings.SoundVolume.Value);

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });
            soundPlayer.SetAudio(AudioManager.Instance.gameObject, assetBundleName, soundName, loop);
            soundPlayer.SetOnFinishListener(soundUnit =>
            {
                if (callBack != null)
                {
                    callBack(soundUnit);
                }
                VoicePlayer.SetOnFinishListener(null);
                AudioManager.Instance.RemoveSoundPlayerFromPool(soundPlayer);
            });
            

            AudioManager.Instance.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }

       


        public static bool IsSoundPlaying()
        {
           return  AudioManager.Instance.IsSoundPlaying();
        }
        public static bool IsSoundPlayingExcSelf(AudioPlayer player)
        {
            return AudioManager.Instance.IsSoundPlayingExcSelf(player);
        }
        public static bool IsVoicePlaying()
        {
            return VoicePlayer.IsPlaying;
        }


        #region 参数 AudioClip

        public static void PlayMusic(AudioClip musicClip, bool loop = true, System.Action onBeganCallback = null,
    System.Action onEndCallback = null, bool allowMusicOff = true, float volume = -1f)
        {
            if (musicClip == null) return;
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = "music" + musicClip.GetHashCode();
            audioMgr.CurrentMusic = musicClip;

            if (!Settings.IsMusicOn.Value && allowMusicOff)
            {
                onBeganCallback?.Invoke();
                onEndCallback?.Invoke();
                return;
            }
            Log.I(">>>>>> Start Play Music");
            // TODO: 需要按照这个顺序去 之后查一下原因
            // 需要先注册事件，然后再play
            MusicPlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback?.Invoke();

                if (volume < 0)
                {
                    MusicPlayer.SetVolume(Settings.MusicVolume.Value);
                }
                else
                {
                    MusicPlayer.SetVolume(volume);
                }

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnStartListener(null);
            });

            MusicPlayer.SetAudio(audioMgr.gameObject, musicClip, loop);

            MusicPlayer.SetOnFinishListener(musicUnit =>
            {
                onEndCallback?.Invoke();

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnFinishListener(null);
            });
        }

        public static void PlayVoice(AudioClip voiceClip, bool loop = false, System.Action onBeganCallback = null,
            System.Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;
            if (voiceClip == null) return;
            audioMgr.CurrentVoiceName = "voice" + voiceClip.GetHashCode();

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }

            VoicePlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback?.Invoke();

                VoicePlayer.SetOnStartListener(null);
            });

            VoicePlayer.SetAudio(AudioManager.Instance.gameObject, voiceClip, loop);

            VoicePlayer.SetOnFinishListener(musicUnit =>
            {
                onEndedCallback?.Invoke();

                VoicePlayer.SetOnFinishListener(null);
            });
        }


        public static AudioPlayer PlaySound(AudioClip soundClip, bool loop = false, Action<AudioPlayer> callBack = null)
        {
            if (!Settings.IsSoundOn.Value) return null;
            if (soundClip == null) return null;
            var soundPlayer = SafeObjectPool<AudioPlayer>.Instance.Allocate();
            soundPlayer.SetAudio(AudioManager.Instance.gameObject, soundClip, loop);
            soundPlayer.SetVolume(Settings.SoundVolume.Value);

            soundPlayer.SetOnFinishListener(soundUnit =>
            {
                if (callBack != null)
                {
                    callBack(soundUnit);
                }

                VoicePlayer.SetOnFinishListener(null);
                AudioManager.Instance.RemoveSoundPlayerFromPool(soundPlayer);
            });

            AudioManager.Instance.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }


        #endregion


        #region
        /// <summary>
        /// 播放网络音频
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="url">全地址  如  http://xxxx.mp3</param>
        /// <param name="audioType">下载音频的类型 </param>
        /// <param name="onStartCallback">音频加载完后 播放之前 的回调</param>
        /// <param name="onEndedCallback">音频播放完后的回调</param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static AudioPlayer PlayNetSound(AudioSource audioSource, string url, AudioType audioType, System.Action<AudioPlayer> onStartCallback = null,
   System.Action<AudioPlayer> onEndedCallback = null, bool loop = false, bool autpPlay = true)
        {
            if (url.IsNullOrEmpty() || !url.ToLower().StartsWith("http") || audioSource == null) return null;

            if (!Settings.IsSoundOn.Value) return null;

            var soundPlayer = SafeObjectPool<AudioPlayer>.Instance.Allocate();

            soundPlayer.SetOnStartListener(player =>
            {
                player.SetVolume(Settings.SoundVolume.Value);

                onStartCallback?.Invoke(player);
                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });

            soundPlayer.SetNetAudio(audioSource, url, audioType, loop,autpPlay);
            soundPlayer.SetOnFinishListener(soundUnit =>
            {
                AudioManager.Instance.RemoveSoundPlayerFromPool(soundPlayer);
                if (onEndedCallback != null)
                {
                    onEndedCallback(soundUnit);
                }

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnFinishListener(null);

            });


            AudioManager.Instance.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }

        /// <summary>
        /// 播放本地音频
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="url">本地路径如 ：D:\UnityProject\UnityProject_Git\ai\Assets\AssetBundleExport\Audio\xx.mp3</param>
        /// <param name="onStartCallback">音频加载完 后 播放之前 的回调</param>
        /// <param name="onEndedCallback">音频播放完后的回调</param>
        /// <param name="loop">是否循环</param>
        /// <returns></returns>
        public static AudioPlayer PlayLocalSound(AudioSource audioSource, string url, System.Action<AudioPlayer> onStartCallback = null,
  System.Action<AudioPlayer> onEndedCallback = null, bool loop = false, bool autpPlay = true)
        {
            if (url.IsNullOrEmpty() || audioSource == null) return null;

            if (!Settings.IsSoundOn.Value) return null;

            var soundPlayer = SafeObjectPool<AudioPlayer>.Instance.Allocate();

            soundPlayer.SetOnStartListener(player =>
            {
                player.SetVolume(Settings.SoundVolume.Value);
                onStartCallback?.Invoke(player);
                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });

            soundPlayer.SetLocalAudio(audioSource, url, loop, autpPlay);
            soundPlayer.SetOnFinishListener(soundUnit =>
            {

                AudioManager.Instance.RemoveSoundPlayerFromPool(soundPlayer);

                if (onEndedCallback != null)
                {
                    onEndedCallback(soundUnit);
                }

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnFinishListener(null);
            });


            AudioManager.Instance.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }

        /// <summary>
        /// 播放本地音频
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="soundName">地路径如 ：D:\UnityProject\UnityProject_Git\ai\Assets\AssetBundleExport\Audio\xx.mp3 或 res文件夹下</param>
        /// <param name="callBack"></param>
        /// <param name="loop"></param>
        /// <param name="customEventId"></param>
        /// <returns></returns>
        public static AudioPlayer PlaySound(AudioSource audioSource, string soundName, System.Action<AudioPlayer> onStartCallback = null, Action<AudioPlayer> callBack = null, bool loop = false,bool autpPlay=true)
        {
            if (!Settings.IsSoundOn.Value) return null;

            var soundPlayer = SafeObjectPool<AudioPlayer>.Instance.Allocate();

            soundPlayer.SetOnStartListener(player =>
            {
                player.SetVolume(Settings.SoundVolume.Value);
                onStartCallback?.Invoke(player);
                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });
            soundPlayer.SetLocalAudio(audioSource,soundName, loop, autpPlay);

            soundPlayer.SetOnFinishListener(soundUnit =>
            {
                AudioManager.Instance.RemoveSoundPlayerFromPool(soundPlayer);
                if (callBack != null)
                {
                    callBack(soundUnit);
                }
                VoicePlayer.SetOnFinishListener(null);
            });


            AudioManager.Instance.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }

        #endregion



    }
}