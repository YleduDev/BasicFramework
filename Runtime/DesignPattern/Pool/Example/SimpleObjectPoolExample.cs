/****************************************************
    文件：SimpleObjectPoolExample.cs
    日期：2020/11/26 11:43:56
	功能:一般对象池 示例
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace Framework.Example
{
    public class UIObject:MonoBehaviour
    {
        GameObject uiObject;
        //Text tex;
        public void Init()
        {
            uiObject = new GameObject();
            uiObject.name = "初始化";
            uiObject.transform.position = Vector3.zero;
            uiObject.transform.SetParent(transform);
            uiObject.SetActive(false);
        }
        public void Show()
        {
            uiObject.SetActive(true);
        }

        public void Hide()
        {
            uiObject.SetActive(false);
        }
    }

    public class SimpleObjectPoolExample : MonoBehaviour 
    {
        SimpleObjectPool<UIObject> uiPool;
        private void Start()
        {
            //初始化 100个
            uiPool = new SimpleObjectPool<UIObject>(() => {
                var ui= new UIObject();
                ui.transform.SetParent(this.transform);
                ui.Init();
                return ui;
            }, u=> u.Hide(), 100);



            Debug.Log(string.Format( "Pool.CurCount:{0}", uiPool.CurCount));

            var one = uiPool.Allocate();

            Debug.Log(string.Format("Pool.CurCount:{0}", uiPool.CurCount));

            uiPool.Recycle(one);

            Debug.Log(string.Format("Pool.CurCount:{0}", uiPool.CurCount));

            for (int i = 0; i < 10; i++)
            {
                uiPool.Allocate();
            }

            Debug.Log(string.Format("Pool.CurCount:{0}", uiPool.CurCount));
        }

    }
}