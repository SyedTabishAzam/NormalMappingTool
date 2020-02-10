using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class LoadMap : MonoBehaviour {

    Texture masterTexure;
	// Use this for initialization
	void Start () {
        //   Texture normalDefault = Resources.Load<Texture>("Tiles_Triangles_001_normal");
        //plainNormal
        Load();
    }
	
	// Update is called once per frame

    public void Load()
    {
      

        Texture normalDefault = Resources.Load<Texture>("plainNormal");
        var texture = new Texture2D(normalDefault.width, normalDefault.height, TextureFormat.RGBA32, true);
        Graphics.CopyTexture(normalDefault, texture);
        texture.Apply();
        texture.name = normalDefault.name;

        masterTexure = normalDefault;
        GetComponent<Renderer>().material.SetTexture("_BumpMap", masterTexure);
    }

    public Texture GetMasterTexture()
    {
        return masterTexure;
    }

    public void OnApplicationQuit()
    {
        var relativePath = "Assets/Resources/plainNormal.jpg";
      
        AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate | (ImportAssetOptions)128);

    }

}
