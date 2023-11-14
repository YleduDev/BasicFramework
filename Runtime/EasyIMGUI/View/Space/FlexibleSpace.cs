using UnityEngine;

namespace Assets.Scripts
{
    public interface IFlexibleSpace : IMGUIView
    {
    }

    internal class FlexibleSpace : View, IFlexibleSpace
    {
        protected override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }
}