#if UNITY_5_6_OR_NEWER

using UnityEngine;

namespace Framework
{
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        protected static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null )
                {
                    mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
                }
                return mInstance;
            }
        }

        public virtual void OnSingletonInit()
        {
        }

        public virtual void Dispose()
        {
            if (MonoSingletonCreator.IsUnitTestMode)
            {
                var curTrans = transform;
                do
                {
                    var parent = curTrans.parent;
                    DestroyImmediate(curTrans.gameObject);
                    curTrans = parent;
                } while (curTrans != null);

                mInstance = null;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected static bool mOnApplicationQuit = false;

        protected virtual void OnApplicationQuit()
        {
            mOnApplicationQuit = true;
            if (mInstance == null) return;
            Destroy(mInstance.gameObject);
            mInstance = null;
        }

        protected virtual void OnDestroy()
        {
            mInstance = null;
        }

        public static bool IsApplicationQuit
        {
            get { return mOnApplicationQuit; }
        }
    }

    

}
#endif