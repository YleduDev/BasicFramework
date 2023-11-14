
using UnityEngine;

namespace Framework.Example
{
	public class UISettingPanelData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UISettingPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
            //please add init code here

           // EventBtn.onClick.AddListener(() => { SendEvent(UIEventID.MenuPanel.ChangeMenuColor); });

            BackBtn.onClick.AddListener(() => { CloseSelf(); });

            //this.SendMsg(new AudioMusicMsg(
            //	"GameBg",
            //	loop: false,
            //	allowMusicOff: false,
            //	onMusicBeganCallback: () => { Debug.Log("Music Start"); },
            //	onMusicEndedCallback: () => { Debug.Log("MusicEnd"); })
            //);

            //this.SendMsg(new AudioMusicMsg("HomeBg"));
        }
        

		protected override void OnClose()
		{
		}
	}
}