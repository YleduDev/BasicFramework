using System;
using System.IO;

namespace Framework
{
    public class UIPanelDesignerTemplate
    {
        public static void Write(string name, string scriptsFolder, string scriptNamespace, PanelCodeInfo panelCodeInfo
          ,UIKitSettingData uIKitSettingData)
        {
            var scriptFile = scriptsFolder + "/{0}.Designer.cs".FillFormat(name);

            var writer = File.CreateText(scriptFile);

            var root = new RootCode()
                .Using("System")
                .Using("UnityEngine")
                .Using("UnityEngine.UI")
                .Using("Framework")
                .EmptyLine()
                .Namespace(scriptNamespace.IsTrimNullOrEmpty()
                    ? uIKitSettingData.Namespace
                    : scriptNamespace, ns =>
                {
                    ns.Custom("// Generate Id:{0}".FillFormat(Guid.NewGuid().ToString()));
                    ns.Class(name, null, true, false, (classScope) =>
                    {
                        classScope.Custom("public const string Name = \"" + name + "\";");
                        classScope.EmptyLine();

                        foreach (var bindInfo in panelCodeInfo.BindInfos)
                        {
                            if (bindInfo.BindScript.Comment.IsNotNullAndEmpty())
                            {
                                classScope.Custom("/// <summary>");
                                classScope.Custom("/// " + bindInfo.BindScript.Comment);
                                classScope.Custom("/// </summary>");
                            }

                            classScope.Custom("[SerializeField]");
                            classScope.Custom("public " + bindInfo.BindScript.ComponentName + " " + bindInfo.Name +
                                              ";");
                        }

                        classScope.EmptyLine();
                        classScope.Custom("private " + name + "Data mPrivateData = null;");

                        classScope.EmptyLine();

                        classScope.CustomScope("protected override void ClearUIComponents()", false, (function) =>
                        {
                            foreach (var bindInfo in panelCodeInfo.BindInfos)
                            {
                                function.Custom(bindInfo.Name + " = null;");
                            }

                            function.EmptyLine();
                            function.Custom("mData = null;");
                        });

                        classScope.EmptyLine();

                        classScope.CustomScope("public " + name + "Data Data", false,
                            (property) =>
                            {
                                property.CustomScope("get", false, (getter) => { getter.Custom("return mData;"); });
                            });

                        classScope.EmptyLine();


                        classScope.CustomScope(name + "Data mData", false, (property) =>
                        {
                            property.CustomScope("get", false,
                                (getter) =>
                                {
                                    getter.Custom("return mPrivateData ?? (mPrivateData = new " + name + "Data());");
                                });

                            property.CustomScope("set", false, (setter) =>
                            {
                                setter.Custom("mUIData = value;");
                                setter.Custom("mPrivateData = value;");
                            });
                        });
                    });
                });

            var codeWriter = new FileCodeWriter(writer);
            root.Gen(codeWriter);
            codeWriter.Dispose();
        }
    }
}