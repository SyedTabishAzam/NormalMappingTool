using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class PaintScript : MonoBehaviour {

    public enum EditorMode { DRAW,EDIT};
    public GameObject canvas;
    public EditorMode currentMode;
    public GameObject painterObject;
    public GameObject normalObject;
    public GameObject layerPainterObject;
    public GameObject layerNormalObject;
    public Material applyMat;
    public Material editMat;
    public RenderTexture rt;

    public bool inNormalMode;
    // Use this for initialization
    public GameObject BrushPrefab;
    bool matSaved = false;
    private GameObject brush;
	void Start () {
        LoadCanvas();
        brush = Instantiate(BrushPrefab);
        
    }

    void LoadCanvas()
    {
    
        Sprite spr = Resources.Load<Sprite>("whiteBackground");
        canvas.GetComponent<SpriteRenderer>().sprite = spr;
    }

    public void BrowseBrush()
    {
        string[] filter = new string[2] { ".png", "*" };
        string path = EditorUtility.OpenFilePanelWithFilters("Choose a file", Application.dataPath, filter);
        if (path.Equals("")) { return; }
        UnityEngine.Debug.Log("OPP" + path);
        string filename = Path.GetFileName(path);
        UnityEngine.Debug.Log(filename);

        string[] fileNameSplit = filename.Split('.');
        string fileWithoutExtension = fileNameSplit[0];
        string newFileName = fileWithoutExtension + "." + fileNameSplit[1];

        File.Copy(path, Application.dataPath + "/Resources/temp/" + newFileName, true);

        string folderName = Path.GetDirectoryName(path);
        AssetDatabase.Refresh();
        LoadBrushToPrefab(fileWithoutExtension);
    }

    private void LoadBrushToPrefab(string fileWithoutExtension)
    {
        Texture2D bufferTexture = Resources.Load("temp/" + fileWithoutExtension) as Texture2D;
        float ratio = bufferTexture.width / 100f;

       // bufferTexture.Resize((int)(bufferTexture.width / ratio), (int)(bufferTexture.height / ratio));
        Texture2D loadedBrush = duplicateTexture(bufferTexture);

      
        Sprite sprite = new Sprite();
        sprite = Sprite.Create(loadedBrush, new Rect(0, 0, loadedBrush.width, loadedBrush.height), new Vector2(0.5f,0.5f));
        ChangeBrush(sprite);
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

    public void ChangeBrush(Sprite spr)
    {
        if(brush!=null)
        {
            brush.GetComponent<BrushScript>().ChangeBrush(spr); 
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(currentMode==EditorMode.DRAW)
        {
            DetectObject();
        }

        
        if (Input.GetKeyDown(KeyCode.N))
        {
            if(!inNormalMode)
            {
                DisplayNormal(true);
                inNormalMode = true;
                currentMode = EditorMode.EDIT;
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            if(inNormalMode)
            {
                DisplayNormal(false);
                inNormalMode = false;
                currentMode = EditorMode.DRAW;
            }
        }

        if(Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.F4))
        {
            SaveToFile();
        }

    }

    void SaveToFile()
    {

        Texture savedNormal = painterObject.GetComponent<Renderer>().material.GetTexture("_BumpMap");
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
        string path = Application.dataPath + "/../Layers/Master.jpg";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log("File saved at: " + path);
        
    }

    void DisplayNormal(bool display)
    {
        painterObject.SetActive(!display);
        normalObject.SetActive(display);

        layerPainterObject.SetActive(!display);
        layerNormalObject.SetActive(display);

    }

    void ChangeMode(EditorMode newMode)
    {
        currentMode = newMode;
    }

    public void ChangeModeToDraw()
    {
        currentMode = EditorMode.DRAW;
    }

    public void ChangeModeToEdit()
    {
        currentMode = EditorMode.EDIT;
    }

    void DetectObject()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray.origin,ray.direction, 1000 * 500f);
        Vector3 greaterHitpoint = Vector3.zero;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if(hit.transform.tag=="Paint")
            {
                if(hit.point.z<greaterHitpoint.z)
                {
                    greaterHitpoint = hit.point;
                }
                MoveBrush(hit.point);
                if (Input.GetMouseButton(0))
                {
                    
                    DoOperationOnNormal(hit.transform.gameObject, new Vector2(0, 0));
                    // DoOperation(hit.transform.gameObject,new Vector2(0,0));
                }
            }
        }
      

    }
    
    void DoOperationOnNormal(GameObject go, Vector2 position)
    {


        Texture normalTexture = go.GetComponent<Renderer>().material.GetTexture("_BumpMap");

        Texture2D normal2D = (Texture2D)normalTexture;

        Renderer sr = go.GetComponent<Renderer>();


        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(go.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out localPoint);

        float pixelX = normal2D.width * -(float)(localPoint.x + 0.5) ; 
        float pixelY = normal2D.height * -(float)(localPoint.y + 0.5);
        

        ColorNormalPixelsAsBrush(normal2D,go, (int)pixelX, (int)pixelY);
     

    }

    void ColorNormalPixelsAsBrush(Texture2D normal2D,GameObject go, int originX, int originY)
    {
        BrushScript bs = brush.GetComponent<BrushScript>();
        bs.ComputeReach(originX, originY);
        bool isStartEdge = true;
        int lastSavedX = 0;
   
        for (int y = bs.startYIndex; y < bs.endYIndex; y++)
        {
            for (int x = bs.startXIndex; x < bs.endXIndex; x++)
            {
                
                Color temp = bs.GetColor(x, y);
                if (temp.a == 1)
                {
                    
                    //Blue will go from 0 to 255
                    if (isStartEdge)
                    {
                        temp = new Color(1f,1f, temp.b);

                        isStartEdge = false;
                    }
                    else
                    {
                        temp = new Color(0.5f, 0.5f, temp.b);
                    }
                    if(temp.b>=0.5f)
                    {
                        normal2D.SetPixel(x, y, temp);
                    }
                    lastSavedX = x;
                }
            }
            
            normal2D.SetPixel(lastSavedX, y, new Color(1f, 0f, 0f));
            isStartEdge = true;
        }


        normal2D.Apply();
       
        //Change Color of all overlapping pixels
        //
    }

    Color ConvertRGBToNormal(Color input)
    {
        return new Color((input.r * 2) - 1, (input.g *2) - 1, (input.b * 2) - 1);
    }

    Color RGBToGray(Color input)
    {
        return new Color(input.grayscale, input.grayscale, input.grayscale);
    }

    void MoveBrush(Vector3 hitpoint)
    {
     //   Vector3 camToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         brush.transform.position = new Vector3(hitpoint.x, hitpoint.y,6.70f);
    }

    void DoOperation(GameObject go,Vector2 position)
    {
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(go.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out localPoint);
        float pixelX = (localPoint.x + (((float)sprite.rect.width / (float)sprite.pixelsPerUnit) / 2f)) * sprite.pixelsPerUnit;
        float pixelY = (localPoint.y + (((float)sprite.rect.height / (float)sprite.pixelsPerUnit) / 2f)) * sprite.pixelsPerUnit;


        ColorPixelsAsBrush(sprite, (int)pixelX, (int)pixelY);
    }

    void ColorPixelsAsBrush(Sprite sprite,int originX,int originY)
    {
        BrushScript bs = brush.GetComponent<BrushScript>();
        bs.ComputeReach(originX, originY);
        for (int y = bs.startYIndex;y < bs.endYIndex;y++ )
        {
            for (int x = bs.startXIndex; x < bs.endXIndex; x++)
            {
                Color temp = bs.GetColor(x, y);
                if(temp.a==1)
                {
                    sprite.texture.SetPixel(x, y, temp);
                }
            }
        }
        
        sprite.texture.Apply();
        //Change Color of all overlapping pixels
        //
    }

    private void OnApplicationQuit()
    {
        AssetDatabase.Refresh();
    }


}
