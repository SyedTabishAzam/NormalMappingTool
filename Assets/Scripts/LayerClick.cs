using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LayerClick : MonoBehaviour {
    Button layerBtn;
	// Use this for initialization

	
	// Update is called once per frame
	
    void Start()
    {
        layerBtn = GetComponent<Button>();
        layerBtn.onClick.AddListener(() => InvokeButtonPressed());
    }



    public void InvokeButtonPressed()
    {
        string layerName = transform.GetChild(0).GetComponent<Text>().text;
        int layerNumber = int.Parse(layerName[layerName.Length-1].ToString());
        GameObject.Find("LayerManager").GetComponent<LayerControl>().ChangeCurrentLayer(layerNumber);
        GameObject.Find("GOParent").GetComponent<ChildControl>().UpdateChildrenView();
    }
}
