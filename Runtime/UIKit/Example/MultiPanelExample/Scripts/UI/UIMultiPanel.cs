using UnityEngine;
using UnityEngine.UI;

namespace Framework.Example
{
	public class UIMultiPanelData : UIPanelData
	{
		public int PageIndex { get; set; }
	}
	public partial class UIMultiPanel : UIPanel
	{
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMultiPanelData ?? new UIMultiPanelData();
			// please add init code here

			PageIndex.text = mData.PageIndex.ToString();
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
