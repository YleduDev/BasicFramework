namespace Framework.Example
{
	public class UIMenuPanelData : IUIData
	{
		// TODO: Query
	}

	public partial class UIMenuPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			ImageBg.color = "#FFFFFFFF".HtmlStringToColor();

			UIKit.GetPanel<UIMenuPanel>().LogInfo();

			BtnPlay.onClick.AddListener(() =>
			{
				CloseSelf();
				UIKit.OpenPanel<UISectionPanel>(UILevel.Common, prefabName: "Resources/UISectionPanel");
				// this.DoTransition<UISectionPanel>(new FadeInOut(), UILevel.Common,
				//prefabName: "Resources/UISectionPanel");
			});

			BtnSetting.onClick.AddListener(() =>
			{
				UIKit.OpenPanel<UISettingPanel>(UILevel.PopUI,
					prefabName: "Resources/UISettingPanel");
			});
			
		}


		protected override void OnClose()
		{

		}
	}
}