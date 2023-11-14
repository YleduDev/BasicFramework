using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class AnimClipListener : MonoBehaviour
{
    public class TimePositionEvent : UnityEvent<AnimationEvent> { }

    const int Time_Position_Scale = 100000;

    struct ListenersKey
    {
        public AnimationClip _clip; //
        public int _timePosition; //监听某个clip的不同时间

        public ListenersKey(AnimationClip clip, float timePosition)
        {
            _clip = clip;
            _timePosition = (int)(timePosition * Time_Position_Scale);
        }
    }

    Dictionary<ListenersKey, TimePositionEvent> _listenersDict = new Dictionary<ListenersKey, TimePositionEvent>();

    public void OnEvent(AnimationEvent evt)
    {
        //Debug.Log($"OnEvent: AnimationEvent");

        var clip = evt.animatorClipInfo.clip;
        var timePosition = evt.time;
        var key = new ListenersKey(clip, timePosition);
        if (_listenersDict.TryGetValue(key, out var tpe))
        {
            tpe.Invoke(evt);
        }
    }

    public void AddListener(AnimationClip clip, float timePosition, UnityAction<AnimationEvent> l)
    {
        if (timePosition < 0)
        {
            Debug.LogError($"timePos < 0: {timePosition}");
            return;
        }
        if (clip == null) { Debug.LogError("clip dose not can be null"); return; }

        var key = new ListenersKey(clip, timePosition);
        if (!_listenersDict.TryGetValue(key, out var tpe))
        {
            tpe = new TimePositionEvent();
            _listenersDict.Add(key, tpe);
        }
        AnimationEvent evt = null;
        if (!TryGetClipAnimationEvent(clip, timePosition, out evt))
        {
            evt = new AnimationEvent();
            evt.functionName = "OnEvent";
            evt.time = timePosition;
            clip.AddEvent(evt);
        }

        tpe.AddListener(l);
    }

    public void RemoveListener(AnimationClip clip, float timePosition, UnityAction<AnimationEvent> l)
    {
        if (timePosition < 0)
            Debug.LogError($"timePos < 0: {timePosition}");
        var key = new ListenersKey(clip, timePosition);
        if (_listenersDict.TryGetValue(key, out var tpe))
        {
            tpe.RemoveListener(l);
        }
    }

    public void RemoveAllListeners(AnimationClip clip, float timePosition)
    {
        var key = new ListenersKey(clip, timePosition);
        if (_listenersDict.TryGetValue(key, out var tpe))
        {
            tpe.RemoveAllListeners();
        }
    }

    public void RemoveAll()
    {
        _listenersDict.Clear();
    }
    public void RemoveAllForClip(AnimationClip clip)
    {
        List<ListenersKey> keys = new List<ListenersKey>();
        foreach (var item in _listenersDict.Keys)
        {
            if (item._clip == clip) keys.Add(item);
        }
        for (int i = 0; i < keys.Count; i++)
        {
            _listenersDict[keys[i]].RemoveAllListeners();
        }
        _listenersDict.Clear();
    }


    public static bool TryGetAnimatorClip(Animator animator, string clipName, out AnimationClip outClip)
    {
        var clips = animator.runtimeAnimatorController.animationClips;
        for (var i = 0; i < clips.Length; ++i)
        {
            var clip = clips[i];
            if (clip.name == clipName)
            {
                outClip = clip;
                return true;
            }
        }

        outClip = null;
        return false;
    }

    public static bool TryGetAnimatorClip(AnimatorClipInfo[] clipInfo, string clipName, out AnimationClip outClip)
    {
        for (var i = 0; i < clipInfo.Length; ++i)
        {
            var clip = clipInfo[i].clip;
            if (clip.name == clipName)
            {
                outClip = clip;
                return true;
            }
        }

        outClip = null;
        return false;
    }

    static bool IsTimePositionEquals(float t1, float t2)
    {
        return ((int)t1 * Time_Position_Scale) == ((int)t2 * Time_Position_Scale);
    }

    public static bool TryGetClipAnimationEvent(AnimationClip clip, float timePosition, out AnimationEvent outEvt)
    {
        for (var i = 0; i < clip.events.Length; ++i)
        {
            var evt = clip.events[i];
            if ("OnEvent" == evt.functionName)
            {
                if (IsTimePositionEquals(evt.time, timePosition))
                {
                    outEvt = evt;
                    return true;
                }
            }
        }
        outEvt = null;
        return false;
    }

    private bool TryGetAnimationEvent(int layerIndex, string clipName, ref float timePosition, out AnimationClip clip, out AnimationEvent evt)
    {
        var animator = GetComponent<Animator>();
        var clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (TryGetAnimatorClip(clipInfo, clipName, out clip))
        {
            if (timePosition < 0)
                timePosition = clip.length;

            if (!TryGetClipAnimationEvent(clip, timePosition, out evt))
            {
                evt = new AnimationEvent();
                evt.functionName = "OnEvent";
                evt.time = timePosition;
                clip.AddEvent(evt);
            }
            return true;
        }

        clip = null;
        evt = null;
        return false;
    }


    public void AddListener(int layerIndex, string clipName, float timePosition, UnityAction<AnimationEvent> l)
    {
        if (TryGetAnimationEvent(layerIndex, clipName, ref timePosition, out var clip, out var evt))
        {
            AddListener(clip, timePosition, l);
        }
        else
        {
            Debug.LogError($"clip not exist: layer:{layerIndex}, name:{clipName}");
        }
    }

    public void AddListener01(int layerIndex, string clipName, float progress, UnityAction<AnimationEvent> l)
    {
        if (progress > 1 || progress < 0)
        {
            Debug.LogError($"{clipName}: event progress  value must ranges from 0 to 1 early ");
            return;
        }

        var animator = GetComponent<Animator>();
        var clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (TryGetAnimatorClip(clipInfo, clipName, out var clip))
        {
            progress = Mathf.Clamp01(progress);

            float timePosition = progress * clip.length;

            //防止过渡溢出
            // if ((timePosition + 0.25f) > clip.length) timePosition = clip.length - 0.25f;

            AnimationEvent evt = null;
            if (!TryGetClipAnimationEvent(clip, timePosition, out evt))
            {
                evt = new AnimationEvent();
                evt.functionName = "OnEvent";
                evt.time = timePosition;
                clip.AddEvent(evt);
            }

            AddListener(clip, timePosition, l);
        }
        else
        {
            Debug.LogError($"clip not exist: layer:{layerIndex}, name:{clipName}");
        }

    }

    public void AddListener01(AnimationClip clip, float progress, UnityAction<AnimationEvent> l)
    {
        if (progress > 1 || progress < 0)
        {
            Debug.LogError($"{clip.name}: event progress  value must ranges from 0 to 1 early ");
            return;
        }

        var animator = GetComponent<Animator>();
        if (clip != null)
        {
            progress = Mathf.Clamp01(progress);

            float timePosition = progress * clip.length;

            //防止过渡溢出
            // if ((timePosition + 0.25f) > clip.length) timePosition = clip.length -0.25f;

            AnimationEvent evt = null;
            if (!TryGetClipAnimationEvent(clip, timePosition, out evt))
            {
                evt = new AnimationEvent();
                evt.functionName = "OnEvent";
                evt.time = timePosition;
                clip.AddEvent(evt);
            }

            AddListener(clip, timePosition, l);
        }
        else
        {
            Debug.LogError($"clip not exist");
        }

    }

    public void RemoveListener01(AnimationClip clip, float progress, UnityAction<AnimationEvent> l)
    {
        if (progress > 1 || progress < 0)
        {
            Debug.LogError($"{clip.name}: event progress  value must ranges from 0 to 1 early ");
            return;
        }
        var animator = GetComponent<Animator>();
        if (clip != null)
        {
            progress = Mathf.Clamp01(progress);

            float timePosition = progress * clip.length;


            //防止过渡溢出
            // if ((timePosition + 0.25f) > clip.length) timePosition = clip.length - 0.25f;

            RemoveListener(clip, timePosition, l);
        }
        else
        {
            Debug.LogError($"clip not exist");
        }

    }

    public void RemoveListener(int layerIndex, string clipName, float timePosition, UnityAction<AnimationEvent> l)
    {
        if (TryGetAnimationEvent(layerIndex, clipName, ref timePosition, out var clip, out var evt))
        {
            RemoveListener(clip, timePosition, l);
        }
        else
        {
            Debug.LogError($"clip not exist: layer:{layerIndex}, name:{clipName}");
        }
    }

    public void RemoveAllListeners(int layerIndex, string clipName, float timePosition)
    {
        if (TryGetAnimationEvent(layerIndex, clipName, ref timePosition, out var clip, out var evt))
        {
            RemoveAllListeners(clip, timePosition);
        }
        else
        {
            Debug.LogError($"clip not exist: layer:{layerIndex}, name:{clipName}");
        }
    }

    public void RemoveAllListeners(int layerIndex, string clipName)
    {
        var animator = GetComponent<Animator>();
        var clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (TryGetAnimatorClip(clipInfo, clipName, out var clip))
        {
            RemoveAllForClip(clip);
        }
    }
}