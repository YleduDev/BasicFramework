/****************************************************
    文件：Singleton.cs
	作者：DULE
    邮箱: 1055767424@qq.com
    日期：2021/2/23 10:54:26
	功能：Nothing
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework
{
    /// <summary>
    /// 普通类的单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        /// <summary>
        /// 静态实例
        /// </summary>
        protected static T mInstance;

        /// <summary>
        /// 标签锁：确保当一个线程位于代码的临界区时，另一个线程不进入临界区。
        /// 如果其他线程试图进入锁定的代码，则它将一直等待（即被阻止），直到该对象被释放
        /// </summary>
        static object mLock = new object();

        /// <summary>
        /// 静态属性
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = SingletonCreator.CreateSingleton<T>();
                    }
                }

                return mInstance;
            }
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            mInstance = null;
        }

        /// <summary>
        /// 单例初始化方法
        /// </summary>
        public virtual void OnSingletonInit()
        {
        }
    }

    /// <summary>
    /// 普通单例创建类
    /// </summary>
    internal static class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class, ISingleton
        {
            var instance = ObjectFactory.CreateNonPublicConstructorObject<T>();
            instance.OnSingletonInit();
            return instance;
        }
    }
}