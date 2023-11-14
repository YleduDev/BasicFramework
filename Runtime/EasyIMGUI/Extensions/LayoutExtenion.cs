using System;

namespace Assets.Scripts
{
    public static class LayoutExtension
    {

        
        public static T Parent<T>(this T view, IMGUILayout parent) where T : IMGUIView
        {
            parent.AddChild(view);
            return view;
        }
    }
}