using UnityEngine;

namespace Assets.Scripts
{
    public interface ITextArea : IMGUIView, IHasText<ITextArea>
    {
        Property<string> Content { get; }
    }

    internal class TextArea : View, ITextArea
    {
        public TextArea()
        {
            Content = new Property<string>(string.Empty);
            mStyleProperty = new GUIStyleProperty(() => GUI.skin.textArea);
        }

        public Property<string> Content { get; private set; }

        protected override void OnGUI()
        {
            Content.Value = CrossPlatformGUILayout.TextArea(Content.Value, mStyleProperty.Value, LayoutStyles);
        }

        public ITextArea Text(string labelText)
        {
            Content.Value = labelText;
            return this;
        }
    }
}