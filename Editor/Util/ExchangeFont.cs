using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// ui工具
/// </summary>
public class ExchangeFont : EditorWindow
{
    [MenuItem("Tools/UI/更换字体")]
    public static void Open()
    {
        EditorWindow.GetWindow(typeof(ExchangeFont), true);
    }

    public TMPro.TMP_FontAsset SelectOldFont;
    static TMPro.TMP_FontAsset OldFont;

    public TMPro.TMP_FontAsset SelectNewFont;
    static TMPro.TMP_FontAsset NewFont;

    static float NewLineSpacing;

    private void OnGUI()
    {
        SelectOldFont = (TMPro.TMP_FontAsset)EditorGUILayout.ObjectField("请选择想更换的字体", SelectOldFont, typeof(TMPro.TMP_FontAsset), true, GUILayout.MinWidth(100));
        OldFont = SelectOldFont;
        SelectNewFont = (TMPro.TMP_FontAsset)EditorGUILayout.ObjectField("请选择新的字体", SelectNewFont, typeof(TMPro.TMP_FontAsset), true, GUILayout.MinWidth(100));
        NewFont = SelectNewFont;
        NewLineSpacing = 1;
        NewLineSpacing = EditorGUILayout.FloatField("新行间距", NewLineSpacing);

        if (GUILayout.Button("更换选中的预制体"))
        {
            if (SelectOldFont == null || SelectNewFont == null)
            {
                Debug.LogError("请选择字体！");
            }
            else
            {
                Change();
            }
        }
        if (GUILayout.Button("更换文件夹下所有的预制体"))
        {
            if (SelectOldFont == null || SelectNewFont == null)
            {
                Debug.LogError("请选择字体！");
            }
            else
            {
                ChangeSelectFloud();
            }
        }
    }

    public static void Change()
    {
        Object[] Texts = Selection.GetFiltered(typeof(TMPro.TextMeshProUGUI), SelectionMode.Deep);
        Debug.Log("找到" + Texts.Length + "个Text，即将处理");
        int count = 0;
        foreach (Object text in Texts)
        {
            if (text)
            {
                TMPro.TextMeshProUGUI AimText = (TMPro.TextMeshProUGUI)text;
                Undo.RecordObject(AimText, AimText.gameObject.name);
                if (AimText.font == OldFont)
                {
                    AimText.font = NewFont;
                    AimText.lineSpacing = NewLineSpacing;
                    //Debug.Log(AimText.name + ":" + AimText.text);
                    EditorUtility.SetDirty(AimText);
                    count++;
                }
            }
        }
        Debug.Log("字体更换完毕！更换了" + count + "个");
    }

    public static void ChangeSelectFloud()
    {

        object[] objs = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
        for (int i = 0; i < objs.Length; i++)
        {
            string ext = System.IO.Path.GetExtension(objs[i].ToString());
            if (!ext.Contains(".GameObject"))
            {
                continue;
            }
            GameObject go = (GameObject)objs[i];
            var Texts = go.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
            int count = 0;
            foreach (TMPro.TextMeshProUGUI text in Texts)
            {
                Undo.RecordObject(text, text.gameObject.name);
                if (text.font == OldFont)
                {
                    text.font = NewFont;
                    text.lineSpacing = NewLineSpacing;
                    EditorUtility.SetDirty(text);
                    count++;
                }
            }
            if (count > 0)
            {
                AssetDatabase.SaveAssets();
                Debug.Log(go.name + "界面有:" + count + "个Arial字体");
            }

        }
    }

}