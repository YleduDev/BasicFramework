using System;
using System.Reflection;
using UnityEngine;
namespace Framework
{
    public class ILKitBehaviour : ILComponentBehaviour
    {
        /* 接入IL后可以使用這套
        private void Awake()
        {
            if (ScriptKit.Script == null)
            {
                ScriptKit.Script = new ILRuntimeScript();

                ScriptKit.LoadScript(() =>
                {
                    ScriptKit.CallStaticMethod(Namespace + "." + ScriptName, "Start", this);
                });
            }
            else
            {
                ScriptKit.CallStaticMethod(Namespace + "." + ScriptName, "Start", this);
            }
        }


        void OnApplicationQuit()
        {
#if UNITY_EDITOR
            if (ScriptKit.Script != null)
            {
                ScriptKit.Dispose();
            }
#endif
        }
        
    */

        //沒有用IL 的可以用這個，不想用反射的，可以在預製體上挂ILNeedCallBehaviour后，刪除ILKitBehaviour
        // 手動 生成UI面板后， UITYPE.Start(生成的ui對象Behavoiur對象)
        //protected virtual void Awake()
        //{
        //    //因为ios 的反射 只读 不可写，所以，目前没接入热更情况下，可以用反射 生成脚本
        //    //接入 IL 后可以使用il 接口加载
        //    Script = CreateInstance(Namespace, ScriptName);
        //    Debug.LogError("ILKitBehaviour");
        //    Script?.InvokeByReflect("Start", this);
        //}


        public static object CreateInstance(string nameSpace, string className)
        {
            try
            {
                string fullName = nameSpace + "." + className;//命名空间.类型名
                var ass = Assembly.GetExecutingAssembly();
                //此为第一种写法
                object ect = ass.CreateInstance(fullName);//加载程序集，创建程序集里面的 命名空间.类型名 实例
                return ect;//类型转换并返回
            }
            catch
            {
                //发生异常，返回类型的默认值
                return null;
            }
        }
    }
}
