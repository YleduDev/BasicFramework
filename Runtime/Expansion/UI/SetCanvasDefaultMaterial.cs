/*
版权声明：本文为CSDN博主「kuilaurence」的原创文章，遵循CC 4.0 BY - SA版权协议，转载请附上原文出处链接及本声明。
原文链接：https://blog.csdn.net/kuilaurence/article/details/89447398

以上shader可以解决UI在所有物体上显示不被遮挡的问题。但是会有个问题，就是需要每一个Image 、RawImage或者Text组件一一替换shader，未免有点麻烦。怎么一次性都自动替换呢？
其实每个组件的Material都继承Graphic.defaultGraphicMaterial。所以解决就有思路了：
1、在场景中的物体上挂上脚本：

*/
using UnityEngine;
/// <summary>
/// 设置canvas的默认材质球（Image上的也是一样），使ui渲染在模型的前面。
/// </summary>
public class SetCanvasDefaultMaterial : MonoBehaviour
{

    public Shader overay;
    private void Awake()
    { 
        UnityEngine.UI.Graphic.defaultGraphicMaterial.shader = overay;
        // UnityEngine.UI.Graphic.defaultGraphicMaterial.shader = Shader.Find("UI/Overlay");//可能shader没有引用，导致shader不会被打入包内，导致Find不到。
    }
}




