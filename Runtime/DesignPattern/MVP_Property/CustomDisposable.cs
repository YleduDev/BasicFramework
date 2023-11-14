using System;
using UnityEngine;

/// <summary>
/// 自定义释放器
/// </summary>
public class CustomDisposable : IDisposable
{
    private Action mOnDispose = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="onDispose"></param>
    public CustomDisposable(Action onDispose)
    {
        mOnDispose = onDispose;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        mOnDispose.Invoke();
        mOnDispose = null;
    }
}
public static class DisposableExtension
{
    /// <summary>
    /// 添加OnDestroyTrigger组件
    /// </summary>
    /// <param name="component">组件</param>
    public static void AddTo(this IDisposable self, Component component)
    {
        var onDestroyTrigger = component.gameObject.GetComponent<OnDestroyTrigger>();

        if (!onDestroyTrigger)
        {
            onDestroyTrigger = component.gameObject.AddComponent<OnDestroyTrigger>();
        }

        onDestroyTrigger.AddDispose(self);
    }
}
