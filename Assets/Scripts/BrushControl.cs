using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class BrushControl : MonoBehaviour {

    public GameObject pickBrushPrefab;
    public Transform pickBrushContent;

    List<Sprite> brushList;
	// Use this for initialization
	void Start () {
        brushList = new List<Sprite>();
        PopulateList();
        PopulateOptions();
	}
		
	
	// Update is called once per frame
	void Update () {
		
	}

  

    void PopulateList()
    {
        List<string> fileNames = GetFileNamesWithoutExtension();
        foreach(string name in fileNames)
        {
            Sprite spr = Resources.Load<Sprite>("Brushes/" + name);
            brushList.Add(spr);

        }
    }

    void PopulateOptions()
    {
        foreach(Sprite spr in brushList)
        {
            GameObject go = Instantiate(pickBrushPrefab, pickBrushContent);
            go.transform.position += Vector3.down *0.7f * go.transform.GetSiblingIndex();
            go.transform.GetChild(0).GetComponent<Image>().sprite = spr;
            go.transform.GetChild(1).GetComponent<Text>().text = spr.name;
        }
    }

    List<string> GetFileNamesWithoutExtension()
    {
        List<string> fileNames = new List<string>();
        string path = Application.dataPath + "/Resources/Brushes";
        DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles("*.png"); //Getting Text files
        string str = "";
        foreach (FileInfo file in Files)
        {

            fileNames.Add(file.Name.Split('.')[0]);
        }
        return fileNames;
    }
}
