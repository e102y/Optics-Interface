using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public Camera cam;
    [HideInInspector]
    public float zoom;
    public float scrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        zoom = 50;
        scrollSpeed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        float d = Input.mouseScrollDelta.y;
        if(d < 0)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + scrollSpeed, 10, 250);
        }
        else if(d > 0)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scrollSpeed, 10, 250);
        }
    }
}
