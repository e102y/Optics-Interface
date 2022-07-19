using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;
using Optics_InterfaceML.Model1;
using Optics_InterfaceML.Model2;
using Optics_InterfaceML;
using System.Diagnostics;
using Unity.Jobs;
using Unity.Collections;

public class CalculatePointsPara : MonoBehaviour
{
    [HideInInspector]
    public float maxX;
    [HideInInspector]
    public float maxY;
    [HideInInspector]
    public float minX;
    [HideInInspector]
    public float minY;
    //I'm exposing more than I really should to make this work
    [HideInInspector]
    public state board;
    public bool NeuralNet;


    UserGeneratedLine generatedLine;
    GameObject[] drawers;
    Interfaces interfaceHandler;
    // Start is called before the first frame update
    void Start()
    {
        maxX = minX = maxY = minY = 0;
        generatedLine = GetComponent<UserGeneratedLine>();
        interfaceHandler = GetComponent<Interfaces>();

        Vector2 p0 = generatedLine.direction;
        Vector2 p1 = generatedLine.origin;


        board = new state();
        //board.planeHeights;
        //board.planes = new NativeArray<double>(2);
        //board.rays;
        /*
        board.init();

        board.planes.Add(1);
        board.planeHeights.Add(p1.y-p0.y);
        //board.planes.Add(1.002);
        //board.planeHeights.Add(10);
        board.planes.Add(1.333);
        board.planeHeights.Add(30);
        //board.planes.Add(2);
        //board.planeHeights.Add(10);



        board.planeHeights[0] = (p0.y - p1.y);
        physRay newRay = PointsToRay(p0, p1, 1, (p0.y > p1.y) ? 0 : 1, (p0.y > p1.y));

        board.rays.Enqueue(newRay);


        //Vector3[][] lines = RaysToPoints(Process.Traverse(board), board);
        
        */

        Stopwatch watch = new Stopwatch();
        watch.Start();

        Process para = new Process();
        para.NN = false;
        NativeArray<state> brd = new NativeArray<state>(1, Allocator.Temp);
        NativeArray<physRay> result;
        result = para.res;
        para.qs = brd;
        JobHandle h1 = para.Schedule(brd.Length, 1);


        watch.Stop();
        UnityEngine.Debug.Log("Total Time: " + watch.Elapsed);


        //List<physRay> physRays = Process.Traverse(board, NeuralNet);
        Vector3[][] lines = RaysToPoints(result, board);
        drawers = new GameObject[lines.Length];

        for (int i = 1; i < lines.Length; ++i)
        {
            drawers[i] = new GameObject("line" + i + ";  Level: " + result[i].level + ";  Direction: " + (result[i].direction ? "Downwards" : "Upwards") + ", angle: " + result[i].theta +
                    ", magnitude: " + result[i].magnitude);
            LineRenderer lr = drawers[i].AddComponent(typeof(LineRenderer)) as LineRenderer;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            Color lineC = new Color(0, 255, 0, Mathf.Clamp(((float)result[i].magnitude), 0, 1));
            lr.startColor = lineC;
            lr.endColor = lineC;
            lr.positionCount = 2;
            lr.SetPositions(lines[i]);
            lr.numCapVertices = 4;

            //these are just best-guesses based on the order in which I'm calculating things
            //it should be fine though
            maxX = (lines[i][1].x > maxX) ? lines[i][1].x : maxX;
            maxY = (lines[i][0].y > maxY) ? lines[i][0].y : maxY;
            minX = (lines[i][0].x < minX) ? lines[i][0].x : minX;
            minY = (lines[i][1].y < minY) ? lines[i][1].y : minY;

        }

        interfaceHandler.Init(); //must be run after board initialization
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (generatedLine.update)
        {
            Vector2 p0 = generatedLine.direction;
            Vector2 p1 = generatedLine.origin;
            board.planeHeights[0] = (p0.y - p1.y);
            physRay newRay = PointsToRay(p0, p1, 1, (p0.y > p1.y) ? 0 : 1, (p0.y > p1.y));
            board.rays = new Queue<physRay>();
            //newRay.y = 10;
            //newRay.x = 10 * Math.Tan(newRay.theta);
            board.rays.Enqueue(newRay);

            List<physRay> physRays = Process.Traverse(board, NeuralNet);
            Vector3[][] lines = RaysToPoints(physRays, board);

            for (int i = 0; i < drawers.Length; ++i)
            {
                Destroy(drawers[i]);
            }
            drawers = new GameObject[lines.Length];
            maxX = minX = maxY = minY = 0;
            for (int i = 1; i < lines.Length; ++i)
            {
                drawers[i] = new GameObject("line" + i + ";  Level: " + physRays[i].level + ";  Direction: " + (physRays[i].direction ? "Downwards" : "Upwards") + ", angle: " + physRays[i].theta +
                    ", magnitude: " + physRays[i].magnitude);
                LineRenderer lr = drawers[i].AddComponent(typeof(LineRenderer)) as LineRenderer;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                Color lineC = new Color(0, 255, 0, Mathf.Clamp(((float)physRays[i].magnitude), 0, 1));
                lr.startColor = lineC;
                lr.endColor = lineC;
                lr.positionCount = 2;
                lr.SetPositions(lines[i]);
                lr.numCapVertices = 4;

                //these are just best-guesses based on the order in which I'm calculating things
                //it should be fine though
                maxX = (lines[i][1].x > maxX) ? lines[i][1].x : maxX;
                maxY = (lines[i][0].y > maxY) ? lines[i][0].y : maxY;
                minX = (lines[i][0].x < minX) ? lines[i][0].x : minX;
                minY = (lines[i][1].y < minY) ? lines[i][1].y : minY;
            }

            interfaceHandler.NextInLine();
            generatedLine.update = false;
        }
        /*
         * 
         * if(change(occured)){
         *  do process
         *  update draw
         * }
         * 
         * 
         */

    }



    static physRay PointsToRay(Vector2 p0, Vector2 p1, double magnitude, int level, bool direction)
    {
        physRay ret = new physRay();
        ret.x = p0.x;//p1.x - p0.x;
        ret.y = p0.y;//-(p1.y - p0.y);
        ret.theta = -Math.Atan((p1.x - p0.x) / (p1.y - p0.y));

        ret.direction = direction;
        ret.magnitude = magnitude;
        ret.level = level;
        return ret;
    }
    static Vector3[] RayToPoint(physRay currentV, state board) //converts a ray to 2 points
    {
        Vector3[] currentP = new Vector3[2];

        float height = (float)board.planeHeights[currentV.level];
        //case for infinite length ray
        //double test = Math.Tan(currentV.theta);
        //if (test < -1000000 || test > 1000000)
        //{
        //    currentP[0] = new Vector3(0, 0, 0);
        //    currentP[1] = new Vector3(50, 0, 0); // the 50 values are arbitrary
        //}
        /*
         *  This ray travelles "downwards" its displacement is added to the previous y and added to the previous X
         */
        if (currentV.direction)
        {
            currentP[0] = new Vector3((float)currentV.x, (float)currentV.y, 0);
            currentP[1] = new Vector3((float)currentV.x + ((float)Math.Tan(currentV.theta) * height), ((float)currentV.y - height), 0);
        }
        /*
         *  This ray travelles "upward" its displacement is subtracted from the previous y and added to the previous X
         */
        else
        {
            currentP[0] = new Vector3((float)currentV.x, (float)currentV.y, 0);
            currentP[1] = new Vector3((float)currentV.x + ((float)Math.Tan(currentV.theta) * height), ((float)currentV.y + height), 0);
        }
        return currentP;
    }

    static Vector3[][] RaysToPoints(List<physRay> arr, state board) //converts a list of rays to a list of Vector3ds 
    {
        Vector3[][] rays = new Vector3[arr.Count][];
        physRay currentV;
        for (int i = 0; i < arr.Count; ++i)
        {
            currentV = arr[i];
            rays[i] = RayToPoint(currentV, board);
        }
        return rays;
    }
    static Vector3[][] RaysToPoints(NativeArray<physRay> arr, state board) //converts a list of rays to a list of Vector3ds 
    {
        Vector3[][] rays = new Vector3[arr.Length][];
        physRay currentV;
        for (int i = 0; i < arr.Length; ++i)
        {
            currentV = arr[i];
            rays[i] = RayToPoint(currentV, board);
        }
        return rays;
    }



    public struct physRay
    {
        public double x, y;             //refers to the origin
        public double theta, magnitude; //theta is angle, magnitude holds power information
        public bool direction;          //true is (180,360), false is (0,180)
        public int level;               //level refers to which interface the ray is dealing with
                                        //Level 0 is the first in the list, level n is the last
    }
    /*
    public struct state
    {
        public Queue<physRay> rays;
        public List<double> planes;
        public List<double> planeHeights;
        public void init()
        {
            rays = new Queue<physRay>();
            planes = new List<double>();
            planeHeights = new List<double>();
        }
    }
    */
    public struct state
    {
        public Queue<physRay> rays;
        public NativeArray<double> planes;
        public NativeArray<double> planeHeights;
        /*
        public void init()
        {
            rays = new Queue<physRay>();
            planes = new List<double>();
            planeHeights = new List<double>();
        }
        */
    }
    struct Process : IJobParallelFor
    {
        public bool NN;
        public NativeArray<state> qs;
        public NativeArray<physRay> res;
        public void Execute(int i)
        {
            List<physRay> temp;
            temp = Traverse(qs[i], NN);
            res = new NativeArray<physRay>(temp.ToArray(), Allocator.Persistent);

        }
        public static double Snell(double n1, double n2, double theta)
        {
            return Math.Asin((n1 / n2) * Math.Sin(theta));
        }
        public static float NNSnell(float n1, float n2, float theta)
        {
            Optics_InterfaceML.Model1.ModelInput sampleData = new Optics_InterfaceML.Model1.ModelInput()
            {
                T0 = theta,
                N0 = n1,
                N1 = n2,
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = Optics_InterfaceML.Model1.ConsumeModel.Predict(sampleData);
            return predictionResult.Score;
        }
        //This function is for processing the stack of materials for a (kind of) one-dimensional
        //case of geometric optics. it assumes that the first material is vacuum
        public static List<physRay> Traverse(state q1, bool NeuralNet)
        {


            double magThreshold = 0.0005; //the threshold for when rays are dropped
                                          //ray rcurrent = q1.rays.Dequeue();
            physRay rcurrent = new physRay();
            physRay rnext = new physRay();
            double nnext = 1; //holds the refractive index of the next medium, will be filled in with a proper value later in the code
            double ncurrent = 1;
            double reflectance = 0;
            double transmittance = 0;
            double nextTheta = 0;
            double nextMagnitude = 0;
            int nextLevel = 0;
            List<physRay> result = new List<physRay>(); //outputs a list of every ray produced after and including the initial ray(s)
                                                        // for(int i = 0; i< q1.planes.Count; i++)
                                                        //{


            while (q1.rays.Count > 0)
            {
                if (!NeuralNet)
                {
                    rcurrent = q1.rays.Dequeue();

                    //Console.WriteLine("The angle of the current ray is: {0:N2}° with a power of {1:N3} at the level {2:D} with a direction {3:B} at {4:N}, {5:N}", rcurrent.theta * (180 / Math.PI), rcurrent.magnitude, rcurrent.level, rcurrent.direction, rcurrent.x, rcurrent.y);
                    //Console.WriteLine("queue currently has {0:D} entries", q1.rays.Count);

                    if (rcurrent.level < q1.planes.Length && rcurrent.level >= 0)
                    {
                        ncurrent = q1.planes[rcurrent.level];
                        result.Add(rcurrent); //limits the output rays to a set between the first and last plane
                        System.Console.WriteLine("The angle of the current ray is: {0:N2}° with a power of {1:N3} at the level {2:D} with a direction {3:B} at {4:N}, {5:N}", rcurrent.theta * (180 / Math.PI), rcurrent.magnitude, rcurrent.level, rcurrent.direction, rcurrent.x, rcurrent.y);
                    }
                    else
                        continue;
                    //this block of code handles out of bounds indicies
                    if ((!rcurrent.direction && rcurrent.level - 1 >= 0) || (rcurrent.direction && rcurrent.level + 1 < q1.planes.Length))
                        nnext = rcurrent.direction ? q1.planes[rcurrent.level + 1] : q1.planes[rcurrent.level - 1];
                    else
                        nnext = 1;

                    nextTheta = Snell(ncurrent, nnext, rcurrent.theta);


                    //transmitted ray math (parallel)
                    /*
                    transmittance = (nextTheta == 0) ? 1 : ((nnext * Math.Cos(nextTheta)) / (ncurrent * Math.Cos(rcurrent.theta))) *
                        Math.Pow((2 * Math.Sin(nextTheta) * Math.Cos(rcurrent.theta)) /
                                ((Math.Sin(nextTheta + rcurrent.theta) * Math.Cos(nextTheta - rcurrent.theta))), 2);
                    */

                    //transmitted ray math (perpendicular)
                    transmittance = (Double.IsNaN(nextTheta)) ? 0 : (nextTheta == 0) ? 1 : ((nnext * Math.Cos(nextTheta)) / (ncurrent * Math.Cos(rcurrent.theta))) *
                        Math.Pow((2 * Math.Sin(nextTheta) * Math.Cos(rcurrent.theta)) /
                                ((Math.Sin(nextTheta + rcurrent.theta))), 2);

                    nextMagnitude = rcurrent.magnitude * transmittance;
                    nextLevel = rcurrent.direction ? rcurrent.level + 1 : rcurrent.level - 1;

                    if (nextMagnitude > magThreshold && ((rcurrent.direction && nextLevel < q1.planes.Length + 1) || (!rcurrent.direction && nextLevel >= 0 - 1)))
                    {
                        rnext = new physRay
                        {
                            x = rcurrent.x + q1.planeHeights[rcurrent.level] * Math.Tan(rcurrent.theta),
                            y = (rcurrent.direction) ? rcurrent.y - q1.planeHeights[rcurrent.level] : rcurrent.y + q1.planeHeights[rcurrent.level],
                            theta = nextTheta,
                            magnitude = nextMagnitude,
                            direction = rcurrent.direction,
                            level = nextLevel
                        };
                        q1.rays.Enqueue(rnext); //adds the transmitted ray to the queue      
                    }


                    //reflected ray math (parallel)
                    //reflectance = (nextTheta == 0) ? 0 : Math.Pow(Math.Tan(rcurrent.theta - nextTheta) / Math.Tan(rcurrent.theta + nextTheta), 2);

                    //relfected ray math (perpendicular)
                    //reflectance = (Double.IsNaN(nextTheta)) ? 1 : (nextTheta == 0) ? 0 : Math.Pow(Math.Sin(rcurrent.theta - nextTheta) / Math.Sin(rcurrent.theta + nextTheta), 2);
                    reflectance = 1 - transmittance;
                    nextMagnitude = rcurrent.magnitude * reflectance;
                    nextLevel = rcurrent.level;

                    if (nextMagnitude > magThreshold && ((rcurrent.direction && nextLevel < q1.planes.Length - 1) || (!rcurrent.direction && nextLevel > 0)))
                    {
                        rnext = new physRay
                        {
                            x = rcurrent.x + q1.planeHeights[rcurrent.level] * Math.Tan(rcurrent.theta),
                            y = (rcurrent.direction) ? rcurrent.y - q1.planeHeights[rcurrent.level] : rcurrent.y + q1.planeHeights[rcurrent.level],
                            theta = rcurrent.theta,
                            magnitude = nextMagnitude,
                            direction = !rcurrent.direction,
                            level = nextLevel
                        };
                        q1.rays.Enqueue(rnext); //adds the reflected ray to the queue
                    }
                }
                else
                {
                    rcurrent = q1.rays.Dequeue();

                    //Console.WriteLine("The angle of the current ray is: {0:N2}° with a power of {1:N3} at the level {2:D} with a direction {3:B} at {4:N}, {5:N}", rcurrent.theta * (180 / Math.PI), rcurrent.magnitude, rcurrent.level, rcurrent.direction, rcurrent.x, rcurrent.y);
                    //Console.WriteLine("queue currently has {0:D} entries", q1.rays.Count);

                    if (rcurrent.level < q1.planes.Length && rcurrent.level >= 0)
                    {
                        ncurrent = q1.planes[rcurrent.level];
                        result.Add(rcurrent); //limits the output rays to a set between the first and last plane
                        System.Console.WriteLine("The angle of the current ray is: {0:N2}° with a power of {1:N3} at the level {2:D} with a direction {3:B} at {4:N}, {5:N}", rcurrent.theta * (180 / Math.PI), rcurrent.magnitude, rcurrent.level, rcurrent.direction, rcurrent.x, rcurrent.y);
                    }
                    else
                        continue;
                    //this block of code handles out of bounds indicies
                    if ((!rcurrent.direction && rcurrent.level - 1 >= 0) || (rcurrent.direction && rcurrent.level + 1 < q1.planes.Length))
                        nnext = rcurrent.direction ? q1.planes[rcurrent.level + 1] : q1.planes[rcurrent.level - 1];
                    else
                        nnext = 1;

                    nextTheta = NNSnell((float)ncurrent, (float)nnext, (float)rcurrent.theta);

                    //transmitted ray math (perpendicular)
                    Optics_InterfaceML.Model2.ModelInput sampleData = new Optics_InterfaceML.Model2.ModelInput
                    {
                        T0 = (float)rcurrent.theta,
                        T1 = (float)nextTheta,
                        N0 = (float)ncurrent,
                        N1 = (float)nnext,
                    };

                    // Make a single prediction on the sample data and print results
                    var predictionResult = Optics_InterfaceML.Model2.ConsumeModel.Predict(sampleData);
                    transmittance = 1 - predictionResult.Score;

                    nextMagnitude = rcurrent.magnitude * transmittance;
                    nextLevel = rcurrent.direction ? rcurrent.level + 1 : rcurrent.level - 1;

                    if (nextMagnitude > magThreshold && ((rcurrent.direction && nextLevel < q1.planes.Length + 1) || (!rcurrent.direction && nextLevel >= 0 - 1)))
                    {
                        rnext = new physRay
                        {
                            x = rcurrent.x + q1.planeHeights[rcurrent.level] * Math.Tan(rcurrent.theta),
                            y = (rcurrent.direction) ? rcurrent.y - q1.planeHeights[rcurrent.level] : rcurrent.y + q1.planeHeights[rcurrent.level],
                            theta = nextTheta,
                            magnitude = nextMagnitude,
                            direction = rcurrent.direction,
                            level = nextLevel
                        };
                        q1.rays.Enqueue(rnext); //adds the transmitted ray to the queue 
                    }

                    //reflected ray math
                    reflectance = transmittance;
                    nextMagnitude = rcurrent.magnitude * reflectance;
                    nextLevel = rcurrent.level;

                    if (nextMagnitude > magThreshold && ((rcurrent.direction && nextLevel < q1.planes.Length - 1) || (!rcurrent.direction && nextLevel > 0)))
                    {
                        rnext = new physRay
                        {
                            x = rcurrent.x + q1.planeHeights[rcurrent.level] * Math.Tan(rcurrent.theta),
                            y = (rcurrent.direction) ? rcurrent.y - q1.planeHeights[rcurrent.level] : rcurrent.y + q1.planeHeights[rcurrent.level],
                            theta = rcurrent.theta,
                            magnitude = nextMagnitude,
                            direction = !rcurrent.direction,
                            level = nextLevel
                        };
                        q1.rays.Enqueue(rnext); //adds the reflected ray to the queue
                    }

                }
            }
            //should be the last ray in the material 

            return result;
        }
    }

}