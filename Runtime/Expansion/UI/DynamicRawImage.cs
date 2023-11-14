using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
：
*/

/// <summary>
/// 动态图
/// </summary>
public class DynamicRawImage : RawImage
{
    private List<(Texture2D, int)> listTexture;
    public List<(Texture2D, int)> ListTexture
    {
        get { return listTexture; }
        set {
            listTexture = value;

            if (anim != null)
            {
                StopCoroutine(anim);
            }

            if (listTexture != null && listTexture.Count > 1)
            {
                //只有当前组件时活跃的情况
                if (this.enabled && gameObject.activeInHierarchy)
                {
                    anim = StartCoroutine(StartDe());
                }
            }
            else if (listTexture.Count == 1)
            {
                (Texture2D tee, int timestamp) = listTexture[0];
                this.texture= tee;
            }
            else
            {
                //this.texture = null;
            }
        }
    }

    //协场句柄
    Coroutine anim;

    /// <summary>
    /// 设置动态贴图
    /// </summary>
    /// <param name="textures"></param>
    public void SetDynamicTexture2D(List<(Texture2D, int)> textures)
    {
        if (textures != null&& textures!= ListTexture)
        {
            ListTexture = textures;
        }
    }

    //协程动画逻辑
    IEnumerator StartDe()
    {
        int prevTimestamp = 0;
        for (int i = 0; i < listTexture.Count; ++i)
        {
            (Texture2D texture, int timestamp) = listTexture[i];
            this.texture = texture;
            int delay = timestamp - prevTimestamp;
            prevTimestamp = timestamp;
            if (delay < 0)
            {
                delay = 0;
            }
            yield return new WaitForSeconds(delay * 0.001f);
            if (i == listTexture.Count - 1)
            {
                i = -1;
            }
        }
    }


    protected override void OnEnable()
    {
        if (listTexture != null && listTexture.Count > 1)
        {
            //只有当前组件时活跃的情况
            if (this.enabled && gameObject.activeInHierarchy)
            {
                anim = StartCoroutine(StartDe());
            }
        }
        else if (listTexture != null && listTexture.Count == 1)
        {
            (Texture2D tee, int timestamp) = listTexture[0];
            this.texture = tee;
        }
        else
        {
           // this.texture = null;
        }

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        if (anim != null)
        {
            StopCoroutine(anim);
        }
        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        if (anim != null)
        {
            StopCoroutine(anim);
        }
        base.OnDestroy();
    }
}
