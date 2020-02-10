using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SelectionScript : MonoBehaviour,IPointerClickHandler {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Sprite clickedSprite = transform.GetChild(0).GetComponent<Image>().sprite;
        GameObject.Find("Painter").GetComponent<PaintScript>().ChangeBrush(clickedSprite);
        DeactivateSelectionPanel();
        Debug.Log(transform.GetChild(1).name + " Game Object Clicked!");
    }

    public void DeactivateSelectionPanel()
    {
        transform.parent.parent.parent.gameObject.SetActive(false);
        GameObject.Find("Painter").GetComponent<PaintScript>().ChangeModeToDraw();
    }
}
