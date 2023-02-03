using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class SpriteResizer : MonoBehaviour
{
    // Start is called before the first frame update


    int maxHeight;
    int maxWidth;

    public void Start()
    {
        maxHeight = maxWidth = Random.Range(200, 320);

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
