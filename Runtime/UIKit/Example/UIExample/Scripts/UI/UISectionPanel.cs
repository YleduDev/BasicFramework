using UnityEngine.UI;

namespace Framework.Example
{
    public class UISectionPanelData : UIPanelData
    {
        // TODO: Query Mgr's Data
    }

    public partial class UISectionPanel : UIPanel
    {
        private const int SectionNumber = 4;

        private readonly Button[] mButtons = new Button[SectionNumber];

        protected override void OnInit(IUIData uiData = null)
        {
            for (var i = 0; i < mButtons.Length; i++)
            {
                mButtons[i] = SectionBtn
                    .Instantiate()
                    .Parent(SectionBtn.transform.parent)
                    .LocalScaleIdentity()
                    .LocalPositionIdentity()
                    .Name("Section" + (i + 1))
                    .Show()
                    .ApplySelfTo(selfBtn => selfBtn.GetComponentInChildren<Text>().text = selfBtn.name)
                    .ApplySelfTo(selfBtn =>
                    {
                        var index = i;
                        selfBtn.onClick.AddListener(() => { ChoiceSection(index); });
                    });
            }

            SettingBtn.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UISettingPanel>(UILevel.PopUI, prefabName: "Resources/UISettingPanel");
            });

            BackBtn.onClick.AddListener(() =>
            {
                CloseSelf();
                UIKit.OpenPanel<UIMenuPanel>(UILevel.Common, prefabName: "Resources/UIMenuPanel");
            });
        }


        private void ChoiceSection(int i)
        {
            UIKit.HidePanel(name);
            UIKit.OpenPanel<UIGamePanel>(UILevel.Common, new UIGamePanelData {SectionNo = i + 1},
                prefabName: "Resources/UIGamePanel");
        }

        protected override void OnClose()
        {
        }
    }
}