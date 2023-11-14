using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// UI 参数对象的封装，因为可能有很多 不同面板的搜索、查找、删除等等，使用一个参数封装（对象池处理）会好好很多
    /// </summary>
    public class PanelSearchKeys : IPoolType, IPoolable
    {
        public Type PanelType;

        public string AssetBundleName;

        public string GameObjName;

        public UILevel Level = UILevel.Common;

        public IUIData UIData;
        
        
        public IPanel Panel;
        
        //默认单个的
        public PanelOpenType OpenType = PanelOpenType.Single;


        public void OnRecycled()
        {
            PanelType = null;
            AssetBundleName = null;
            GameObjName = null;
            UIData = null;
            Panel = null;
        }

        public bool IsRecycled { get; set; }


        public override string ToString()
        {
            return "PanelSearchKeys PanelType:{0} AssetBundleName:{1} GameObjName:{2} Level:{3} UIData:{4}".FillFormat(PanelType, AssetBundleName, GameObjName, Level,
                UIData);
        }

        public static PanelSearchKeys Allocate()
        {
            return SafeObjectPool<PanelSearchKeys>.Instance.Allocate();
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<PanelSearchKeys>.Instance.Recycle(this);
        }
    }
}