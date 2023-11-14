/****************************************************
    文件：SingletonProperty.cs
    日期：2020/11/9 18:7:35
	功能：将单例的构造 使用属性构成,而不是继承(因为特殊情况下 只能继承一个Class类)
 *****************************************************/

using UnityEngine;
namespace Framework
{
    public class SingletonProperty<T> where T:class,ISingleton
    {
        private static T mInstance;
        private static readonly object mLock = new object();

        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = ObjectFactory.CreateNonPublicConstructorObject<T>();
                        mInstance.OnSingletonInit();
                        return mInstance;
                    }
                }

                return mInstance;
            }
        }

        public static void Dispose()
        {
            mInstance = null;
        }
    }
}