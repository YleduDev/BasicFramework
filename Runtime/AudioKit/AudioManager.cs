//***********************************************************
// 描述：这是一个功能代码
// 作者：
// 创建时间：2021-11-09 14:11:11
// 版 本：1.0
//***********************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Framework.Audio
{
    public class AudioManager :MonoSingleton<AudioManager>
    {

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            Log.I("AudioManager.Init");
            MusicPlayer = AudioPlayer.Allocate();
            MusicPlayer.usedCache = false;
            VoicePlayer = AudioPlayer.Allocate();
            VoicePlayer.usedCache = false;

            gameObject.transform.position = Vector3.zero;
            AudioKit.Settings.MusicVolume.Bind(volume => { MusicPlayer.SetVolume(volume); })
    .DisposeWhenGameObjectDestroyed(this);

            AudioKit.Settings.VoiceVolume.Bind(volume => { VoicePlayer.SetVolume(volume); })
                .DisposeWhenGameObjectDestroyed(this);

            AudioKit.Settings.IsMusicOn.Bind(musicOn =>
            {
                if (musicOn)
                {
                    if (CurrentMusicName.IsNotNullAndEmpty())
                    {
                        AudioKit.PlayMusic(CurrentMusic);
                    }
                }
                else
                {
                    MusicPlayer.Stop();
                }
            }).DisposeWhenGameObjectDestroyed(this);

            AudioKit.Settings.IsVoiceOn.Bind(voiceOn =>
            {
                if (voiceOn)
                {
                    if (CurrentVoiceName.IsNotNullAndEmpty())
                    {
                        AudioKit.PlayVoice(CurrentVoice);
                    }
                }
                else
                {
                    VoicePlayer.Stop();
                }
            }).DisposeWhenGameObjectDestroyed(this);

            AudioKit.Settings.IsSoundOn.Bind(soundOn =>
            {
                if (soundOn)
                {
                }
                else
                {
                    ForEachAllSound(player => player.Stop());
                }
            }).DisposeWhenGameObjectDestroyed(this);


            AudioKit.Settings.SoundVolume.Bind(soundVolume =>
            {
                ForEachAllSound(player => player.SetVolume(soundVolume));
            }).DisposeWhenGameObjectDestroyed(this);


            CheckAudioListener();
        }

        public AudioPlayer MusicPlayer { get; private set; }

        public AudioPlayer VoicePlayer { get; private set; }

        private static Dictionary<string, List<AudioPlayer>> mSoundPlayerInPlaying =
            new Dictionary<string, List<AudioPlayer>>(30);


        public void ForEachAllSound(Action<AudioPlayer> operation)
        {
            foreach (var audioPlayer in mSoundPlayerInPlaying.SelectMany(keyValuePair => keyValuePair.Value))
            {
                operation(audioPlayer);
            }
        }

        public bool IsSoundPlayingExcSelf(AudioPlayer Player)
        {
            foreach (var audioPlayer in mSoundPlayerInPlaying.SelectMany(keyValuePair => keyValuePair.Value))
            {
                if (audioPlayer != Player && audioPlayer.IsPlaying) return true;
            }
            return false;
        }

        //当前有段音效播放
        public bool IsSoundPlaying()
        {
            foreach (var audioPlayer in mSoundPlayerInPlaying.SelectMany(keyValuePair => keyValuePair.Value))
            {
                if (audioPlayer.IsPlaying) return true;
            }
            return false;
        }

        public void AddSoundPlayer2Pool(AudioPlayer audioPlayer)
        {
              // Debug.LogError("AddSoundPlayer2Pool:" + audioPlayer.Name);
            if (mSoundPlayerInPlaying.ContainsKey(audioPlayer.Name))
            {
                mSoundPlayerInPlaying[audioPlayer.Name].Add(audioPlayer);
            }
            else
            {
                mSoundPlayerInPlaying.Add(audioPlayer.Name, new List<AudioPlayer> { audioPlayer });
            }
        }

        public void RemoveSoundPlayerFromPool(AudioPlayer audioPlayer)
        {
            mSoundPlayerInPlaying[audioPlayer.Name].Remove(audioPlayer);
        }


        //常驻内存不卸载音频资源（待拓展）
        protected HashSet<string> mRetainAudioNames;


        #region 对外接口


        public void CheckAudioListener()
        {
            // 确保有一个AudioListener
            if (FindObjectOfType<AudioListener>() == null)
            {
                gameObject.AddComponent<AudioListener>();
            }
        }

        public string CurrentMusicName { get; set; }

        public string CurrentVoiceName { get; set; }

        public AudioClip CurrentMusic { get; set; }

        public AudioClip CurrentVoice { get; set; }

        #endregion

        public static void PlayVoiceOnce(AudioClip clip)
        {
            if (!clip)
            {
                return;
            }

            var unit = new AudioPlayer();
            unit.SetAudio(Instance.gameObject, clip, false);
        }
        public void ClearAllPlayingSound()
        {
            mSoundPlayerInPlaying.Clear();
        }
    }
}