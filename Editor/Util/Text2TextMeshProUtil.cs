using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using TMPro;
public class Text2TextMeshProUtil : EditorWindow
{
    Vector2 scrollPos = Vector2.zero;
    static List<string> scriptsFolders;
    string path;
    string Path
    {
        get
        {
            return path;
        }
        set
        {
            if (value != path)
            {
                if (directoryPrefabs == null)
                {
                    directoryPrefabs = new List<string>();
                }
                else
                {
                    directoryPrefabs.Clear();
                }
                if (AllAssetPaths == null)
                {
                    AllAssetPaths = new List<string>();
                }
                else
                {
                    AllAssetPaths.Clear();
                }
                path = value;
            }
        }
    }

    static EditorWindow myWindow;
    static List<string> directoryPrefabs;
    static List<string> AllAssetPaths;
    bool showPrefabs = true;
    private static TMP_FontAsset tmp_Font = null;
    readonly string[] ReplaceMode = { "批量替换", "单个替换" };
    int replaceModeIndex;
    int ReplaceModeIndex
    {
        get
        {
            return replaceModeIndex;
        }
        set
        {
            if (value != replaceModeIndex)
            {
                if (directoryPrefabs == null)
                {
                    directoryPrefabs = new List<string>();
                }
                else
                {
                    directoryPrefabs.Clear();
                }
                if (AllAssetPaths == null)
                {
                    AllAssetPaths = new List<string>();
                }
                else
                {
                    AllAssetPaths.Clear();
                }
                Path = "";
                replaceModeIndex = value;
            }
        }
    }
    bool isAutoFixLinkedScripts = true;
    [MenuItem("Tools/Text2TextMeshPro")]
    static void Init()
    {
        myWindow = EditorWindow.GetWindow(typeof(Text2TextMeshProUtil));
        directoryPrefabs = new List<string>();
        AllAssetPaths = new List<string>();
        scriptsFolders = new List<string> { "Assets/GameMain/Scripts/UI" };
        myWindow.minSize = new Vector2(500, 300);
    }
    void OnGUI()
    {
        isAutoFixLinkedScripts = EditorGUILayout.BeginToggleGroup("修改关联脚本", isAutoFixLinkedScripts);
        ShowScriptsFolders();
        EditorGUILayout.EndToggleGroup();
        ReplaceModeIndex = GUILayout.Toolbar(ReplaceModeIndex, ReplaceMode);
        if (ReplaceModeIndex == 0)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUILayoutOption[] { GUILayout.Width(myWindow.position.width), GUILayout.Height(myWindow.position.height - scriptsFolders.Count * 20 - 70) });
            GetPath(true);
            ShowAllPrefabs();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            LoadPrefab();
            Text2TextMeshPro();
            EditorGUILayout.EndHorizontal();
        }
        else if (ReplaceModeIndex == 1)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUILayoutOption[] { GUILayout.Width(myWindow.position.width), GUILayout.Height(myWindow.position.height - scriptsFolders.Count * 20 - 70) });
            GetPath(false);
            ShowAllPrefabs();
            EditorGUILayout.EndScrollView();
            Text2TextMeshPro();
        }
    }
    void GetPath(bool isFolder = true)
    {
        Event e = Event.current;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Path:", GUILayout.Width(40));
        Path = GUILayout.TextField(Path);
        EditorGUILayout.EndHorizontal();
        if (Event.current.type == EventType.DragExited || Event.current.type == EventType.DragUpdated)
        {
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                if (Event.current.type == EventType.DragExited)
                {
                    DragAndDrop.AcceptDrag();
                    if (!isFolder)
                    {
                        if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                        {
                            Object[] objectReferences = DragAndDrop.objectReferences;
                            for (int i = 0; i < objectReferences.Length; i++)
                            {
                                int index = i;
                                if (AssetDatabase.GetAssetPath(objectReferences[index]).EndsWith(".prefab"))
                                {
                                    if (!directoryPrefabs.Contains(AssetDatabase.GetAssetPath(objectReferences[index])))
                                    {
                                        directoryPrefabs.Add(AssetDatabase.GetAssetPath(objectReferences[index]));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                        {
                            if (File.Exists(DragAndDrop.paths[0]))
                            {
                                EditorUtility.DisplayDialog("警告", "批量模式下请拖拽文件夹！", "确定");
                                return;
                            }
                            Path = DragAndDrop.paths[0];
                        }
                    }
                }
                e.Use();
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }
    }
    void ShowAllPrefabs()
    {
        if (directoryPrefabs != null && directoryPrefabs.Count > 0)
        {
            showPrefabs = EditorGUILayout.Foldout(showPrefabs, "显示预制体");
            if (showPrefabs)
            {
                for (int i = 0; i < directoryPrefabs.Count; i++)
                {
                    int index = i;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.SelectableLabel($"预制体路径：{directoryPrefabs[index]}");
                    if (GUILayout.Button("查看", GUILayout.Width(60)))
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(directoryPrefabs[index]));
                        Selection.activeGameObject = AssetDatabase.LoadAssetAtPath<Object>(directoryPrefabs[index]) as GameObject;
                    }
                    if (GUILayout.Button("删除", GUILayout.Width(60)))
                    {
                        directoryPrefabs.RemoveAt(index);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                if (GUILayout.Button("清空选择", GUILayout.Width(60)))
                {
                    directoryPrefabs.Clear();
                    AllAssetPaths.Clear();
                    Path = "";
                }
            }
        }
    }
    void LoadPrefab()
    {
        if (GUILayout.Button("加载预制体"))
        {
            if (!string.IsNullOrEmpty(Path))
            {
                DirectoryInfo direction = new DirectoryInfo(Path);
                FileInfo[] files = direction.GetFiles("*.prefab", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    int startindex = files[i].FullName.IndexOf("Assets");
                    string unityPath = files[i].FullName.Substring(startindex);
                    if (!directoryPrefabs.Contains(unityPath))
                    {
                        directoryPrefabs.Add(unityPath);
                    }
                }
            }
        }
    }
    void Text2TextMeshPro()
    {
        if (directoryPrefabs != null && directoryPrefabs.Count > 0)
        {
            if (GUILayout.Button("一键替换"))
            {
                for (int i = 0; i < directoryPrefabs.Count; i++)
                {
                    int index = i;
                    Text2TextMeshPro(directoryPrefabs[index]);
                    EditorUtility.DisplayProgressBar("替换进度", "当前进度", index / (float)directoryPrefabs.Count);
                }
                EditorUtility.ClearProgressBar();
            }
        }
    }
    void Text2TextMeshPro(string path)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(path);
        if (root)
        {
            Text[] list = root.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < list.Length; i++)
            {
                Text text = list[i];
                Transform target = text.transform;
                Vector2 size = text.rectTransform.sizeDelta;
                string strContent = text.text;
                Color color = text.color;
                int fontSize = text.fontSize;
                FontStyle fontStyle = text.fontStyle;
                TextAnchor textAnchor = text.alignment;
                bool richText = text.supportRichText;
                HorizontalWrapMode horizontalWrapMode = text.horizontalOverflow;
                VerticalWrapMode verticalWrapMode = text.verticalOverflow;
                bool raycastTarget = text.raycastTarget;
                GameObject.DestroyImmediate(text);

                TextMeshProUGUI textMeshPro = target.gameObject.AddComponent<TextMeshProUGUI>();
                if (tmp_Font == null)
                {
                    tmp_Font = Resources.Load<TMP_FontAsset>("MyTextMesh");
                }
                textMeshPro.rectTransform.sizeDelta = size;
                textMeshPro.text = strContent;
                textMeshPro.color = color;
                textMeshPro.fontSize = fontSize;
                textMeshPro.fontStyle = fontStyle == FontStyle.BoldAndItalic ? FontStyles.Bold : (FontStyles)fontStyle;
                switch (textAnchor)
                {
                    case TextAnchor.UpperLeft:
                        textMeshPro.alignment = TextAlignmentOptions.TopLeft;
                        break;
                    case TextAnchor.UpperCenter:
                        textMeshPro.alignment = TextAlignmentOptions.Top;
                        break;
                    case TextAnchor.UpperRight:
                        textMeshPro.alignment = TextAlignmentOptions.TopRight;
                        break;
                    case TextAnchor.MiddleLeft:
                        textMeshPro.alignment = TextAlignmentOptions.MidlineLeft;
                        break;
                    case TextAnchor.MiddleCenter:
                        textMeshPro.alignment = TextAlignmentOptions.Midline;
                        break;
                    case TextAnchor.MiddleRight:
                        textMeshPro.alignment = TextAlignmentOptions.MidlineRight;
                        break;
                    case TextAnchor.LowerLeft:
                        textMeshPro.alignment = TextAlignmentOptions.BottomLeft;
                        break;
                    case TextAnchor.LowerCenter:
                        textMeshPro.alignment = TextAlignmentOptions.Bottom;
                        break;
                    case TextAnchor.LowerRight:
                        textMeshPro.alignment = TextAlignmentOptions.BottomRight;
                        break;
                }
                textMeshPro.richText = richText;
                if (verticalWrapMode == VerticalWrapMode.Overflow)
                {
                    textMeshPro.enableWordWrapping = true;
                    textMeshPro.overflowMode = TextOverflowModes.Overflow;
                }
                else
                {
                    textMeshPro.enableWordWrapping = horizontalWrapMode == HorizontalWrapMode.Overflow ? false : true;
                }
                textMeshPro.raycastTarget = raycastTarget;
            }
        }
        PrefabUtility.SaveAsPrefabAsset(root, path, out bool success);
        if (!success)
        {
            Debug.LogError($"预制体：{path} 保存失败!");
        }
        if (isAutoFixLinkedScripts)
        {
            ChangeScriptsText2TextMeshPro();
        }
    }
    void ShowScriptsFolders()
    {
        for (int i = 0; i < scriptsFolders.Count; i++)
        {
            int index = i;
            EditorGUILayout.BeginHorizontal();
            SelectScriptsFolder(index);
            EditorGUILayout.EndHorizontal();
        }
    }
    void SelectScriptsFolder(int index)
    {
        Event e = Event.current;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"scriptsFolder{index + 1}:", GUILayout.Width(80));
        scriptsFolders[index] = GUILayout.TextField(scriptsFolders[index], GUILayout.Width(200));
        if (Event.current.type == EventType.DragExited || Event.current.type == EventType.DragUpdated)
        {
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                if (Event.current.type == EventType.DragExited)
                {
                    DragAndDrop.AcceptDrag();
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        if (File.Exists(DragAndDrop.paths[0]))
                        {
                            EditorUtility.DisplayDialog("警告", "请选择文件夹！", "确定");
                            return;
                        }
                        scriptsFolders[index] = DragAndDrop.paths[0];
                    }
                }
                e.Use();
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }
        if (GUILayout.Button("添加路径"))
        {
            scriptsFolders.Add(scriptsFolders[scriptsFolders.Count - 1]);
        }
        if (GUILayout.Button("删除路径"))
        {
            if (scriptsFolders.Count == 1)
            {
                EditorUtility.DisplayDialog("警告", "仅剩最后一个文件夹，删除将会出错！", "确定");
                return;
            }
            scriptsFolders.RemoveAt(index);
        }

        EditorGUILayout.EndHorizontal();

    }
    void ChangeScriptsText2TextMeshPro()
    {
        AllAssetPaths.Clear();
        Debug.LogError(directoryPrefabs.Count);
        if (directoryPrefabs != null && directoryPrefabs.Count > 0)
        {
            for (int i = 0; i < directoryPrefabs.Count; i++)
            {
                int index = i;
                string scriptsName = System.IO.Path.GetFileNameWithoutExtension(directoryPrefabs[index]);
                string[] tmp = AssetDatabase.FindAssets($"{scriptsName} t:Script", scriptsFolders.ToArray());
                if (tmp != null && tmp.Length > 0)
                {
                    AllAssetPaths.AddRange(tmp);
                }
            }
            for (int i = 0; i < AllAssetPaths.Count; i++)
            {
                int index = i;
                ChangeScriptsText2TextMeshPro(AssetDatabase.GUIDToAssetPath(AllAssetPaths[index]));
            }
        }
        AssetDatabase.Refresh();
    }
    void ChangeScriptsText2TextMeshPro(string script)
    {
        StreamReader sr = new StreamReader(script);
        string str = sr.ReadToEnd();
        sr.Close();
        str = str.Replace("<Text>", "<TMPro.TextMeshProUGUI>");
        str = str.Replace(" Text ", " TMPro.TextMeshProUGUI ");
        StreamWriter sw = new StreamWriter(script, false, System.Text.Encoding.UTF8);
        sw.Write(str);
        sw.Close();
    }
}
#endif
