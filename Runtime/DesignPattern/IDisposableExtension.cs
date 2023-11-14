using System;
using UnityEngine;

namespace Framework
{
    public static class IDisposableExtensions
    {


        /// <summary>
        /// 与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        public static void DisposeWhenGameObjectDestroyed(this IDisposable self, GameObject gameObject)
        {
            gameObject.GetOrAddComponent<OnDestroyDisposeTrigger>()
                .AddDispose(self);
        }

        /// <summary>
        /// 与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        public static void DisposeWhenGameObjectDestroyed(this IDisposable self, Component component)
        {
            component.gameObject.GetOrAddComponent<OnDestroyDisposeTrigger>()
                .AddDispose(self);
        }
        
        
        /// <summary>
        /// 与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        public static void DisposeWhenGameObjectDisabled(this IDisposable self, GameObject gameObject)
        {
            gameObject.GetOrAddComponent<OnDisableDisposeTrigger>()
                .AddDispose(self);
        }

        /// <summary>
        /// 与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        public static void DisposeWhenGameObjectDisabled(this IDisposable self, Component component)
        {
            component.gameObject.GetOrAddComponent<OnDisableDisposeTrigger>()
                .AddDispose(self);
        }
    }
}