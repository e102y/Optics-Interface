using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Arrows : MonoBehaviour
{
    public UserGeneratedLine generatedLine;
    public GameObject self;

    // Start is called before the first frame update
    void Start()
    {
        self.transform.SetPositionAndRotation(generatedLine.direction, self.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        self.transform.SetPositionAndRotation(generatedLine.direction, self.transform.rotation);
    }
}
