using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interfaces : MonoBehaviour
{

    CalculatePoints primary;
    public List<double> planes;
    public List<double> planeHeights;

    GameObject[] drawers;
    GameObject Yaxis;
    UserGeneratedLine generatedLine;


    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }

    public void Init()
    {
        primary = GetComponent<CalculatePoints>();
        generatedLine = GetComponent<UserGeneratedLine>();
        //let's me reference these lists here
        planes = primary.board.planes;
        planeHeights = primary.board.planeHeights;

        drawers = new GameObject[planes.Count];
        float ypos = 0;
        for (int i = 1; i < planes.Count; ++i)
        {
            drawers[i] = new GameObject("interface" + i);
            LineRenderer lr = drawers[i].AddComponent(typeof(LineRenderer)) as LineRenderer;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.blue - new Color(0,0,0,0.5f);
            lr.endColor = Color.blue - new Color(0, 0, 0, 0.5f);
            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(primary.minX-50, ypos, -1));
            lr.SetPosition(1, new Vector3(primary.maxX+50, ypos, -1));
            ypos -= (float)planeHeights[i];
        }

        Yaxis = new GameObject("interface Y");
        LineRenderer lr2 = Yaxis.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lr2.material = new Material(Shader.Find("Sprites/Default"));
        lr2.startColor = Color.blue - new Color(0, 0, 0, 0.5f);
        lr2.endColor = Color.blue - new Color(0, 0, 0, 0.5f);
        lr2.positionCount = 2;
        lr2.SetPosition(0, new Vector3(0, primary.minY - 50, -1));
        lr2.SetPosition(1, new Vector3(0, primary.maxY + 50, -1));
    }
    //called by calculate points
    public void NextInLine()
    {
        if (generatedLine.update)
        {
            for (int i = 0; i < drawers.Length; ++i)
            {
                Destroy(drawers[i]);
            }
            drawers = new GameObject[planes.Count];
            float ypos = 0;
            for (int i = 1; i < planes.Count; ++i)
            {
                drawers[i] = new GameObject("interface" + i);
                LineRenderer lr = drawers[i].AddComponent(typeof(LineRenderer)) as LineRenderer;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = Color.blue - new Color(0, 0, 0, 0.5f);
                lr.endColor = Color.blue - new Color(0, 0, 0, 0.5f);
                lr.positionCount = 2;
                lr.SetPosition(0, new Vector3(primary.minX - 50, ypos, -1));
                lr.SetPosition(1, new Vector3(primary.maxX + 50, ypos, -1));
                ypos -= (float)planeHeights[i];
            }

            Destroy(Yaxis);
            Yaxis = new GameObject("interface Y");
            LineRenderer lr2 = Yaxis.AddComponent(typeof(LineRenderer)) as LineRenderer;
            lr2.material = new Material(Shader.Find("Sprites/Default"));
            lr2.startColor = Color.blue - new Color(0, 0, 0, 0.5f);
            lr2.endColor = Color.blue - new Color(0, 0, 0, 0.5f);
            lr2.positionCount = 2;
            lr2.SetPosition(0, new Vector3(0, primary.minY - 50, -1));
            lr2.SetPosition(1, new Vector3(0, primary.maxY + 50, -1));
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
