using System;

namespace Framework
{
    /// <summary>
    /// UI√Ê∞Â Ù–‘
    /// </summary>
    public class PanelInfo : IPoolType, IPoolable
    {
        public IUIData UIData;

        public UILevel Level = UILevel.Common;

        public string AssetBundleName;

        public string GameObjName;

        public Type PanelType;

        public static PanelInfo Allocate(string gameObjName, UILevel level, IUIData uiData, Type panelType,
            string assetBundleName)
        {
            var panelInfo = SafeObjectPool<PanelInfo>.Instance.Allocate();

            panelInfo.GameObjName = gameObjName;
            panelInfo.Level = level;
            panelInfo.UIData = uiData;
            panelInfo.PanelType = panelType;
            panelInfo.AssetBundleName = assetBundleName;
            return panelInfo;
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<PanelInfo>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            UIData = null;
            AssetBundleName = null;
            GameObjName = null;
            PanelType = null;
        }

        public bool IsRecycled { get; set; }
    }
}