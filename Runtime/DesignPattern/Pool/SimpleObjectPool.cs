/****************************************************
    文件：SimpleObjectPool.cs
    日期：2020/11/26 11:34:49
	功能：一般对象池
*****************************************************/

using System;
using UnityEngine;
namespace Framework
{
    public class SimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> mResetMethod;

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null, int initCount = 0)
        {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
            mResetMethod = resetMethod;

            for (int i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            if (mResetMethod != null)
            {
                mResetMethod.Invoke(obj);
            }

            mCacheStack.Push(obj);
            return true;
        }
    }
}