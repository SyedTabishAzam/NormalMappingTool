using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushScript : MonoBehaviour {
    public int startXIndex, startYIndex, endXIndex, endYIndex;
    private SpriteRenderer spriteRend;
    private Sprite spr;
    private Vector3[] activePixels;
    private int startingX, startingY;
	// Use this for initialization
	void Start () {
        spriteRend = GetComponent<SpriteRenderer>();
        spr = spriteRend.sprite;
        startingX = startingY = 0;
        StoreActivePixels();
	}

    public void ChangeBrush(Sprite mspr)
    {
        GetComponent<SpriteRenderer>().sprite = mspr;
        spr = mspr;
        StoreActivePixels();
    }

    public void Reload(Sprite mspr)
    {

        
        //Component.Destroy(GetComponent<SpriteRenderer>());
        //this.gameObject.AddComponent<SpriteRenderer>().sprite = mspr;
        //spr = mspr;
        //StoreActivePixels();
    }

    void StoreActivePixels()
    {
        activePixels = new Vector3[(int)spr.rect.width * (int)spr.rect.height];
        int i = 0;
        for (int y = 0; y < spr.rect.height; y++)
        {
            for (int x = 0; x < spr.rect.width; x++)
            {
                Vector3 activePos = new Vector3(x, y, 0);
                if (spr.texture.GetPixel(x, y) == Color.red)
                {
                    activePos.z = 1;

                }
                activePixels[i] = activePos;
                i++;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ComputeReach(int mStartingX, int mStartingY)
    {
        startingX = mStartingX;
        startingY = mStartingY;
        startXIndex = mStartingX - (int)(spr.rect.width / 2);
        startYIndex = mStartingY - (int)(spr.rect.height / 2);
        endXIndex = mStartingX + (int)(spr.rect.width / 2);
        endYIndex = mStartingY + (int)(spr.rect.height / 2);
        
    }

    public Color GetColor(int x,int y)
    {
        int actualCoordinateX = (x - startingX) + (int)(spr.rect.width/2);
        int actualCoordinateY = (y - startingY) + (int)(spr.rect.height/2);
       
        return spr.texture.GetPixel(actualCoordinateX , actualCoordinateY);
    }
}
