namespace Framework
{
	using UnityEngine;

	/// <summary>
	/// 每个UIbehaviour对应的Data
	/// </summary>
	public interface IUIData
	{
	}

	public class UIPanelData : IUIData
	{
		protected UIPanel mPanel;
	}
	
	public abstract partial class UIPanel : MonoBehaviour, IPanel
	{
		public Transform Transform
		{
			get { return transform; }
		}

		UIKitConfig.IPanelLoader IPanel.Loader { get; set; }

		public PanelInfo Info { get; set; }

		public PanelState State { get; set; }

		protected IUIData mUIData;

        protected virtual void OnDestroy()
        {
            ClearUIComponents();
        }

		protected virtual void ClearUIComponents()
		{
		}

		public void Init(IUIData uiData = null)
		{
			mUIData = uiData;
			OnInit(uiData);
		}

		public void Open(IUIData uiData = null)
		{
			State = PanelState.Opening;
			OnOpen(uiData);
		}

		public virtual void Hide()
		{
			State = PanelState.Hide;
            gameObject.SetActive(false);
            OnHide();
        }

        protected virtual void OnHide() { }
        protected virtual void OnInit(IUIData uiData = null)
		{

		}

		protected virtual void OnOpen(IUIData uiData = null)
		{

		}


		/// <summary>
		/// 关闭,不允许子类调用
		/// </summary>
		void IPanel.Close(bool destroyed)
		{
			Info.UIData = mUIData;
			mOnClosed.InvokeGracefully();
			mOnClosed = null;
			Hide();
			State = PanelState.Closed;
			OnClose();

			if (destroyed)
			{
				Destroy(gameObject);
			}

			this.As<IPanel>().Loader.Unload();
			this.As<IPanel>().Loader = null;

			mUIData = null;
		}

		protected void CloseSelf()
		{
			UIKit.ClosePanel(this);
		}

		protected void Back()
		{
			UIKit.Back(name);
		}

		/// <summary>
		/// 必须使用这个
		/// </summary>
		protected abstract void OnClose();

		private System.Action mOnClosed;

		public void OnClosed(System.Action onPanelClosed)
		{
			mOnClosed = onPanelClosed;
		}

        public virtual void Show()
        {
            gameObject.SetActive(true);

            OnShow();
        }

        protected virtual void OnShow() { }
    }
}