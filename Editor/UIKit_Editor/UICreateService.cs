using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

namespace Framework.Editor
{
    public class UICreateService
    {
        public static void CreatUIManager(Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
        {

            //UIRoot
            GameObject UIManagerGo = new GameObject("UIRoot");
            UIManagerGo.layer = LayerMask.NameToLayer("UI");

            UIRoot UIRoot = UIManagerGo.AddComponent<UIRoot>();

            CreateUICamera(UIRoot, 99, referenceResolution, MatchMode, isOnlyUICamera, isVertical);

            ProjectWindowUtil.ShowCreatedAsset(UIManagerGo);

            //保存UIManager
            ReSaveUIManager(UIManagerGo);
        }

        public static void CreateUICamera(UIRoot UIRoot, float cameraDepth, Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
        {

            GameObject UIManagerGo = UIRoot.gameObject;
            UIManagerGo.AddComponent<RectTransform>();

            var sObj = new SerializedObject(UIRoot);

            //挂载点
            GameObject goTmp = null;
            RectTransform rtTmp = null;

            goTmp = new GameObject("Bg");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("Bg").objectReferenceValue = rtTmp.gameObject;


            //goTmp = new GameObject("AnimationUnder");
            //goTmp.layer = LayerMask.NameToLayer("UI");
            //goTmp.transform.SetParent(UIManagerGo.transform);
            //goTmp.transform.localScale = Vector3.one;
            //rtTmp = goTmp.AddComponent<RectTransform>();
            //rtTmp.anchorMax = new Vector2(1, 1);
            //rtTmp.anchorMin = new Vector2(0, 0);
            //rtTmp.anchoredPosition3D = Vector3.zero;
            //rtTmp.sizeDelta = Vector2.zero;

            //sObj.FindProperty("mAnimationUnderTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("Common");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("Common").objectReferenceValue = rtTmp.gameObject;


            //goTmp = new GameObject("AnimationOn");
            //goTmp.layer = LayerMask.NameToLayer("UI");
            //goTmp.transform.SetParent(UIManagerGo.transform);
            //goTmp.transform.localScale = Vector3.one;
            //rtTmp = goTmp.AddComponent<RectTransform>();
            //rtTmp.anchorMax = new Vector2(1, 1);
            //rtTmp.anchorMin = new Vector2(0, 0);
            //rtTmp.anchoredPosition3D = Vector3.zero;
            //rtTmp.sizeDelta = Vector2.zero;

            //sObj.FindProperty("mAnimationOnTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("PopUI");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("PopUI").objectReferenceValue = rtTmp.gameObject;


            //goTmp = new GameObject("Const");
            //goTmp.layer = LayerMask.NameToLayer("UI");
            //goTmp.transform.SetParent(UIManagerGo.transform);
            //goTmp.transform.localScale = Vector3.one;
            //rtTmp = goTmp.AddComponent<RectTransform>();
            //rtTmp.anchorMax = new Vector2(1, 1);
            //rtTmp.anchorMin = new Vector2(0, 0);
            //rtTmp.anchoredPosition3D = Vector3.zero;
            //rtTmp.sizeDelta = Vector2.zero;
            //sObj.FindProperty("mConstTrans").objectReferenceValue = rtTmp.gameObject;


            //goTmp = new GameObject("Toast");
            //goTmp.layer = LayerMask.NameToLayer("UI");
            //goTmp.transform.SetParent(UIManagerGo.transform);
            //goTmp.transform.localScale = Vector3.one;
            //rtTmp = goTmp.AddComponent<RectTransform>();
            //rtTmp.anchorMax = new Vector2(1, 1);
            //rtTmp.anchorMin = new Vector2(0, 0);
            //rtTmp.anchoredPosition3D = Vector3.zero;
            //rtTmp.sizeDelta = Vector2.zero;

            //sObj.FindProperty("mToastTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("CanvasPanel");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            sObj.FindProperty("CanvasPanel").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("Design");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            goTmp.AddComponent<Hide>();

            goTmp = new GameObject("EventSystem");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            goTmp.AddComponent<UnityEngine.EventSystems.EventSystem>();
            goTmp.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            //UIcamera
            GameObject cameraGo = new GameObject("UICamera");
            cameraGo.layer = LayerMask.NameToLayer("UI");
            cameraGo.transform.SetParent(UIManagerGo.transform);
            cameraGo.transform.localPosition = new Vector3(0, 0, -100);
            Camera camera = cameraGo.AddComponent<Camera>();
            camera.cullingMask = LayerMask.GetMask("UI");
            camera.orthographic = true;
            camera.depth = cameraDepth;
            camera.allowHDR = false;
            camera.clearFlags = CameraClearFlags.Depth;
            cameraGo.AddComponent<FlareLayer>();
            //cameraGo.AddComponent<UnityEngine.GUILayer>();
            sObj.FindProperty("UICamera").objectReferenceValue = camera.gameObject;


            GameObject manager = new GameObject("Manager");
            manager.layer = LayerMask.NameToLayer("UI");
            manager.transform.SetParent(UIManagerGo.transform);
            var re = manager.AddComponent<RectTransform>();
            re.anchorMax = new Vector2(1, 1);
            re.anchorMin = new Vector2(0, 0);
            re.anchoredPosition3D = Vector3.zero;
            UIManager uiManager = manager.AddComponent<UIManager>();

            //Canvas
            Canvas canvasComp = UIManagerGo.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceCamera;
            canvasComp.worldCamera = camera;
            canvasComp.sortingOrder = 100;
            sObj.FindProperty("Canvas").objectReferenceValue = canvasComp.gameObject;

            //UI Raycaster
            sObj.FindProperty("GraphicRaycaster").objectReferenceValue = UIManagerGo.AddComponent<GraphicRaycaster>();



            //CanvasScaler
            CanvasScaler scaler = UIManagerGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = MatchMode;
            sObj.FindProperty("CanvasScaler").objectReferenceValue = scaler;

            if (!isOnlyUICamera)
            {
                camera.clearFlags = CameraClearFlags.Depth;
                camera.depth = cameraDepth;
            }
            else
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
            }
            scaler.matchWidthOrHeight = isVertical ? 1 : 0;

            //重新保存
            ReSaveUIManager(UIManagerGo);

            sObj.ApplyModifiedPropertiesWithoutUndo();
        }

        static void ReSaveUIManager(GameObject UIManagerGo)
        {
            string dirPath = Application.dataPath + "/Art/UI/Resources";
            string filePath = "Assets/Art/UI/Resources/UIRoot.prefab";
            Debug.Log(dirPath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            PrefabUtility.CreatePrefab(filePath, UIManagerGo, ReplacePrefabOptions.ConnectToPrefab);
        }

    }
}