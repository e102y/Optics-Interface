using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollLR : MonoBehaviour
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

    void SetCamera(float x)
    {
        cam.transform.SetPositionAndRotation( new Vector3(
            (vals.minX + x*(vals.maxX+50))*1.05f,
            cam.transform.position.y, cam.transform.position.z), cam.transform.rotation);
    }
}
