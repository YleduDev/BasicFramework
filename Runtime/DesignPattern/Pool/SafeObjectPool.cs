/****************************************************
    文件：SafeObjectPool.cs
    日期：2020/11/9 17:53:44
	功能：安全对象池,一般管理 实列对象的优化工具
*****************************************************/

using System;
using UnityEngine;
namespace Framework
{
    public class SafeObjectPool<T>:Pool<T>,ISingleton where T:IPoolable, new()
    {
        #region Singleton

        protected SafeObjectPool()
        {
            mFactory = new DefaultObjectFactory<T>();
        }
        public void OnSingletonInit()
        {
            //mFactory = new DefaultObjectFactory<T>();
        }

        public static SafeObjectPool<T> Instance
        {
            get { return SingletonProperty<SafeObjectPool<T>>.Instance; }
        }
        public void Dispose()
        {
            SingletonProperty<SafeObjectPool<T>>.Dispose();
        }

        #endregion

        public override T Allocate()
        {
            var result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        public override bool Recycle(T obj)
        {
            if (obj == null || obj.IsRecycled)
            {
                return false;
            }

            if (mMaxCount > 0)
            {
                if (mCacheStack.Count >= mMaxCount)
                {
                    obj.OnRecycled();
                    return false;
                }
            }

            obj.IsRecycled = true;
            obj.OnRecycled();
            mCacheStack.Push(obj);

            return true;
        }


        /// <summary>
        /// 可以初始化
        /// </summary>
        /// <param name="maxCount">Max Cache count.</param>
        /// <param name="initCount">Init Cache count.</param>
        public void Init(int maxCount, int initCount)
        {
            MaxCacheCount = maxCount;

            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if (CurCount < initCount)
            {
                for (var i = CurCount; i < initCount; ++i)
                {
                    Recycle(new T());
                }
            }
        }

        /// <summary>
        /// Gets or sets the max cache count.
        /// </summary>
        /// <value>The max cache count.</value>
        public int MaxCacheCount
        {
            get { return mMaxCount; }
            set
            {
                mMaxCount = value;

                if (mCacheStack != null)
                {
                    if (mMaxCount > 0)
                    {
                        if (mMaxCount < mCacheStack.Count)
                        {
                            int removeCount = mCacheStack.Count - mMaxCount;
                            while (removeCount > 0)
                            {
                                mCacheStack.Pop();
                                --removeCount;
                            }
                        }
                    }
                }
            }
        }

    }
}