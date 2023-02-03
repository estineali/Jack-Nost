using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class MemoryGenerator : MonoBehaviour
{

    int refreshFreqSecs = 5;

    bool fileNamesSet = false;
    string[] fileNames;
    public string nostBucketURL = "https://nostagain.appspot.com.storage.googleapis.com/";

    List<Memory> memories; 

    // Start is called before the first frame update
    void Start()
    {
        /// Run this every refreshFreqSecs.
        /// Update the array with any new ones in there. 
        StartCoroutine(GetNostagainBucket());


        memories = new List<Memory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fileNamesSet)
        {
            foreach (string item in fileNames)
            {
                Debug.Log(item);
            }
        }
    }


    /// <summary>
    /// This needs to run every X minutes 
    /// </summary>
    /// <returns>Defines the list of file names</returns>
    IEnumerator GetNostagainBucket()
    {

        UnityWebRequest bucketFiles = UnityWebRequest.Get(nostBucketURL);

        yield return bucketFiles.SendWebRequest();


        if (bucketFiles.result == UnityWebRequest.Result.Success)
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

            // Show results as text

            // Or retrieve results as binary data
            //byte[] results = bucketFiles.downloadHandler.data;
        }
    }

    /// <summary>
    /// This Method is called from the coroutine
    /// to set the list of file names on firebase storage.
    /// </summary>
    /// <param name="names"></param>
    public void ReturnStringNames(List<string> names)
    {
        if (fileNamesSet)
        {
            // instead of nulling them, Add the new ones. 
            fileNames = null;
            Debug.Log("File names reset");
        }
        fileNames = new string[names.Count];
        names.CopyTo(fileNames);
        fileNamesSet = true;
    }

    IEnumerator DownloadImage(string textureName)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(nostBucketURL + "/" + textureName);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

        }
    }

    void InstantiateMemory(Texture2D memory)
    {
        /// Create new gameobject
        /// Set its position
        /// Add SpriteMotion Script
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