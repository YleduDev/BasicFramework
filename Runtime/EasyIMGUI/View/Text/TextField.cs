using UnityEngine;

namespace Assets.Scripts.Editor
{
    public interface ITextField : IMGUIView, IHasText<ITextField>
    {
        Property<string> Content { get; }

        ITextField PasswordMode();
    }

    internal class TextField : View,ITextField
    {
        public TextField()
        {
            Content = new Property<string>(string.Empty);

            mStyleProperty = new GUIStyleProperty(() => GUI.skin.textField);
        }

        public Property<string> Content { get; private set; }

        protected override void OnGUI()
        {
            if (mPasswordMode)
            {
                Content.Value = CrossPlatformGUILayout.PasswordField(Content.Value, Style.Value, LayoutStyles);
            }
            else
            {
                Content.Value = CrossPlatformGUILayout.TextField(Content.Value, Style.Value, LayoutStyles);
            }
        }
        

        private bool mPasswordMode = false;

        public ITextField PasswordMode()
        {
            mPasswordMode = true;
            return this;
        }

        public ITextField Text(string labelText)
        {
            Content.Value = labelText;
            return this;
        }
    }
}