using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpriteManager : MonoBehaviour
{
    // Start is called before the first frame update


    int maxHeight = 350;
    int maxWidth = 350;

    public void Start()
    {
        float scaler = ScaleSprite(GetComponent<SpriteRenderer>().sprite);
     
        transform.localScale = new Vector3(scaler, scaler);
    }

    // Update is called once per frame
    public void Update()
    {
        
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
