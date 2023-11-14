using UnityEngine;

namespace Assets.Scripts.Editor
{
    public interface IToggle : IMGUIView,IHasText<IToggle>
    {
        Property<bool> ValueProperty { get; }

        IToggle IsOn(bool isOn);
    }
    
    internal class Toggle : View,IToggle
    {
        private string mText { get; set; }

        public Toggle()
        {
            ValueProperty = new Property<bool>(false);

            Style = new GUIStyleProperty(() => GUI.skin.toggle);
        }

        public Property<bool> ValueProperty { get; private set; }
        public IToggle IsOn(bool isOn)
        {
            ValueProperty.Value = isOn;
            return this;
        }

        protected override void OnGUI()
        {
            ValueProperty.Value = GUILayout.Toggle(ValueProperty.Value, mText ?? string.Empty, Style.Value, LayoutStyles);
        }

        public IToggle Text(string text)
        {
            mText = text;
            return this;
        }
    }
}