using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationScript : MonoBehaviour {

    public enum Modes { MOVE,ROTATE,SCALE};
    public Modes currentMode;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMoveClick()
    {
        currentMode = Modes.MOVE;
    }

    public void OnRotateClick()
    {
        currentMode = Modes.ROTATE;
    }

    public void OnScaleClick()
    {
        currentMode = Modes.SCALE;
    }
}
