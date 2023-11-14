using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace  Framework
{
    /// <summary>
    /// 资源卸载 Helper
    /// </summary>
    public static class ResUnloadHelper
    {
        public static void UnloadRes(Object asset)
        {
            if (asset is GameObject)
            {
            }
            else
            {
                Resources.UnloadAsset(asset);
            }
        }

        public static void DestroyObject(Object asset)
        {
            Object.Destroy(asset);
        }
    }
}