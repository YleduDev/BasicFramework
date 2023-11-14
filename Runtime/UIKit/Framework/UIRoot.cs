using UnityEngine;
using UnityEngine.UI;
#if  UNITY_ANDROID || UNITYIOS
using UnityEngine.Rendering.Universal;
#endif


namespace Framework
{
    [MonoSingletonPath("UIRoot")]
    public class UIRoot : MonoSingleton<UIRoot>
    {
        public Camera UICamera;
        public Canvas Canvas;
        public CanvasScaler CanvasScaler;
        public GraphicRaycaster GraphicRaycaster;

        public RectTransform Bg;
        public RectTransform Common;
        public RectTransform PopUI;
        public RectTransform CanvasPanel;

        private static UIRoot mInstance;

        public static UIRoot Instance
        {
            get
            {
                if (!mInstance)
                {
                    mInstance = FindObjectOfType<UIRoot>();
                    if (mInstance)
                    {
#if UNITY_ANDROID || UNITYIOS
                        mInstance.ScreenSpaceCameraRenderMode();
                        var data = Camera.main.GetUniversalAdditionalCameraData();
                        data.cameraStack.Add(mInstance.UICamera);                                     
#elif  UNITY_STANDALONE
                        mInstance.ScreenSpaceOverlayRenderMode();
#endif


                    }

                }

                if (!mInstance)
                {
                    var go= Instantiate(Resources.Load<GameObject>("UIRoot"));
                    mInstance = MonoSingletonProperty<UIRoot>.Instance;
                    mInstance.name = "UIRoot";

#if UNITY_ANDROID || UNITYIOS
                        mInstance.ScreenSpaceCameraRenderMode();
                        var data = Camera.main.GetUniversalAdditionalCameraData();
                        data.cameraStack.Add(mInstance.UICamera);                                     
#elif  UNITY_STANDALONE
                        mInstance.ScreenSpaceOverlayRenderMode();
#endif

                    DontDestroyOnLoad(go);
                }

                return mInstance;
            }
        }


        public Camera Camera
        {
            get { return UICamera; }
        }

        public void SetResolution(int width, int height, float matchOnWidthOrHeight)
        {
            CanvasScaler.referenceResolution = new Vector2(width, height);
            CanvasScaler.matchWidthOrHeight = matchOnWidthOrHeight;
        }

        public Vector2 GetResolution()
        {
            return CanvasScaler.referenceResolution;
        }

        public float GetMatchOrWidthOrHeight()
        {
            return CanvasScaler.matchWidthOrHeight;
        }

        public void ScreenSpaceOverlayRenderMode()
        {
            Canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
            UICamera.gameObject.SetActive(false);
        }

        public void ScreenSpaceCameraRenderMode()
        {
            Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            UICamera.gameObject.SetActive(true);
            Canvas.worldCamera = UICamera;
        }


        public void SetLevelOfPanel(UILevel level, IPanel panel)
        {

            var canvas = panel.Transform.GetComponent<Canvas>();

            if (canvas)
            {
                panel.Transform.SetParent(CanvasPanel);
            }
            else
            {
                switch (level)
                {
                    case UILevel.Bg:
                        panel.Transform.SetParent(Bg);
                        break;
                    case UILevel.Common:
                        panel.Transform.SetParent(Common);
                        break;
                    case UILevel.PopUI:
                        panel.Transform.SetParent(PopUI);
                        break;
                }
            }
        }

    }
}