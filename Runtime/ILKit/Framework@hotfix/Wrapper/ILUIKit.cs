using System.Collections.Generic;
using System.Linq;
using UniRx.Triggers;
using UniRx;

namespace Framework.ILUIKit
{



    public static class ILUIKit
    {
        /* ILRuntime 
        public static void OpenPanel(string panelName)
        {
            var panel = UIKit.OpenPanel(panelName)
                .GetILComponent<ILUIPanelInterface>();
            panel.Open();
        }
        public static void OpenPanel(string panelName, UILevel uilevel)
        {
            var panel = UIKit.OpenPanel(panelName, uilevel)
                .GetILComponent<ILUIPanelInterface>();
            panel.Open();
        }
        public static void OpenPanel(string panelName, UILevel uilevel, ILUIData uiData = null)
        {
            var panel = UIKit.OpenPanel(panelName, uilevel)
                .GetILComponent<ILUIPanelInterface>();
            panel.Open(uiData);
        }

        public static T OpenPanel<T>(UILevel uilevel, ILUIData uiData = null) where T : ILUIPanelInterface, new()
        {
            var panel = UIKit.OpenPanel(typeof(T).Name, uilevel)
                .GetILComponent<T>();
            panel.Open(uiData);
            return panel;
        }

        public static T OpenPanel<T>(ILUIData uiData = null) where T : ILUIPanelInterface, new()
        {
            var panel = UIKit.OpenPanel(typeof(T).Name);
            var p =panel.GetILComponent<T>();
            p?.Open(uiData);
            return p;
        }

        public static T GetPanel<T>() where T : ILUIPanel, new()
        {
            return UIKit.GetPanel(typeof(T).Name)
                .GetILComponent<T>();
        }
        */


        #region 

        //不做堆栈 因为 泛型处理的
        // private Dictionary<string, ILUIPanel>

        public static T OpenPanel<T>(string panelName) where T : ILUIPanel, new()
        {
            if (panelName.IsNullOrEmpty()) panelName = typeof(T).Name;
            var panel = UIKit.OpenPanel(panelName);
            var ik = panel.AddILComponent<T>();
            ik.As<ILUIPanelInterface>().Open();

            return ik;
        }
        public static T OpenPanel<T>(string assetBundleName, ILUIData uiData = null) where T : ILUIPanel, new()
        {
            string panelName = typeof(T).Name;
            var panel = UIKit.OpenPanel(panelName, UILevel.Common, assetBundleName);
            var ik = panel.AddILComponent<T>();
            ik.As<ILUIPanelInterface>().Open(uiData);
            return ik;
        }


        public static T OpenPanel<T>(string assetBundleName, UILevel uILevel, ILUIData uiData = null) where T : ILUIPanel, new()
        {
            string panelName = typeof(T).Name;
            var panel = UIKit.OpenPanel(panelName, uILevel, assetBundleName);
            var ik = panel.AddILComponent<T>();
            ik.As<ILUIPanelInterface>().Open(uiData);
            return ik;
        }

        public static T OpenPanel<T>(string assetBundleName, string panelName, ILUIData uiData = null) where T : ILUIPanel, new()
        {
            if (panelName.IsNullOrEmpty()) panelName = typeof(T).Name;
            var panel = UIKit.OpenPanel(panelName, PanelOpenType.Single, UILevel.Common, assetBundleName);
            var ik = panel.AddILComponent<T>();
            ik.As<ILUIPanelInterface>().Open(uiData);
            return ik;
        }
        public static T OpenMultyPanel<T>(string assetBundleName, ILUIData uiData = null) where T : ILUIPanel, new()
        {
            string panelName = typeof(T).Name;
            var panel = UIKit.OpenPanel(panelName, PanelOpenType.Multiple, UILevel.Common, assetBundleName);
            var ik = panel.AddILComponent<T>();
            ik.As<ILUIPanelInterface>().Open(uiData);
            return ik;
        }

        public static void ClosePanel(string panelName)
        {
            var panel = UIKit.GetPanel(panelName);
            if (panel)
            {
                UIKit.ClosePanel(panelName);
                var ilPanel = panel.GetILComponent<ILUIPanelInterface>();
                //ILComponentBehaviour
                if (ilPanel == null)
                {
                    var ilBs = panel.GetComponents<ILComponentBehaviour>();
                    var ilB = ilBs.ToList().Find(item => item.Script != null);
                    ilPanel = (ILUIPanelInterface)ilB.Script;
                }
                ilPanel.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ClosePanel<T>() where T : ILUIPanel
        {
            var panelName = typeof(T).Name;
            var panel = UIKit.GetPanel(panelName);
            if (panel)
            {
                UIKit.ClosePanel(panelName);
                var ilPanel = panel.GetILComponent<ILUIPanelInterface>();
                //ILComponentBehaviour
                if (ilPanel == null)
                {
                    var ilBs = panel.GetComponents<ILComponentBehaviour>();
                    var ilB = ilBs.ToList().Find(item => item.Script != null);
                    ilPanel = (ILUIPanelInterface)ilB.Script;
                }
                ilPanel.Close();
            }
        }
        public static void CloseAllPanel()
        {
            foreach (var layer in UIKit.Table)
            {
                var scripts = layer.Transform.GetComponents<ILComponentBehaviour>();
                var script = scripts.ToList().Find(item => item.Script != null);
                if (script != null)
                {
                    var ilPanel = (ILUIPanelInterface)script.Script;
                    ilPanel.Close();
                }
            }
            UIKit.CloseAllPanel();
        }

        #endregion


        public static T GetPanel<T>(string panelName = null) where T : ILUIPanel
        {
            if (panelName.IsNullOrEmpty()) panelName = typeof(T).Name;
            var panel = UIKit.GetPanel(panelName);
            ILUIPanel ilPanel = null;
            if (panel)
            {
                ilPanel = panel.GetILComponent<ILUIPanel>();
                //防止挂载了2个或多个ILComponentBehaviour
                if (ilPanel == null)
                {
                    var ilBs = panel.GetComponents<ILComponentBehaviour>();
                    var ilB = ilBs.ToList().Find(item => item.Script != null);
                    ilPanel = (ILUIPanel)ilB.Script;
                }
            }
            return (T)ilPanel;
        }

        public static ILUIPanel GetPanel(string panelName)
        {
            var panel = UIKit.GetPanel(panelName);
            ILUIPanel ilPanel = null;
            if (panel)
            {
                ilPanel = panel.GetILComponent<ILUIPanel>();
                //��ֹ ��2��ILComponentBehaviour
                if (ilPanel == null)
                {
                    var ilBs = panel.GetComponents<ILComponentBehaviour>();
                    var ilB = ilBs.ToList().Find(item => item.Script != null);
                    ilPanel = (ILUIPanel)ilB.Script;
                }
            }
            return ilPanel;
        }

        //public static void CloseSelf<T>(this T self) where T : ILUIPanel
        //{
        //    ClosePanel<T>();
        //}
    }

}
namespace Framework
{
    public interface ILUIPanelInterface : ICanGetILComponentFromGameObject
    {
        void Open(ILUIData uiData = null);
        void Close();
        void Show();
        void Hide(System.Action callback = null);
    }

    public interface ILUIData
    {
    }



    
    public abstract class ILUIPanel : ILComponent, ILUIPanelInterface
    {
        protected  override void OnStart()
        {
        }

        protected sealed override void OnDestroy()
        {
        }

        protected virtual string Source() {
            return "";
        }


        /// <summary>
        /// 上报参数
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string,string> StatisticsPramas()
        {
            return null;
        }


        protected abstract void OnOpen(ILUIData uiData = null);

        protected abstract void OnClose();

        void ILUIPanelInterface.Open(ILUIData uiData = null)
        {
            OnOpen(uiData);

        }

        void ILUIPanelInterface.Close()
        {
            OnClose();
        }

        public virtual void Show()
        {
            gameObject?.Show();
        }

        public virtual void Hide(System.Action callback=null)
        {
            gameObject?.Hide();
            callback?.Invoke();
        }
    }
    

    public static class ILUIKitExtensions
    {
        //默认的ui ab 包名称
        public const string panelABName = "uipanel";

        public static T OpenPanel<T>(ILUIData uiData = null) where T : ILUIPanel, new()
        {
            return ILUIKit.ILUIKit.OpenPanel<T>(panelABName, uiData);
        }

        public static T OpenPanel<T>() where T : ILUIPanel, new()
        {
            return ILUIKit.ILUIKit.OpenPanel<T>(panelABName, null);
        }

        public static T OpenPanel<T>(UILevel uILevel = UILevel.Common) where T : ILUIPanel, new()
        {
            return ILUIKit.ILUIKit.OpenPanel<T>(panelABName, uILevel, null);
        }

        public static T OpenMultyPanel<T>(ILUIData uiData = null) where T : ILUIPanel, new() 
        {
            return ILUIKit.ILUIKit.OpenMultyPanel<T>(panelABName, uiData);
        }

        public static void ClosePanel<T>() where T : ILUIPanel, new()
        {
            GetPanel<T>()?.Hide(() => { ILUIKit.ILUIKit.ClosePanel<T>(); });
        }

        public static void ClosePanel(string panelName)
        {
            GetPanel(panelName)?.Hide(() => { ILUIKit.ILUIKit.ClosePanel(panelName); });
        }

        public static T GetPanel<T>() where T : ILUIPanel, new()
        {
            return ILUIKit.ILUIKit.GetPanel<T>();
        }
        public static ILUIPanel GetPanel(string panelName) 
        {
            return ILUIKit.ILUIKit.GetPanel(panelName);
        }

        public static void ShowPanel<T>() where T : ILUIPanel, new()
        {
            var panel = GetPanel<T>();
            if (panel != null)
            {
                panel.Show();
            }
        }

        public static void ShowPanel(string  panelName)
        {
            var panel = GetPanel(panelName);
            if (panel != null)
            {
                panel.Show();
            }
        }

        public static void HiedPanel<T>(System.Action callBack=null) where T : ILUIPanel, new()
        {
            var panel = GetPanel<T>();
            if (panel != null)
            {
                panel.Hide(callBack);
            }
        }

        public static void HiedPanel(string panelName, System.Action callBack=null)
        {
            var panel = GetPanel(panelName);
            if (panel != null)
            {
                panel.Hide(callBack);
            }
        }

        public static void CloseSelf<T>(this T self) where T : ILUIPanel, new()
        {
            ClosePanel<T>();
        }
    }
}