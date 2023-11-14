using System;
using System.Reflection;
using UnityEngine;
namespace Framework
{
    public class ILKitBehaviour : ILComponentBehaviour
    {
        /* ����IL�����ʹ���@��
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

        //�]����IL �Ŀ������@���������÷���ģ��������A�u�w�Ϲ�ILNeedCallBehaviour�󣬄h��ILKitBehaviour
        // �ք� ����UI���� UITYPE.Start(���ɵ�ui����Behavoiur����)
        //protected virtual void Awake()
        //{
        //    //��Ϊios �ķ��� ֻ�� ����д�����ԣ�Ŀǰû�����ȸ�����£������÷��� ���ɽű�
        //    //���� IL �����ʹ��il �ӿڼ���
        //    Script = CreateInstance(Namespace, ScriptName);
        //    Debug.LogError("ILKitBehaviour");
        //    Script?.InvokeByReflect("Start", this);
        //}


        public static object CreateInstance(string nameSpace, string className)
        {
            try
            {
                string fullName = nameSpace + "." + className;//�����ռ�.������
                var ass = Assembly.GetExecutingAssembly();
                //��Ϊ��һ��д��
                object ect = ass.CreateInstance(fullName);//���س��򼯣�������������� �����ռ�.������ ʵ��
                return ect;//����ת��������
            }
            catch
            {
                //�����쳣���������͵�Ĭ��ֵ
                return null;
            }
        }
    }
}
