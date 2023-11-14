using UnityEngine;

namespace Assets.Scripts
{
    public interface IScrollLayout : IMGUILayout
    {
        
    }
    
    internal class ScrollLayout : Layout,IScrollLayout
    {
        Vector2 mScrollPos = Vector2.zero;

        protected override void OnGUIBegin()
        {
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, LayoutStyles);
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndScrollView();
        }
    }
}