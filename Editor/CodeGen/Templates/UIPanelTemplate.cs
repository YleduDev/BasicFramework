namespace Framework
{
    using System.IO;

    public class UIPanelTemplate
    {
        public static void Write(string name, string srcFilePath, string scriptNamespace,
            UIKitSettingData uiKitSettingData)
        {
            var scriptFile = srcFilePath;

            if (File.Exists(scriptFile))
            {
                return;
            }

            var writer = File.CreateText(scriptFile);

            var codeWriter = new FileCodeWriter(writer);


            var rootCode = new RootCode()
                .Using("UnityEngine")
                .Using("UnityEngine.UI")
                .Using("Framework")
                .EmptyLine()
                .Namespace(scriptNamespace, nsScope =>
                {
                    nsScope.Class(name + "Data", "UIPanelData", false, false, classScope => { });

                    nsScope.Class(name, "UIPanel", true, false, classScope =>
                    {
                        //classScope.CustomScope("protected override void ProcessMsg(int eventId, QMsg msg)", false,
                        //    (function) => { function.Custom("throw new System.NotImplementedException();"); });

                        //classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnInit(IUIData uiData = null)", false,
                            function =>
                            {
                                function.Custom("mData = uiData as {0} ?? new {0}();".FillFormat(name + "Data"));
                                function.Custom("// please add init code here");
                            });

                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnOpen(IUIData uiData = null)", false,
                            function => { });

                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnShow()", false,
                            function => { });
                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnHide()", false,
                            function => { });

                        classScope.EmptyLine();
                        classScope.CustomScope("protected override void OnClose()", false,
                            function => { });
                    });
                });

            rootCode.Gen(codeWriter);
            codeWriter.Dispose();
        }
    }
}