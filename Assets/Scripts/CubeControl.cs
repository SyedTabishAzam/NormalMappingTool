using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour {
    GameObject masterObj;
    GameObject layerObj;
	// Use this for initialization
	void Start () {
        masterObj = GameObject.Find("PainterObject");
        layerObj = GameObject.Find("LayerPaintObject");


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchMaster(bool showAll)
    {
        if(masterObj && layerObj)
        {
            if(showAll)
            {
                Texture masterTexture = masterObj.GetComponent<Renderer>().material.GetTexture("_BumpMap");
                GetComponent<Renderer>().material.SetTexture("_BumpMap", masterTexture);
                GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
            }
            else
            {
                Texture layerTexture = layerObj.GetComponent<Renderer>().material.GetTexture("_BumpMap");
                GetComponent<Renderer>().material.SetTexture("_BumpMap", layerTexture);
                GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
            }
        }
    }
}
