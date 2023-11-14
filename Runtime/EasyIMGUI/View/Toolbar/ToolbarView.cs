using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IToolbar : IMGUIView
    {
        IToolbar Menus(List<string> menuNames);
        IToolbar AddMenu(string name, Action<string> onMenuSelected = null);
        
        Property<int> IndexProperty { get; }

        IToolbar Index(int index);
    }

    internal class ToolbarView : View, IToolbar
    {
        public ToolbarView()
        {
            IndexProperty = new Property<int>(0);
            
            IndexProperty.Bind(index =>
            {
                if (MenuSelected.Count > index)
                {
                    MenuSelected[index].Invoke(MenuNames[index]);
                }
            });
            
            Style = new GUIStyleProperty(() => GUI.skin.button);
        }

        public IToolbar Menus(List<string> menuNames)
        {
            this.MenuNames = menuNames;
            // empty
            this.MenuSelected = MenuNames.Select(menuName => new Action<string>((str => { }))).ToList();
            return this;
        }

        public IToolbar AddMenu(string name, Action<string> onMenuSelected = null)
        {
            MenuNames.Add(name);
            if (onMenuSelected == null)
            {
                MenuSelected.Add((item) => { });
            }
            else
            {
                MenuSelected.Add(onMenuSelected);
            }

            return this;
        }

        List<string> MenuNames = new List<string>();

        List<Action<string>> MenuSelected = new List<Action<string>>();

        public Property<int> IndexProperty { get; private set; }
        public IToolbar Index(int index)
        {
            IndexProperty.Value = index;
            return this;
        }

        protected override void OnGUI()
        {
            IndexProperty.Value = GUILayout.Toolbar(IndexProperty.Value, MenuNames.ToArray(), Style.Value, LayoutStyles);
        }
    }
}