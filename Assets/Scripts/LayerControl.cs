using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
public class LayerControl : MonoBehaviour {
    int layerNumber = 0;
    string newFileName = "";
    public GameObject layerPainterObj;
    public GameObject layerNormalObj;
    public GameObject layerPrefab;
    GameObject currentModel;
    public Transform LayerParent;
    Shader shader;
    List<Material> layers;
    Material newMat;
    Texture currentTexture;
    int cyclingLayer = 0;
    // Use this for initialization
    void Start ()
    {
        layers = new List<Material>();
        CreateNewLayer();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I))
        {
            CreateNewLayer();
        }

        if(Input.GetKeyDown(KeyCode.F3))
        {
            CycleThroughLayers();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            SaveAllLayers();
        }
    }

    public void SetCurrent3DModel(GameObject model)
    {
        currentModel = model;
    }
    public void SaveAllLayers()
    {
        foreach(Material m in layers)
        {
            Texture savedNormal = m.GetTexture("_BumpMap");
            string saveName = savedNormal.name;
            SaveToFile(savedNormal, saveName);
        }
    }

    void SaveToFile(Texture normal, string outputName)
    {

        Texture savedNormal = normal;
        Debug.Log("Saving map : " + savedNormal.name);
        //Width and height of returning texture
        int width = savedNormal.width;
        int height = savedNormal.height;

        //Create output texture to save shaders output
        Texture2D outputTex = new Texture2D(width, height, TextureFormat.ARGB32, false);


        //Create a new render texture to write an offscreen buffer
        RenderTexture buffer = new RenderTexture(
                                width,
                                height,
                                0,                            // No depth/stencil buffer
                                RenderTextureFormat.ARGB32,   // Standard colour format
                                RenderTextureReadWrite.Linear // No sRGB conversions
                            );

        //Bind texture to the creater buffer
        Graphics.Blit(savedNormal, buffer);
        RenderTexture.active = buffer;

        //Capture whole screen texture and start reading from top left pixel
        outputTex.ReadPixels(
                    new Rect(0, 0, width, height),
                    0, 0,
                    false
        );

        //Create bytes array to save encoded png
        byte[] bytes;
        bytes = outputTex.EncodeToJPG();

        //Write to file
        string path = Application.dataPath + "/../Layers/" + outputName + ".jpg";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log("File saved at: " + path);

    }

    public void CycleThroughLayers()
    {
        ChangeCurrentLayer(cyclingLayer);
        cyclingLayer = (cyclingLayer + 1) % layers.Count;
    }

    public void CreateNewLayer()
    {
        CopyBPToTemp();
        CreateNewMaterial();
        AssignMatToLayerPainter(newMat);
        SavePreviousMat();
        UpdateUI();
        ChangeNormalLayerMat();
    }

    public void ChangeNormalLayerMat()
    {
        layerNormalObj.GetComponent<Renderer>().material.SetTexture("_MainTex", currentTexture);
        layerNormalObj.GetComponent<Renderer>().material.EnableKeyword("_AlbedoMap");

    }

    public void ChangeCurrentLayer(int layerNumber)
    {
        layerPainterObj.GetComponent<Renderer>().material = layers[layerNumber];
        layerPainterObj.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
       
        layerNormalObj.GetComponent<Renderer>().material.SetTexture("_MainTex", layers[layerNumber].GetTexture("_BumpMap"));
        layerNormalObj.GetComponent<Renderer>().material.EnableKeyword("_AlbedoMap");
    }

    public void CopyBPToTemp()
    {
        //string bpPath = Application.dataPath + "/Resources/LayerBP.jpg";

        newFileName = "Layer" + layerNumber++;

        //File.Copy(bpPath, Application.dataPath + "/Resources/temp/" + newFileName + ".jpg", true);
        //AssetDatabase.Refresh();

        Texture2D normalDefault = Resources.Load<Texture2D>("LayerBP");
        var texture = new Texture2D(normalDefault.width, normalDefault.height, TextureFormat.RGBA32, true);
        Graphics.CopyTexture(normalDefault, texture);
        texture.Apply();
        texture.name = newFileName;
        currentTexture = texture;
        AssetDatabase.Refresh();
        //SaveToTemp(texture);

    }

    void SavePreviousMat()
    {
        layers.Add(layerPainterObj.GetComponent<Renderer>().material);
    }

    void CreateNewMaterial()
    {
        newMat = new Material(Shader.Find("Standard"));
        newMat.SetTexture("_BumpMap", currentTexture);
        
    }

    void AssignMatToLayerPainter(Material newMat)
    {
        layerPainterObj.GetComponent<Renderer>().material = newMat;
        layerPainterObj.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
    }

    void UpdateUI()
    {
        GameObject newLayerUI = Instantiate(layerPrefab, LayerParent);
        newLayerUI.transform.localPosition += (-newLayerUI.transform.up *45.5f)  * newLayerUI.transform.GetSiblingIndex();
        newLayerUI.transform.GetChild(0).GetComponent<Text>().text = newFileName;
    }

    void SaveToTemp(Texture texture)
    {
        Texture savedNormal = texture;
      
        //Width and height of returning texture
        int width = savedNormal.width;
        int height = savedNormal.height;

        //Create output texture to save shaders output
        Texture2D outputTex = new Texture2D(width, height, TextureFormat.RGBA32, false);


        //Create a new render texture to write an offscreen buffer
        RenderTexture buffer = new RenderTexture(
                                width,
                                height,
                                0,                            // No depth/stencil buffer
                                RenderTextureFormat.ARGB32,   // Standard colour format
                                RenderTextureReadWrite.Linear // No sRGB conversions
                            );

        //Bind texture to the creater buffer
        Graphics.Blit(savedNormal, buffer);
        RenderTexture.active = buffer;

        //Capture whole screen texture and start reading from top left pixel
        outputTex.ReadPixels(
                    new Rect(0, 0, width, height),
                    0, 0,
                    false
        );

        //Create bytes array to save encoded png
        byte[] bytes;
        bytes = outputTex.EncodeToJPG();

        //Write to file
        string path = Application.dataPath + "/Resources/temp/" + newFileName + ".jpg";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log("Layer saved at: " + path);
    }

    Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

}
