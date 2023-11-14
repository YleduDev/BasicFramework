using System;

namespace Assets.Scripts
{
    public interface ICustom : IMGUIView
    {
        ICustom OnGUI(Action onGUI);
    }
    
    internal class CustomView : View,ICustom
    {
        private Action mOnGUIAction { get; set; }

        protected override void OnGUI()
        {
            mOnGUIAction.Invoke();
        }

        public ICustom OnGUI(Action onGUI)
        {
            mOnGUIAction = onGUI;
            return this;
        }
    }
}