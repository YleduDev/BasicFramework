using UnityEngine;
using Framework;

namespace Framework
{
	public class UITestPanelData : ILUIData
	{
	}

	public partial class UITestPanel : ILUIPanel
	{
		protected override void OnOpen(ILUIData uiData = null)
		{
			mData = uiData as UITestPanelData ?? new UITestPanelData();
            // Code Here
            Image.color = Color.red;
		}

		protected override void OnClose()
		{
			
			// Close Code Here
		}
	}
}
