// Generate Id:32477c6b-0fbb-47e3-9cf3-0e12797491c0
using UnityEngine;
using Framework;
using Framework.Example;

namespace Framework
{
	public partial class UITestPanel
	{
		public const string NAME = "UITestPanel";

		public UnityEngine.UI.Image Image;
		public UITestElement UITestElement;
		public Transform  transform  { get; set; }
		public GameObject gameObject { get; set; }
		public  ILKitBehaviour MonoBehaviour { get; set; }

		public static void Start(ILKitBehaviour ilkitBehaviour)
		{
			var ilBehaviour = new UITestPanel
			{
				transform = ilkitBehaviour.transform,
				gameObject = ilkitBehaviour.gameObject,
				MonoBehaviour = ilkitBehaviour
			};

			ilkitBehaviour.Script = ilBehaviour;

			ilBehaviour.SetupBinds();
			ilBehaviour.OnStart();

			ilkitBehaviour.OnDestroyAction += ilBehaviour.DestroyScript;
		}

		void SetupBinds()
		{
			Image = transform.Find("Image").GetComponent<UnityEngine.UI.Image>();
			UITestElement = transform.Find("UITestElement").GetComponent<UITestElement>();
		}

		void ClearBinds()
		{
			Image = null;
			UITestElement = null;
		}

		void DestroyScript()
		{
			OnDestroy();

			ClearBinds();

			transform = null;
			gameObject = null;
			MonoBehaviour = null;

			mPrivateData = null;
		}

		protected UITestPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITestPanelData());
			}
			set
			{
				mPrivateData = value;
			}
		}

		private UITestPanelData mPrivateData = null;
	}
}
