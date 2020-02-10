using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MakeInvisible()
    {
        GetComponent<MeshRenderer>().enabled = !GetComponent<MeshRenderer>().enabled;
    }
}
