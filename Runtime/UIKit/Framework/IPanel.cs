
namespace Framework 
{
    using UnityEngine;

    /// <summary>/// PanelµÄ×´Ì¬/// </summary>
    public enum PanelState
    {
        Opening = 0,
        Hide = 1,
        Closed = 2,
    }
	
    /// <summary>
    /// IUIPanel.
    /// </summary>
    public partial interface IPanel
    {
        Transform Transform { get; }
		
        UIKitConfig.IPanelLoader Loader { get; set; }
		
        PanelInfo Info { get; set; }
		
        PanelState State { get; set; }

        void Init(IUIData uiData = null);

        void Open(IUIData uiData = null);

        void Show();

        void Hide();
		
        void Close(bool destroy = true);
    }
}