using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace  Framework
{
    public partial class ResKit
    {

        public static void Init()
        {
            ResMgr.Init();
        }
        
        public static IEnumerator InitAsync()
        {
            yield return ResMgr.InitAsync();
        }


        
    }
}