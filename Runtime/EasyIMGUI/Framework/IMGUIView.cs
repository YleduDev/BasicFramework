using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Assets.Scripts
{
    public interface IMGUIView : IDisposable
    {
        bool Visible { get; set; }

        void DrawGUI();

        IMGUILayout Parent { get; set; }

        GUIStyleProperty Style { get; }

        Color BackgroundColor { get; set; }

        void RefreshNextFrame();

        void AddLayoutOption(GUILayoutOption option);

        void RemoveFromParent();

        void Refresh();
    }
}