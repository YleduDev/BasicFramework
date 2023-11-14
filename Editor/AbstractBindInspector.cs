using Assets.Scripts;
using Assets.Scripts.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomEditor(typeof(AbstractBind), true)]
    public class AbstractBindInspector : UnityEditor.Editor
    {
        class LocaleText
        {
            public static string MarkType
            {
                get { return Language.IsChinese ? " 标记类型:" : " Mark Type:"; }
            }

            public static string Type
            {
                get { return Language.IsChinese ? " 类型:" : " Type:"; }
            }

            public static string ClassName
            {
                get { return Language.IsChinese ? " 生成类名:" : " Generate Class Name:"; }
            }

            public static string Comment
            {
                get { return Language.IsChinese ? " 注释" : " Comment"; }
            }

            public static string BelongsTo
            {
                get { return Language.IsChinese ? " 属于:" : " Belongs 2:"; }
            }

            public static string Select
            {
                get { return Language.IsChinese ? "选择" : "Select"; }
            }

            public static string Generate
            {
                get { return Language.IsChinese ? " 生成代码" : " Generate Code"; }
            }
        }


        private AbstractBind mBindScript
        {
            get { return target as AbstractBind; }
        }

        private VerticalLayout mRootLayout;
        private IHorizontalLayout mComponentLine;
        private HorizontalLayout mClassnameLine;

        private void OnEnable()
        {
            mRootLayout = new VerticalLayout("box");

            EasyIMGUI.Space()
                .Parent(mRootLayout);

            var markTypeLine = EasyIMGUI.Horizontal()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.MarkType)
                .FontSize(12)
                .Width(60)
                .Parent(markTypeLine);

            var enumPopupView = new EnumPopupView(mBindScript.MarkType)
                .Parent(markTypeLine);

            enumPopupView.ValueProperty.Bind(newValue =>
            {
                mBindScript.MarkType = (BindType) newValue;

                OnRefresh();
            });


            EasyIMGUI.Space()
                .Parent(mRootLayout);

            EasyIMGUI.Custom().OnGUI(() =>
            {
                if (mBindScript.CustomComponentName == null ||
                    string.IsNullOrEmpty(mBindScript.CustomComponentName.Trim()))
                {
                    mBindScript.CustomComponentName = mBindScript.name;
                }
            }).Parent(mRootLayout);


            mComponentLine = EasyIMGUI.Horizontal();

            EasyIMGUI.Label().Text(LocaleText.Type)
                .Width(60)
                .FontSize(12)
                .Parent(mComponentLine);

            if (mBindScript.MarkType == BindType.DefaultUnityElement)
            {
                var components = mBindScript.GetComponents<Component>();

                var componentNames = components.Where(c => !(c is AbstractBind))
                    .Select(c => c.GetType().FullName)
                    .ToArray();

                var componentNameIndex = 0;

                componentNameIndex = componentNames.ToList()
                    .FindIndex((componentName) => componentName.Contains(mBindScript.ComponentName));

                if (componentNameIndex == -1 || componentNameIndex >= componentNames.Length)
                {
                    componentNameIndex = 0;
                }

                mBindScript.ComponentName = componentNames[componentNameIndex];

                PopupView.Create()
                    .WithIndexAndMenus(componentNameIndex, componentNames)
                    .OnIndexChanged(index => { mBindScript.ComponentName = componentNames[index]; })
                    .Parent(mComponentLine);
            }

            mComponentLine.Parent(mRootLayout);

            EasyIMGUI.Space()
                .Parent(mRootLayout);

            var belongsTo = EasyIMGUI.Horizontal()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.BelongsTo)
                .Width(60)
                .FontSize(12)
                .Parent(belongsTo);

            EasyIMGUI.Label().Text(CodeGenUtil.GetBindBelongs2(target as AbstractBind))
                .Width(200)
                .FontSize(12)
                .Parent(belongsTo);


            EasyIMGUI.Button()
                .Text(LocaleText.Select)
                .OnClick(() =>
                {
                    Selection.objects = new Object[]
                    {
                        CodeGenUtil.GetBindBelongs2GameObject(target as AbstractBind)
                    };
                })
                .Width(60)
                .Parent(belongsTo);

            mClassnameLine = new HorizontalLayout();

            EasyIMGUI.Label().Text(LocaleText.ClassName)
                .Width(60)
                .FontSize(12)
                .Parent(mClassnameLine);

            EasyIMGUI.TextField().Text(mBindScript.CustomComponentName)
                .Parent(mClassnameLine)
                .Content.Bind(newValue => { mBindScript.CustomComponentName = newValue; });

            mClassnameLine.Parent(mRootLayout);

            EasyIMGUI.Space()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.Comment)
                .FontSize(12)
                .Parent(mRootLayout);

            EasyIMGUI.Space()
                .Parent(mRootLayout);

            EasyIMGUI.TextArea()
                .Text(mBindScript.Comment)
                .Height(100)
                .Parent(mRootLayout)
                .Content.Bind(newValue => mBindScript.CustomComment = newValue);

            var bind = target as AbstractBind;
            var rootGameObj = CodeGenUtil.GetBindBelongs2GameObject(bind);


            if (rootGameObj.transform.GetComponent("ILKitBehaviour"))
            {
            }
            else if (rootGameObj.transform.IsUIPanel())
            {
                EasyIMGUI.Button()
                    .Text(LocaleText.Generate + " " + CodeGenUtil.GetBindBelongs2(bind))
                    .OnClick(() =>
                    {
                        var rootPrefabObj = PrefabUtility.GetPrefabObject(rootGameObj);
                        UICodeGenerator.DoCreateCode(new[] {rootPrefabObj});
                    })
                    .Height(30)
                    .Parent(mRootLayout);
            }
            else if (rootGameObj.transform.IsViewController())
            {
                EasyIMGUI.Button()
                    .Text(LocaleText.Generate + " " + CodeGenUtil.GetBindBelongs2(bind))
                    .OnClick(() => { CreateViewControllerCode.DoCreateCodeFromScene(bind.gameObject); })
                    .Height(30)
                    .Parent(mRootLayout);
            }


            OnRefresh();
        }

        private void OnRefresh()
        {
            if (mBindScript.MarkType == BindType.DefaultUnityElement)
            {
                mComponentLine.Visible = true;
                mClassnameLine.Visible = false;
            }
            else
            {
                mClassnameLine.Visible = true;
                mComponentLine.Visible = false;
            }
        }

        private void OnDisable()
        {
            mRootLayout.Clear();
            mRootLayout = null;
        }

        public override void OnInspectorGUI()
        {
            mRootLayout.DrawGUI();
            base.OnInspectorGUI();
        }
    }
}