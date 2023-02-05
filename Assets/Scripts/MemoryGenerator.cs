using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class MemoryGenerator : MonoBehaviour
{

    int refreshFreqSecs = 5;
    public string nostBucketURL = "https://nostagain.appspot.com.storage.googleapis.com/";
    public PhysicsMaterial2D bouncyMaterial;
    Dictionary<string, Memory> memories;

    bool saveImgsToDisk = false; // True if you want to save the pictures to disk after they are downloaded 
    bool routineOver = true;

    // Start is called before the first frame update
    void Awake()
    {
        memories = new Dictionary<string, Memory>();

        StartCoroutine(RefreshImages());
    }

    private void Update()
    {
        if (routineOver)
        {
            StartCoroutine(RefreshImages());
        }
    }

    IEnumerator RefreshImages()
    {
        routineOver = false;
        Debug.Log("Refreshing Images");
        StartCoroutine(GetNostagainBucket());
        yield return new WaitForSeconds(refreshFreqSecs);
        routineOver = true;

    }


    /// <summary>
    /// This needs to run every X minutes 
    /// </summary>
    /// <returns>Defines the list of file names</returns>
    IEnumerator GetNostagainBucket()
    {
        UnityWebRequest bucketFiles = UnityWebRequest.Get(nostBucketURL);

        yield return bucketFiles.SendWebRequest();


        if (bucketFiles.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(bucketFiles.error);
        }
        else
        {

            XmlDocument bucketPage = new XmlDocument();
            bucketPage.LoadXml(bucketFiles.downloadHandler.text);
            XmlNodeList fileNames = bucketPage.GetElementsByTagName("Key");
            List<string> textureNames = new List<string>();


            if (fileNames.Count != 0)
            {
                foreach (XmlNode file in fileNames)
                {
                    textureNames.Add(file.InnerText);
                }
                ReturnStringNames(textureNames);
            }
        }
    }

    /// <summary>
    /// This Method is called from the coroutine
    /// to set the list of file names on firebase storage.
    /// </summary>
    /// <param name="names"></param>
    public void ReturnStringNames(List<string> fileNames)
    {

        // Check if object was deleted, then remove it
        foreach (var item in memories)
        {
            bool found = false;
            foreach (string newFile in fileNames)
            {
                if (newFile == item.Key)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                GameObject toDestroy = memories[item.Key].gameObjRef;
                memories.Remove(item.Key);
                Destroy(toDestroy);
                break;
            }
        }

        //Instantiate any new images
        foreach (string item in fileNames)
        {
            if (!memories.ContainsKey(item))
            {
                string[] nameAndExtension = item.Split(".");
                if (nameAndExtension != null)
                {
                    string ext = nameAndExtension[nameAndExtension.Length - 1].ToLower();

                    Debug.Log("Image Extension is: " + ext);
                    if (ext == "png" || ext == "jpg" || ext == "jpeg")
                    {
                        StartCoroutine(DownloadImage(item));
                    }
                }
            }
        }
    }

    IEnumerator DownloadImage(string textureName)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(nostBucketURL + textureName);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Memory mem = new Memory(myTexture, textureName);

            if (memories.Count == 0)
            {
                memories.Add(textureName, mem);
                InstantiateMemory(textureName);
                if (saveImgsToDisk)
                    SaveImage(textureName, myTexture);
            }
            else
            {
                if (!memories.ContainsKey(textureName))
                {
                    memories.Add(textureName, mem);
                    InstantiateMemory(textureName);

                    if (saveImgsToDisk)
                        SaveImage(textureName, myTexture);
                }
            }

            


        }
    }

    private void SaveImage(string textureName, Texture2D myTexture)
    {
        byte[] bytes = myTexture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        System.IO.File.WriteAllBytes(dirPath + textureName + ".png", bytes);
    }

    void InstantiateMemory(string memoryName)
    {

        if (memories[memoryName].loaded)
        {
            return;
        }

        GameObject gameObj = new GameObject();
        gameObj.SetActive(false);

        Rigidbody2D rb2d = gameObj.AddComponent<Rigidbody2D>();

        rb2d.angularDrag = 10;
        rb2d.drag = 2;

        BoxCollider2D collider =  gameObj.AddComponent<BoxCollider2D>();

        collider.sharedMaterial = bouncyMaterial;

        SpriteRenderer sr = gameObj.AddComponent<SpriteRenderer>();
        sr.color = Color.white;

        Texture2D t = memories[memoryName].texture;
        sr.sprite = Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0.5f, 0.5f) );

        memories[memoryName].Loaded();


        gameObj.AddComponent<SpriteResizer>();
        gameObj.AddComponent<SpriteMotion>();

        memories[memoryName].gameObjRef = gameObj;

        gameObj.SetActive(true);

        gameObj.transform.position = GetPosition();
    }

    Vector3 GetPosition()
    {
        return new Vector3(Random.Range(ScreenUtils.ScreenLeft, ScreenUtils.ScreenRight),
                           Random.Range(ScreenUtils.ScreenBottom, ScreenUtils.ScreenTop),
                           0);
    }

}


/// <summary>
/// This is only a class to hold the texturename and texture itself.
/// And also to maintain whether it has been loaded or not.
/// Loaded means it is being displayed on the wall. 
/// </summary>
public class Memory
{
    public Texture2D texture;
    public string textureName;
    public bool loaded;
    public GameObject gameObjRef;

    public Memory(Texture2D _texture, string _name)
    {
        texture = _texture;
        textureName = _name;
        loaded = false;
    }

    public void Loaded()
    {
        loaded = true;
    }

}