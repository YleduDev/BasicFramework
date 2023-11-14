using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework
{
    #region Event

    public interface ITypeEventSystem
    {
        void Send<T>() where T : new();
        void Send<T>(T e);
        IUnRegister Register<T>(Action<T> onEvent);
        void UnRegister<T>(Action<T> onEvent);
    }

    public interface IUnRegister
    {
        void UnRegister();
    }

    public struct TypeEventSystemUnRegister<T> : IUnRegister
    {
        private ITypeEventSystem mTypeEventSystem;
        private Action<T> mOnEvent;

        public TypeEventSystemUnRegister(ITypeEventSystem typeEventSystem, Action<T> onEvent)
        {
            mTypeEventSystem = typeEventSystem;
            mOnEvent = onEvent;
        }

        public void UnRegister()
        {
            mTypeEventSystem.UnRegister(mOnEvent);

            mTypeEventSystem = null;

            mOnEvent = null;
        }
    }

    public class UnRegisterOnDestroyTrigger : MonoBehaviour
    {
        private HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister)
        {
            bool addSuccess = mUnRegisters.Add(unRegister);
            if (!addSuccess)
                Debug.LogWarning("Repeat Add UnRegister");
        }

        private void OnDestroy()
        {
            foreach (var unRegister in mUnRegisters)
            {
                unRegister.UnRegister();
            }

            mUnRegisters.Clear();
        }
    }

    public static class UnRegisterExtension
    {
        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            }

            trigger.AddUnRegister(unRegister);
        }
    }

    public interface IUnRegisterList : IUnRegister
    {
        void Add(IUnRegister unRegister);
    }

    public class UnRegisterList : IUnRegisterList
    {
        private readonly List<IUnRegister> mUnRegisters = new List<IUnRegister>();

        public void Add(IUnRegister unRegister)
        {
            mUnRegisters.Add(unRegister);
        }

        public void UnRegister()
        {
            foreach (var unRegister in mUnRegisters)
            {
                unRegister.UnRegister();
            }

            mUnRegisters.Clear();
        }
    }

    public static class UnRegisterListExtension
    {
        public static void AddToUnRegisterList(this IUnRegister self, IUnRegisterList unRegisterList)
        {
            unRegisterList.Add(self);
        }
    }
    /// <summary>
    /// Type 类型事件管理器（字典key 为 type 对象）
    /// </summary>
    public class TypeEventSystem : ITypeEventSystem
    {
        /// <summary>
        /// 全局变量
        /// </summary>
        public static readonly TypeEventSystem Global = new TypeEventSystem();

        public interface IRegistrations
        {
        }

        public class Registrations<T> : IRegistrations
        {
            public Action<T> OnEvent = e => { };
        }

        private Dictionary<Type, IRegistrations> mEventRegistration = new Dictionary<Type, IRegistrations>();

        public void Send<T>() where T : new()
        {
            var e = new T();
            Send(e);
        }

        public void Send<T>(T e)
        {
            var type = typeof(T);
            IRegistrations registrations;
            if (mEventRegistration.TryGetValue(type, out registrations))
            {
                (registrations as Registrations<T>).OnEvent(e);
            }
        }

        public IUnRegister Register<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations registrations;
            if (!mEventRegistration.TryGetValue(type, out  registrations))
            {
                registrations = new Registrations<T>();
                mEventRegistration.Add(type, registrations);
            }

            (registrations as Registrations<T>).OnEvent += onEvent;

            return new TypeEventSystemUnRegister<T>(this, onEvent);
        }

        public void UnRegister<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations registrations;
            if (mEventRegistration.TryGetValue(type, out  registrations))
            {
                (registrations as Registrations<T>).OnEvent -= onEvent;
            }
        }
    }

    public interface IOnEvent<T>
    {
        void OnEvent(T e);
    }

    public static class OnGlobalEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct
        {
            return TypeEventSystem.Global.Register<T>(self.OnEvent);
        }

        public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct
        {
            TypeEventSystem.Global.UnRegister<T>(self.OnEvent);
        }
    }

    #endregion
}