
using UnityEngine;

namespace Framework.Example
{
	public class Example : MonoBehaviour
	{
		private void Awake()
		{
            UIKit.OpenPanel<UIMenuPanel>(prefabName: "Resources/UIMenuPanel");
		}
	}
}