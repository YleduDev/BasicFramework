using UnityEngine;

namespace Assets.Scripts
{
    public interface IVerticalLayout : IMGUILayout
    {
        IVerticalLayout Box();
    }
    
    public class VerticalLayout : Layout,IVerticalLayout
    {
        public VerticalLayout(){}
        
        public string VerticalStyle { get; set; }

        public VerticalLayout(string verticalStyle = null)
        {
            VerticalStyle = verticalStyle;
        }

        protected override void OnGUIBegin()
        {
            if (string.IsNullOrEmpty(VerticalStyle))
            {
                GUILayout.BeginVertical(LayoutStyles);
            }
            else
            {
                GUILayout.BeginVertical(VerticalStyle, LayoutStyles);
            }
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndVertical();
        }

        public IVerticalLayout Box()
        {
            VerticalStyle = "box";
            return this;
        }
    }
    

}