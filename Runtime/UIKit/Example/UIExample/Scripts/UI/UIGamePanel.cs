namespace Framework.Example
{
	public class UIGamePanelData : UIPanelData
	{
		public int SectionNo;
	}

	public partial class UIGamePanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGamePanelData;
			gameText.text = "Hello,You are in Section {0}".FillFormat(mData.SectionNo);
			
			backBtn.onClick.AddListener(() =>
			{
				UIKit.OpenPanel<UISectionPanel>(UILevel.Common, prefabName: "Resources/UISectionPanel");
				CloseSelf();
			});
		}


		
		protected override void OnClose()
		{
			
		}
	}
}