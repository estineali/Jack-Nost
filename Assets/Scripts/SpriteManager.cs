using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class SpriteManager : MonoBehaviour
{
    // Start is called before the first frame update


    int maxHeight = 250;
    int maxWidth = 250;

    public void Start()
    {
        float scaler = ScaleSprite(GetComponent<SpriteRenderer>().sprite);
     
        transform.localScale = new Vector3(scaler, scaler);
    }

    float ScaleSprite(Sprite image) 
    {
        float scaler;
        if (image.rect.height == 0 || image.rect.width == 0)
        {
            return 1;
        }

        if (image.rect.height >= image.rect.width)
        {
            scaler = maxHeight / image.rect.height;
        }
        else
        {
            scaler = maxWidth / image.rect.width;
        }
        return scaler; 
    }
}
