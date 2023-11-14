
#if UNITY_5_6_OR_NEWER
using UnityEngine;
using Object = UnityEngine.Object;
#endif

namespace Framework
{
    /// <summary>
    /// 继承Mono的属性单例？
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if UNITY_5_6_OR_NEWER
    public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
                }

                return mInstance;
            }
        }

        public static void Dispose()
        {
            if (MonoSingletonCreator.IsUnitTestMode)
            {
                Object.DestroyImmediate(mInstance.gameObject);
            }
            else
            {
                Object.Destroy(mInstance.gameObject);
            }

            mInstance = null;
        }
    }
#endif
}