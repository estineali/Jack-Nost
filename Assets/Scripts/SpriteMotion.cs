using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMotion : MonoBehaviour
{

    Rigidbody2D rb2d;
    Vector2 motionDirection;
    const int motionForceMagnitude = 1;
    const float rotateDegreesPerSecond = 50;




    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        // initial motion Vector
        motionDirection = new Vector3(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the ship about the Z axis

        float rotationAmount = rotateDegreesPerSecond * Time.deltaTime;

        transform.Rotate(Vector3.forward, rotationAmount);

        //Update motion-direction vector
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        motionDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    void FixedUpdate()
    {
        rb2d.AddForce(motionDirection * motionForceMagnitude);
    }
}
