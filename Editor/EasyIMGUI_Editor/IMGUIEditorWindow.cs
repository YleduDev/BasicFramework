#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Assets.Scripts
{
    public abstract class IMGUIEditorWindow : EditorWindow
    {
        public static T Create<T>(bool utility, string title = null) where T : IMGUIEditorWindow
        {
            return string.IsNullOrEmpty(title) ? GetWindow<T>(utility) : GetWindow<T>(utility, title);
        }

        private readonly List<IMGUIView> mChildren = new List<IMGUIView>();

        public bool Openning { get; set; }



        public void Open()
        {
            Openning = true;
            Show();
        }

        public new void Close()
        {
            Openning = false;
            base.Close();
        }
        
        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public void AddChild(IMGUIView childView)
        {
            mChildren.Add(childView);
        }

        public void RemoveChild(IMGUIView childView)
        {
            mChildren.Remove(childView);
        }

        public List<IMGUIView> Children
        {
            get { return mChildren; }
        }

        public void RemoveAllChidren()
        {
            mChildren.Clear();
        }

        public abstract void OnClose();


        public abstract void OnUpdate();

        private void OnDestroy()
        {
            OnClose();
        }

        protected abstract void Init();

        private bool mInited = false;

        public virtual void OnGUI()
        {
            if (!mInited)
            {
                Init();
                mInited = true;
            }

            OnUpdate();

            if (Visible)
            {
                mChildren.ForEach(childView => childView.DrawGUI());
            }
        }
    }
}
#endif