using System;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IHorizontalLayout : IMGUILayout
    {
        IHorizontalLayout Box();
    }

    public class HorizontalLayout : Layout,IHorizontalLayout
    {
        public string HorizontalStyle { get; set; }
        

        protected override void OnGUIBegin()
        {
            if (string.IsNullOrEmpty(HorizontalStyle))
            {
                GUILayout.BeginHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal(HorizontalStyle);
            }
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndHorizontal();
        }

        public IHorizontalLayout Box()
        {
            HorizontalStyle = "box";
            return this;
        }
    }

}