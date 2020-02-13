using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class BrushScript : MonoBehaviour {
    public int startXIndex, startYIndex, endXIndex, endYIndex;
    private SpriteRenderer spriteRend;
    private Sprite spr;
    private Vector3[] activePixels;
    private int startingX, startingY;
    public Slider bumpSlider;
    Texture2D normalMap;
    public List<Vector2> edges;
	// Use this for initialization
	void Start () {
        spriteRend = GetComponent<SpriteRenderer>();
        edges = new List<Vector2>();
        spr = spriteRend.sprite;
        startingX = startingY = 0;
        StoreActivePixels();
        SaveEdgeInformation();
        bumpSlider = GameObject.Find("Slider").GetComponent<Slider>();
        normalMap = NormalMap(spr.texture, bumpSlider.value);
    }

    public void ChangeBrush(Sprite mspr)
    {
        GetComponent<SpriteRenderer>().sprite = mspr;
        spr = mspr;
        StoreActivePixels();
        SaveEdgeInformation();
        normalMap = NormalMap(spr.texture, bumpSlider.value);
        SaveToTemp(normalMap);
    }

    public void OnSliderValueChange(Slider sld )
    {
        normalMap = NormalMap(spr.texture, sld.value);
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
        string path = Application.dataPath + "/Resources/temp/" + "TestBrush" + ".jpg";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log("Layer saved at: " + path);
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

    void SaveEdgeInformation()
    {
     
        edges.Clear();
        bool startEdge = true;
        float lastSavedPixel = 0;
        for (int y = 0; y < spr.rect.height; y++)
        {

            for (int x = 0; x < spr.rect.width; x++)
            {
                if (spr.texture.GetPixel(x, y).a > 0)
                {
                    if (startEdge)
                    {
                        Vector2 startEdgePixel = new Vector2(x, y);
                        edges.Add(startEdgePixel);
                        startEdge = false;
                    }
                    lastSavedPixel = x;
                }
            }
            Vector2 endEdgePixel = new Vector2(lastSavedPixel, y);
            edges.Add(endEdgePixel);
            startEdge = true;
        }
    }
	// Update is called once per frame
	void Update () {
		
	}

    public List<Vector2> GetEdges()
    {
        if(edges!=null)
        {
            return edges;
        }
        return null;
    }

    public void ReCal()
    {
        List<Vector2> newEdges = new List<Vector2>();
        foreach (Vector2 edge in edges)
        {
            int actualCoordinateX = ((int)edge.x - startingX) + (int)(spr.rect.width / 2);
            int actualCoordinateY = ((int)edge.y - startingY) + (int)(spr.rect.height / 2);
            Vector2 temp = new Vector2(actualCoordinateX, actualCoordinateY);
            newEdges.Add(temp);
        }
        edges = newEdges;

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

    public Color GetNormalColor(int x, int y)
    {
        int actualCoordinateX = (x - startingX) + (int)(spr.rect.width / 2);
        int actualCoordinateY = (y - startingY) + (int)(spr.rect.height / 2);

        return normalMap.GetPixel(actualCoordinateX, actualCoordinateY);
    }

    private Texture2D NormalMap(Texture2D source, float strength)
    {
        
        Texture2D result;
      
        result = new Texture2D(source.width, source.height, TextureFormat.RGBA32, true);
        float xTexelSize = 1 / result.width;
        float yTexelSize = 1/result.height;
        for (int by = 0; by < result.height; by++)
        {
            for (int bx = 0; bx < result.width; bx++)
            {
               
                Color leftPixel = source.GetPixel(bx - 1, by);
                Color rightPixel = source.GetPixel(bx + 1, by);
                Color upPixel = source.GetPixel(bx, by-1);
                Color DownPixel = source.GetPixel(bx , by+1);

                float xLeft = leftPixel.grayscale * strength;
                float xRight =  rightPixel.grayscale * strength;
                float yUp = upPixel.grayscale * strength;
                float yDown = DownPixel.grayscale * strength;
                float xDelta = ((xRight - xLeft) + 1) * 0.5f;
                float yDelta =((yUp - yDown) + 1) * 0.5f;
                result.SetPixel(bx, by, new Color(xDelta, yDelta, 1.0f, 1.0f));
              
            }
        }
        result.Apply();
        return result;
    }
}
