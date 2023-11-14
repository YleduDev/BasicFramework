using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Framework.Audio
{
    public class AudioPlayer : IPoolable, IPoolType
    {
        private ResLoader mLoader;

        private AudioSource mAudioSource;
        public AudioSource AudioSource
        {
            get { return mAudioSource; }
        }
        private AudioClip mAudioClip;
        public AudioClip AudioClip
        {
            get { return mAudioClip; }
        }
        private bool mIsLoop;
        private string mName;

        public string Name
        {
            get { return mName; }
        }
        private bool mUsedCache = true;

        private bool mIsCache = false;
        private bool mAudioSourceCache = false;

        private Action<AudioPlayer> mOnStartListener;
        private Action<AudioPlayer> mOnFinishListener;
        private bool mIsPause = false;
        private float mLeftDelayTime = -1;
        private int mPlayCount = 0;
        private TimeItem mTimeItem;

        private bool canPlayAudioWhenNotAuto = false;//在不自动播的情况下，判断是否能播放音频

        private bool autoPlay = true;//自动播放标准
        private bool isLoaded=false;

        private float songPosition;
        /// <summary>/// // </summary>
        public float SongPosition
        {
            get
            {
                return AudioSource==null?0: AudioSource.time;
            }
            set
            {
                if (value >= Duration) value = Duration;
                if (value <= 0) value = 0;
                if (mAudioSource)
                {
                    mAudioSource.time = value;
                    if (mTimeItem != null)
                    {
                        mLeftDelayTime = Duration -value;                      
                        mTimeItem.Cancel();
                        mTimeItem = null;
                    }
                    //
                    if(!mIsPause&&mLeftDelayTime >= 0) mTimeItem= Timer.Instance.Post2Scale(OnResumeTimeTick, mLeftDelayTime);                
                }
            }
        }
        public float Duration
        {
            get { if (mAudioClip != null) return mAudioClip.length; return 0; }
        }
        

        public bool usedCache
        {
            get { return mUsedCache; }
            set { mUsedCache = value; }
        }
        public int playCount
        {
            get { return mPlayCount; }
        }
        public static AudioPlayer Allocate()
        {
            return SafeObjectPool<AudioPlayer>.Instance.Allocate();
        }
        public bool IsPlaying
        {
            get { if (mAudioSource) return mAudioSource.isPlaying; return false; }
        }

        public void SetOnStartListener(Action<AudioPlayer> l)
        {
            mOnStartListener = l;
        }

        public void SetOnFinishListener(Action<AudioPlayer> l)
        {
            mOnFinishListener = l;
        }
        public bool IsRecycled
        {
            get { return mIsCache; }

            set { mIsCache = value; }
        }

        public void SetAudio(GameObject root, AudioClip clip, bool loop, float delay = 0)
        {
            if (clip == null)
            {
                return;
            }

            if (mAudioClip == clip)
            {
                return;
            }
            if (mAudioSource == null)
            {
                mAudioSource = root.AddComponent<AudioSource>();
            }

            //清空
            CleanResources();
            mAudioSourceCache = false;
            mIsLoop = loop;
            mAudioClip = clip;
            mName = clip.name;
            PlayAudioClip();
        }


        public void SetAudio(GameObject root,string abName ,string name, bool loop)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (mName == name)
            {
                return;
            }

            if (mAudioSource == null)
            {
                mAudioSource = root.AddComponent<AudioSource>();
            }

            //
            var preLoader = mLoader;

            mLoader = null;

            CleanResources();

            mLoader = ResLoader.Allocate();

            mIsLoop = loop;
            mName = name;
            mAudioSourceCache = false;

            if (name.IsLocalAudioPath())
            {
                mLoader.LoadLocalAudioAsync(name, OnResLoadFinish);
            }
            //TODO 网络音频
            else//ab包内 或者 res 内
            {
                mLoader.Add2Load(abName, name, OnResLoadFinish);
            }
           

            if (preLoader != null)
            {
                preLoader.Recycle2Cache();
                preLoader = null;
            }

            if (mLoader != null)
            {
                mLoader.LoadAsync();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="loop"></param>
        public void SetAudio(GameObject root,string name, bool loop, bool autoPlay = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (mName == name)
            {
                return;
            }

            if (mAudioSource == null)
            {
                mAudioSource = root.AddComponent<AudioSource>();
            }

            //
            var preLoader = mLoader;

            mLoader = null;

            CleanResources();

            mLoader = ResLoader.Allocate();

            mIsLoop = loop;
            mName = name;
            mAudioSourceCache = false;
            this.autoPlay = autoPlay;

            mLoader.Add2Load(name, ResLoadType.LocalAudio, OnResLoadFinish );

            if (preLoader != null)
            {
                preLoader.Recycle2Cache();
                preLoader = null;
            }

            if (mLoader != null)
            {
                mLoader.LoadAsync();
            }
        }


        public void SetLocalAudio(AudioSource audioSource, string FileName, bool loop = false,bool autoPlay=true)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return;
            }

            if (mName == FileName)
            {
                return;
            }

            if (audioSource == null)
            {
                return;
            }

            if (mAudioSource == null)
            {
                mAudioSource = audioSource;
            }
            //
            var preLoader = mLoader;

            mLoader = null;

            CleanResources();

            mLoader = ResLoader.Allocate();
            mName = FileName;
            mIsLoop = loop;
            mAudioSourceCache = true;
            this.autoPlay = autoPlay;

            if (FileName.IsLocalAudioPath())
            {
                mLoader.Add2Load(FileName, ResLoadType.LocalAudio,  OnResLoadFinish);

                mName = FileName.Substring(11);
            }
            else {
                mLoader.Add2Load(null, FileName,  OnResLoadFinish );
            }

            
            if (preLoader != null)
            {
                preLoader.Recycle2Cache();
                preLoader = null;
            }

            if (mLoader != null)
            {
                mLoader.LoadAsync();
            }
            Log.I("调用播放了");
        }


        public void SetNetAudio(AudioSource audioSource, string url, AudioType audioType,bool loop = false, bool autoPlay = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            if (mName == url)
            {
                return;
            }

            if (audioSource == null)
            {
                return;
            }

            if (mAudioSource == null)
            {
                mAudioSource = audioSource;
            }
            //
            var preLoader = mLoader;

            mLoader = null;

            CleanResources();

            mLoader = ResLoader.Allocate();

            mIsLoop = loop;
            mName = url;

            mAudioSourceCache = true;
            this.autoPlay= autoPlay;

            var at=  mLoader.GetResLoadType4AudioType(audioType);
            mLoader.Add2Load(url, at,  OnResLoadFinish );
            
            if (preLoader != null)
            {
                preLoader.Recycle2Cache();
                preLoader = null;
            }

            if (mLoader != null)
            {
                mLoader.LoadAsync();
            }
        }

        public void Stop()
        {
            Release();
        }
        public void Pause()
        {
            if (mIsPause)
            {
                return;
            }

            mLeftDelayTime = -1;
            //
            if (mTimeItem != null)
            {
                mLeftDelayTime = mTimeItem.SortScore - Timer.Instance.currentScaleTime;
                mTimeItem.Cancel();
                mTimeItem = null;
            }

            mIsPause = true;

            mAudioSource?.Pause();

         }

        public void Resume()
        {
            if (!mIsPause)
            {
                return;
            }

            if (mLeftDelayTime >= 0)
            {
                mTimeItem = Timer.Instance.Post2Scale(OnResumeTimeTick, mLeftDelayTime);
            }

            mIsPause = false;

            mAudioSource?.UnPause();
            
        }
        private void OnResumeTimeTick(int repeatCount)
        {
            OnSoundPlayFinish(repeatCount);

            if (mIsLoop)
            {


                mTimeItem = Timer.Instance.Post2Scale(OnSoundPlayFinish, mAudioClip.length, -1);
            }
        }

        public void SetVolume(float volume)
        {
            if (null != mAudioSource)
            {
                mAudioSource.volume = volume;
            }
        }

        /// <summary>
        /// 改方法只能正常播放一次，不能在调用一次后再次调用（调用直接return）
        /// 应用环境: videoPlayer 不想在加载后直接播放
        /// </summary>
        public void PlayAudioClip()
        {
            //在不值得的情况下 且当前音频未加载成功
            if (!autoPlay&&!isLoaded&& !canPlayAudioWhenNotAuto)
            {
                canPlayAudioWhenNotAuto = true;
                return;
            }
            //PlayAudioClip 只能播放一次
            if (mIsPause || mTimeItem != null) return;

            if (mAudioSource == null || mAudioClip == null )
            {
                Release();
                return;
            }


            mAudioSource.clip = mAudioClip;
            mAudioSource.loop = mIsLoop;
            mAudioSource.volume = 1.0f;
            int loopCount = 1;
            if (mIsLoop)
            {
                loopCount = -1;
            }

            mTimeItem = Timer.Instance.Post2Scale(OnSoundPlayFinish, mAudioClip.length, loopCount);
            Log.I("Time Post2Scale === Time:" + mAudioClip.length);
            if (null != mOnStartListener)
            {
                mOnStartListener(this);
            }

             mAudioSource.Play();
        }

        private void OnSoundPlayFinish(int count)
        {
         
            ++mPlayCount;
            
            Debug.Log("OnSoundPlayFinish");
            if (mOnFinishListener != null)
            {
                mOnFinishListener(this);
            }

            if (!mIsLoop)
            {
                Release();
            }

        }
        private void OnResLoadFinish(bool result, IRes res)
        {
            if (!result)
            {
                Release();
                return;
            }

            mAudioClip = res.Asset as AudioClip;

            if (mAudioClip == null)
            {
                Log.E("Asset Is Invalid AudioClip:" + mName);
                Release();
                return;
            }
            //Debug.Log("OnResLoadFinish:  IsRecycled:" + IsRecycled+ "________AssetName:"+ res.AssetName + "___Name:"+ Name);

            //if (IsRecycled|| !res.AssetName.ToLower().Contains(Name.ToLower()))
            //{
            //    Release();
            //    return;
            //}

            isLoaded = true;

            //在自动播放的逻辑中直接播放，在不自动的逻辑中 
            if (autoPlay|| (canPlayAudioWhenNotAuto&& !autoPlay)) PlayAudioClip();

        }


        private void Release()
        {

            CleanResources();

            if (mUsedCache)
            {
                Recycle2Cache();
            }
        }

        private void CleanResources()
        {
            mName = null;

            mPlayCount = 0;
            mIsPause = false;
            mOnFinishListener = null;
            mLeftDelayTime = -1;
            canPlayAudioWhenNotAuto = false;
            isLoaded=false;
            autoPlay = true;
            //mCustomEventID = -1;

            if (mTimeItem != null)
            {
                mTimeItem.Cancel();
                mTimeItem = null;
            }

            if (mAudioSource != null)
            {
                if (mAudioSource.clip == mAudioClip)
                {
                    mAudioSource.Stop();
                    mAudioSource.clip = null;
                }
            }
            mAudioClip = null;
            if (mLoader != null)
            {
                mLoader.Recycle2Cache();
                mLoader = null;
            }
        }

        public void OnRecycled()
        {
            CleanResources();

            mUsedCache = true;
        }

        public void Recycle2Cache()
        {
            if (!SafeObjectPool<AudioPlayer>.Instance.Recycle(this))
            {
                if (mAudioSource != null &&!mAudioSourceCache )
                {
                    GameObject.Destroy(mAudioSource);
                    mAudioSource = null;
                }
            }
        }
    }
}
