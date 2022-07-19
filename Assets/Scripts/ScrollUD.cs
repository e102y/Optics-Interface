using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollUD : MonoBehaviour
{
    public Scrollbar scroll;
    public Camera cam;
    public CalculatePoints vals;

    // Start is called before the first frame update
    void Start()
    {
        //find camera
        //find calculate points
        //find scrollbar

        scroll.onValueChanged.AddListener(SetCamera);

        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetCamera(float y)
    {
        cam.transform.SetPositionAndRotation(new Vector3(cam.transform.position.x,
            (vals.maxY + y * (vals.minY-50))*1.05f, 
            cam.transform.position.z), cam.transform.rotation);
    }
}
