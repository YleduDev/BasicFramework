using System.Collections;
using System.Collections.Generic;
using Framework.Example;
using UnityEngine;
using Framework;

public class UIKitExample_LoadFromResoruces : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{

		UIKit.OpenPanel<UISomePanelFromResources>(prefabName: "resources://UISomePanelFromResources");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
