using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Framework.Example
{
    public class UITesePanelApp : MonoBehaviour
    {
        private void Start()
        {
            //本地資源加載
            // ILUIKit.OpenPanel("resources/UITestPanel");

            //走 AB 本地目前沒打包。
            //ILUIKit.OpenPanel("UITestPanel");
            //ILUIKit.OpenPanel<UITestPanel>();



            //不想用 ILKitBehaviour 的反射
            //就將預製體上的 ILKitBehaviour 刪除，換上 ILNeedCallBehaviour
            //var panel= UIKit.OpenPanel("resources/UITestPanel");
            //if (panel)
            //{
            //    var ilKit = panel.GetComponent<ILKitBehaviour>();
            //    UITestPanel.Start(ilKit);
            //    ilKit.Script.As<ILUIPanelInterface>().Open();
            //}

            ILUIKit.ILUIKit.OpenPanel<UITestPanel>("resources/UITestPanel");
        }

    }
}