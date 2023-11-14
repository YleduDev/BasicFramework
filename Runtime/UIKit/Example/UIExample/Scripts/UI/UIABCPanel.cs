using UnityEngine;
using UnityEngine.UI;

namespace Framework.Example
{
	public class UIABCPanelData : UIPanelData
	{
	}
	public partial class UIABCPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIABCPanelData ?? new UIABCPanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
