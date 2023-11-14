/****************************************************
    文件：OnDestroyTrigger.cs
    日期：2020/11/10 11:18:5
	功能：所有释放性质类的 触发器（在脚本销毁时），防止没有手动注销
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
public class OnDestroyTrigger : MonoBehaviour
{
    HashSet<IDisposable> mDisposables = new HashSet<IDisposable>();

    public void AddDispose(IDisposable disposable)
    {
        if (!mDisposables.Contains(disposable)) mDisposables.Add(disposable);
    }

    private void OnDestroy()
    {
        //
        if (Application.isPlaying)
        {
            foreach (var disposable in mDisposables)
            {
                disposable?.Dispose();
            }

            mDisposables.Clear();
            mDisposables = null;
        }
    }

}
