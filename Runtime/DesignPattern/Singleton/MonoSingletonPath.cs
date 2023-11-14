
using System;

namespace Framework
{
#if UNITY_5_6_OR_NEWER
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonPath : Attribute
    {
        private string mPathInHierarchy;

        public MonoSingletonPath(string pathInHierarchy)
        {
            mPathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy
        {
            get { return mPathInHierarchy; }
        }
    }
#endif
}