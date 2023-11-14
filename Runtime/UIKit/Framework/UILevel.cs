using System;

namespace Framework
{
    
    public enum UILevel
    {
        AlwayBottom = -2, //如果不想区分太复杂那最底层的UI请使用这个
        Bg = -1, //背景层UI
        Common = 0, //普通层UI
        PopUI = 1, //弹出层UI
        Toast = 2, //对话框层UI
        Forward = 3, //最高UI层用来放置UI特效和模型
        // 一个 Panel 就是一个 Canvas 的 Panel
        CanvasPanel = 100, // 
    }
}