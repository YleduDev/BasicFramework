using Assets.Scripts;
using System;
#if UNITY_EDITOR
using UnityEditor;
namespace Assets.Scripts
{
    public class EnumPopupView : View
    {
        public Property<Enum> ValueProperty { get; set; }

        public EnumPopupView(Enum initValue)
        {
            ValueProperty = new Property<Enum>(initValue);
            ValueProperty.Value = initValue;
            Style = new GUIStyleProperty(() => EditorStyles.popup);
        }

        protected override void OnGUI()
        {
            Enum enumType = ValueProperty.Value;
            ValueProperty.Value = EditorGUILayout.EnumPopup(enumType, Style.Value, LayoutStyles);
        }
    }
}
#endif