// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Framework.Example
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    
    
    // Generate Id:266a1d87-b2a4-427d-9b83-dcf5e2e60ec1
    public partial class UISettingPanel
    {
        
        public const string NAME = "UISettingPanel";
        
        [SerializeField()]
        public UnityEngine.UI.Button EventBtn;
        
        [SerializeField()]
        public UnityEngine.UI.Button BackBtn;
        
        private UISettingPanelData mPrivateData = null;
        
        public UISettingPanelData mData
        {
            get
            {
                return mPrivateData ?? (mPrivateData = new UISettingPanelData());
            }
            set
            {
                mUIData = value;
                mPrivateData = value;
            }
        }
        
        protected override void ClearUIComponents()
        {
            EventBtn = null;
            BackBtn = null;
            mData = null;
        }
    }
}
