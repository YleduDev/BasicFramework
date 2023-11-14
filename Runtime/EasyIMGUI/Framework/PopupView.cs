using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Assets.Scripts
{
    public interface IPopup : IMGUIView
    {
        IPopup WithIndexAndMenus(int index, params string[] menus);

        IPopup OnIndexChanged(Action<int> indexChanged);

        IPopup ToolbarStyle();

        Property<int> IndexProperty { get; }
        IPopup Menus(List<string> value);
    }
#if UNITY_EDITOR
    public class PopupView : View, IPopup
    {
        protected PopupView()
        {
            mStyleProperty = new GUIStyleProperty(() => EditorStyles.popup);
        }

        public static IPopup Create()
        {
            return new PopupView();
        }

        private Property<int> mIndexProperty = new Property<int>(0);

        public Property<int> IndexProperty
        {
            get { return mIndexProperty; }
        }

        public IPopup Menus(List<string> menus)
        {
            mMenus = menus.ToArray();
            return this;
        }

        private string[] mMenus = { };

        protected override void OnGUI()
        {
            IndexProperty.Value =
                EditorGUILayout.Popup(IndexProperty.Value, mMenus, mStyleProperty.Value, LayoutStyles);
        }

        public IPopup WithIndexAndMenus(int index, params string[] menus)
        {
            IndexProperty.Value = index;
            mMenus = menus;
            return this;
        }

        public IPopup OnIndexChanged(Action<int> indexChanged)
        {
            IndexProperty.Bind(indexChanged);
            return this;
        }

        public IPopup ToolbarStyle()
        {
            mStyleProperty = new GUIStyleProperty(() => EditorStyles.toolbarPopup);
            return this;
        }
    }
#endif
}