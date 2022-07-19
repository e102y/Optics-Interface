using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGeneratedLine : MonoBehaviour
{
    [HideInInspector]
    public Vector3 origin;
    [HideInInspector]
    public Vector3 direction;
    float directionScale;
    public bool update;
    public bool selected;
    public LineRenderer r;
    public LineRenderer o;
    Camera c;
    public GameObject ui;
    //CalculatePoints calculatePoints;

    // Start is called before the first frame update
    void Start()
    {
        directionScale = 30;
        origin = new Vector3(0, 0, -2);
        direction = (new Vector2(-Mathf.Sin(Mathf.PI/4), Mathf.Sin(Mathf.PI / 4)))*directionScale;
        direction.z = -2;

        c = (Object.FindObjectsOfType(typeof(Camera)) as Camera[])[0]; //scruffy way to find the first camera
        //r = GetComponent<LineRenderer>();
        //calculatePoints = GetComponent<CalculatePoints>();

        r.material = new Material(Shader.Find("Sprites/Default"));
        r.startColor = Color.red;
        r.endColor = Color.red;
        //r.startWidth = 5;
        //r.endWidth = 5;
        r.positionCount = 2;
        r.SetPosition(0, origin);
        r.SetPosition(1, direction);
        r.numCapVertices = 4;

        Color outline = Color.yellow;
        o.material = new Material(Shader.Find("Sprites/Default"));
        o.startColor = outline;
        o.endColor = outline;
        o.widthMultiplier = 2.5f;
        o.positionCount = 2;
        o.SetPosition(0, origin);
        o.SetPosition(1, direction);
        o.numCapVertices = 4;
        o.enabled = false;


        selected = false;
        update = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 n0 = c.ScreenToWorldPoint(Input.mousePosition);
            if (
                (n0.x > (direction.x - 5)  && n0.x < (origin.x + 5))
                && (n0.y < (direction.y + 5) && n0.y > (origin.y - 5))
                )
            {
                selected = true;
                o.enabled = true;
                //ui.SetActive(true); //disabled until working
            }
            else
            {
                selected = false;
                o.enabled = false;
                ui.SetActive(false);
            }
        }

        if (selected)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                Vector3 n0 = c.ScreenToWorldPoint(Input.mousePosition);
                Vector2 n1 = n0;
                Vector2 n2 = n1 - (Vector2)origin;

                n2.Normalize();

                n2.x = Mathf.Clamp(n2.x, -1, 0);
                //n2.x = Mathf.Clamp(n2.x, -1, 1);
                //not quite 0 for infinite ray problems
                n2.y = Mathf.Clamp(n2.y, 0.000000001f, 1);


                direction = n2.normalized * directionScale;
                direction.z = -2;

                r.SetPosition(1, direction);
                o.SetPosition(1, direction);
                //update = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                update = true;
            }
            if (update)
            {
                r.SetPosition(1, direction);
                o.SetPosition(1, direction);
                //update = false;
            }
        }
        
    }
}
