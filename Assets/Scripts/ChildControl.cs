using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildControl : MonoBehaviour {

    // Use this for initialization
    bool showAll = true;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchMasterOrLayer()
    {
        showAll = !showAll;
        foreach(Transform child in transform)
        {
            child.gameObject.GetComponent<CubeControl>().SwitchMaster(showAll);
        }
    }

    public void UpdateChildrenView()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<CubeControl>().SwitchMaster(showAll);
        }
    }
}
