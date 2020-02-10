using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class LoadingScript : MonoBehaviour {


    public Transform goParent;
    public GameObject layerManager;
    public Transform LayerContentTransform;
    public GameObject layerTemplatePrefab;
    public Material defaultMat;
    public GameObject painterObject;
    public Transform gridorigin;
    int currentSessionCounter ;
    public bool clearOnExit;
    // Use this for initialization
    void Start () {
        currentSessionCounter = 0;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBrowse()
    {

        string[] filter = new string[2] { "fbx", "*" };
        string path = EditorUtility.OpenFilePanelWithFilters("Choose a file", Application.dataPath, filter);
        if (path.Equals("")) { return; }
        UnityEngine.Debug.Log("OPP" + path);
        string filename = Path.GetFileName(path);
        UnityEngine.Debug.Log(filename);

        string[] fileNameSplit = filename.Split('.');
        string fileWithoutExtension = fileNameSplit[0] + currentSessionCounter++;
        string newFileName = fileWithoutExtension + "." + fileNameSplit[1];

        File.Copy(path, Application.dataPath + "/Resources/temp/" + newFileName, true);

        string folderName = Path.GetDirectoryName(path);
        AssetDatabase.Refresh();

        GameObject meshes = Resources.Load<GameObject>("temp/" + fileWithoutExtension);
        CreateGameObject(meshes);
        Debug.Log("meshname: " + meshes.name);

        painterObject.GetComponent<LoadMap>().Load();
      
    }

    private void CreateGameObject(GameObject meshes)
    {
        GameObject newGO = Instantiate(meshes, goParent);

        newGO.transform.position = gridorigin.position;

      //  newGO.AddComponent<LoadMap>();
        newGO.AddComponent<CubeControl>();
        
        //Append to New
        //Material[] matOldArray = newGO.GetComponent<Renderer>().materials;
        //Material[] matNewArray = new Material[matOldArray.Length+1];
  
        //int count = 0;
        //foreach(Material m in matOldArray)
        //{
        //    matNewArray[count] = m;
        //    count++;
        //}
        //matNewArray[count]=defaultMat;
        //newGO.GetComponent<Renderer>().materials = matNewArray;

        //Replace with all
        //Material[] matNewArray = new Material[1];
        //matNewArray[0] = defaultMat;
        //newGO.GetComponent<Renderer>().materials = matNewArray;
        newGO.GetComponent<Renderer>().material.SetTexture("_BumpMap", painterObject.GetComponent<LoadMap>().GetMasterTexture());
        newGO.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
        Debug.Log(newGO.GetComponent<Renderer>().material.GetTexture("_BumpMap").name);
        layerManager.GetComponent<LayerControl>().SetCurrent3DModel(newGO);
    }

    void AddToLayers(string mname)
    {
        GameObject layer = Instantiate(layerTemplatePrefab, LayerContentTransform);
        layer.transform.GetChild(0).GetComponent<Text>().text = mname;
        layer.transform.position += Vector3.down *  layer.transform.GetSiblingIndex() * 40;
    }

    private void OnApplicationQuit()
    {
        if(clearOnExit)
        {
            string path = Application.dataPath + "/Resources/temp/";
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }
}
