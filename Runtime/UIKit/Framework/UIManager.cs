using System.Linq;
using UnityEngine;

namespace Framework
{
    [MonoSingletonPath("UIRoot/Manager")]
    public partial class UIManager : MonoBehaviour, ISingleton
    {
        void ISingleton.OnSingletonInit()
        {
        }

        private static UIManager mInstance;

        public static UIManager Instance
        {
            get
            {
                if (!mInstance)
                {
                    var uiRoot = UIRoot.Instance;
                    Debug.Log("currentUIRoot:" + uiRoot);
                    mInstance = MonoSingletonProperty<UIManager>.Instance;
                }

                return mInstance;
            }
        }

        public IPanel OpenUI(PanelSearchKeys panelSearchKeys)
        {
            if (panelSearchKeys.OpenType == PanelOpenType.Single)
            {
                var retPanel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

                if (retPanel == null)
                {
                    retPanel = CreateUI(panelSearchKeys);
                }

                retPanel.Open(panelSearchKeys.UIData);
                retPanel.Show();
                return retPanel;
            }
            else
            {
                var retPanel = CreateUI(panelSearchKeys);
                retPanel.Open(panelSearchKeys.UIData);
                retPanel.Show();
                return retPanel;
            }
        }

        /// <summary>
        /// 显示UIBehaiviour
        /// </summary>
        /// <param name="uiBehaviourName"></param>
        public void ShowUI(PanelSearchKeys panelSearchKeys)
        {
            var retPanel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

            if (retPanel != null)
            {
                retPanel.Show();
            }
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        /// <param name="uiBehaviourName"></param>
        public void HideUI(PanelSearchKeys panelSearchKeys)
        {
            var retPanel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

            if (retPanel != null)
            {
                retPanel.Hide();
            }
        }

        /// <summary>
        /// 删除所有UI层
        /// </summary>
        public void CloseAllUI()
        {
            foreach (var layer in UIKit.Table)
            {
                layer.Close();
                layer.Info.Recycle2Cache();
                layer.Info = null;
            }

            UIKit.Table.Clear();
        }

        /// <summary>
        /// 隐藏所有 UI
        /// </summary>
        public void HideAllUI()
        {
            UIKit.Table.ForEach(dataItem => dataItem.Hide());
        }

        /// <summary>
        /// 隐藏所有 UI
        /// </summary>
        public void ShowAllUI()
        {
            UIKit.Table.ForEach(dataItem => dataItem.Show());
        }
        /// <summary>
        /// 关闭并卸载UI
        /// </summary>
        /// <param name="behaviourName"></param>
        public void CloseUI(PanelSearchKeys panelSearchKeys)
        {
            var panel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).LastOrDefault();

            if (panel as UIPanel)
            {
                panel.Close();
                UIKit.Table.Remove(panel);
                panel.Info.Recycle2Cache();
                panel.Info = null;
            }
        }

        public void RemoveUI(PanelSearchKeys panelSearchKeys)
        {
            var panel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

            if (panel != null)
            {
                UIKit.Table.Remove(panel);
            }
        }

        /// <summary>
        /// 获取UIBehaviour
        /// </summary>
        /// <param name="uiBehaviourName"></param>
        /// <returns></returns>
        public UIPanel GetUI(PanelSearchKeys panelSearchKeys)
        {
            return UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault() as UIPanel;
        }

    

        public IPanel CreateUI(PanelSearchKeys panelSearchKeys)
        {
            var panel = UIKit.Config.LoadPanel(panelSearchKeys);

            UIKit.Root.SetLevelOfPanel(panelSearchKeys.Level, panel);
            var canvas = panel.Transform.GetComponent<Canvas>();

            if (!canvas)
                if (panelSearchKeys.Level != UILevel.CanvasPanel)
                    UIKit.Config.SetDefaultSizeOfPanel(panel);

            panel.Transform.gameObject.name = panelSearchKeys.GameObjName ?? panelSearchKeys.PanelType.Name;

            panel.Info = PanelInfo.Allocate(panelSearchKeys.GameObjName, panelSearchKeys.Level, panelSearchKeys.UIData,
                panelSearchKeys.PanelType, panelSearchKeys.AssetBundleName);
            
            UIKit.Table.Add(panel);

            panel.Init(panelSearchKeys.UIData);

            //if (!canvas) UIKit.Root.defaultCurvedUISettings.AddEffectToChildren();
            
            return panel;
        }
    }
}