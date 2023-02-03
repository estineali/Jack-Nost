using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        // Get screen height
        // Get screen width
        // Set collision detectors at these locations? Or warp image to the start? Set screen bounds

        ScreenUtils.Initialize();
    }

    void Start()
    {
        Debug.Log("Project Lost Again");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
