using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class River : MonoBehaviour
{
    //the river singleton
    public static River river_instance = null;

    //public gameobjects
    public GameObject rivertest;
    public GameObject voidSpace;
    public GameObject tree;
    public GameObject deadtree;
    public GameObject monumentTower;
    public GameObject monumentChurch;

    [HideInInspector]
    public GameObject treeGO;
    [HideInInspector]
    public GameObject spaceGO;

    //list for finding the map components
    [HideInInspector]
    public List<float> ZposList;
    [HideInInspector]
    public List<float> XposList;

    private List<Vector3> riverPoints;

    public int treeType;

    //the current variables for storing the start of the river
    private float currentSmallestZ;
    private float currentSmallestX;

    //the current variables for storing the end of the river
    private float currentLargestZ;
    private float currentLargestX;
    private float randomValue;
    private float randomOffset;
    float scaleX, scaleZ;
    private float spaceX, spaceZ;

    //vector 3's for the river positions
    private Vector3 oppositeStartPoint, oppositeEndPoint;
    private Vector3 startPoint, endPoint, midPoint;
    private Vector3 upperQuartile, lowerQuartile, newLowerQuartile, newUpperQuartile;
    private Vector3 offset;

    private Vector3 testStart;
    private Vector3 newSize;

    public Transform trees;

    public bool hasRiverGenerated = false;

    /// <summary>
    /// Initialise various lists and random multipliers
    /// </summary>
    void Start()
    {
        //init lists and random values
        ZposList = new List<float>();
        XposList = new List<float>();
        riverPoints = new List<Vector3>();

        getRandomMultiplier();
        offset = new Vector3(0, -0.07f, 0);

    }

    /// <summary>
    /// Create the singleton
    /// </summary>
    void Awake()
    {
        //river singleton
        if (river_instance == null)
        {
            river_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Function that creates 2 lists with all of the city points and where the roads are located
    /// </summary>
    /// <param name="zPosition"> The point of every z position in the world</param>
    /// <param name="xPosition"> The point of every x position in the world</param>
    public void addPointsToList(float zPosition, float xPosition)
    {
        //add it to our z position list
        ZposList.Add(zPosition);
        XposList.Add(xPosition);
    }

    /// <summary>
    /// Find the smallest points in the city, which will be used for the start of the river
    /// </summary>
    public void findSmallestPoints()
    {
        //find the smallest points in both lists
        if (ZposList.Count != 0 && XposList.Count != 0)
        {
            currentSmallestZ = ZposList.Min();
            currentSmallestX = XposList.Min();
        }
    }

    /// <summary>
    /// Find the largest points in the city, which will be used for the end of the river, this then creates a vector 
    /// between the smallest and largest which can be manipulated.
    /// </summary>
    public void findLargestPoints()
    {
        //find the largest points in the lists
        if (ZposList.Count != 0 && XposList.Count != 0)
        {
            currentLargestZ = ZposList.Max();
            currentLargestX = XposList.Max();

            formulateRiver();
        }
    }

    /// <summary>
    /// When we reset, we need to reset the river by resetting the lists 
    /// </summary>
    public void resetParams()
    {
        //resets the lists if we hit the reset button
        ZposList.Clear();
        XposList.Clear();
    }

    /// <summary>
    /// How much to we need to offset the upper and lower quartile values?
    /// </summary>
    private void getRandomMultiplier()
    {
        randomValue = Random.Range(10, 50);
    }

    /// <summary>
    /// Function that formulates the river with all points passed to it
    /// </summary>
    private void formulateRiver()
    {
        if (hasRiverGenerated == false)
        {
            oppositeStartPoint = new Vector3(currentLargestX + 10, 0, currentSmallestZ - 5);
            Instantiate(rivertest, oppositeStartPoint + offset, transform.rotation);
            riverPoints.Add(oppositeStartPoint);

            oppositeEndPoint = new Vector3(currentSmallestX - 10, 0, currentLargestZ + 10);
            Instantiate(rivertest, oppositeEndPoint + offset, transform.rotation);
            riverPoints.Add(oppositeEndPoint);

            //start point of the river and its respective other points
            startPoint = new Vector3(currentSmallestX - 10, 0, currentSmallestZ - 10);
            Instantiate(rivertest, startPoint + offset, transform.rotation);
            riverPoints.Add(startPoint);

            endPoint = new Vector3(currentLargestX, 0, currentLargestZ + 10);
            Instantiate(rivertest, endPoint + offset, transform.rotation);
            riverPoints.Add(endPoint);

            midPoint = startPoint + (endPoint - startPoint) / 2;
            Instantiate(rivertest, midPoint + offset, transform.rotation);
            riverPoints.Add(midPoint);

            lowerQuartile = startPoint + (midPoint - startPoint) / 2;
            newLowerQuartile = lowerQuartile + new Vector3(0, 0, 30);
            riverPoints.Add(lowerQuartile);

            upperQuartile = endPoint + (midPoint - endPoint) / 2;
            newUpperQuartile = upperQuartile + new Vector3(0, 0, -10);
            riverPoints.Add(upperQuartile);

            //create void densities
            createVoidSpaces();

            //prepare the points for the turtle to draw
            createParamFuncs();

            hasRiverGenerated = true;
        }
    }

    /// <summary>
    /// Function that packages the river points for the turtle calss to draw, this saves turtle re-creation in this class.
    /// </summary>
    private void createParamFuncs()
    {
        //package the functions so they can be drawn by the turtle
        passToTurtle(startPoint, newLowerQuartile, true);
        passToTurtle(newLowerQuartile, midPoint, true);
        passToTurtle(midPoint, newUpperQuartile, true);
        passToTurtle(newUpperQuartile, endPoint, true);

        passToTurtle(oppositeStartPoint, newUpperQuartile, true);
        passToTurtle(oppositeEndPoint, newLowerQuartile, true);
    }

    /// <summary>
    /// Generate the type of tree that we want
    /// </summary>
    private void GenerateTreeType()
    {
        treeType = Random.Range(1, 3);
    }

    /// <summary>
    /// How much do we want to offset an object?
    /// </summary>
    private void GenerateRandomOffset()
    {
        randomOffset = Random.Range(-5,5);
    }

    /// <summary>
    /// Pass each river point to the turtle class and formulate the river and trees.
    /// </summary>
    /// <param name="riverFirstPos"> Each first pos of the river segment</param>
    /// <param name="riverNewPos"> Each end position of the river segment</param>
    /// <param name="riverParams"> Lets the turtle know if we are drawing a river</param>
    private void passToTurtle(Vector3 riverFirstPos, Vector3 riverNewPos, bool riverParams)
    {
        //pass the points to be drawn to the turtle 
        Turtle.turtle_instance.createLine(riverFirstPos, riverNewPos, riverParams);
        float dist = Vector3.Distance(riverFirstPos, riverNewPos);
        Turtle.turtle_instance.createColliderAngle(riverFirstPos, riverNewPos, dist);


        for (int i = 0; i < dist; i = i + 2)
        {
            GenerateTreeType();

            if (treeType == 1)
            {
                GenerateRandomOffset();

                if(i <= 2)
                {
                    spawnTrees(riverFirstPos, riverNewPos, i);
                }

                //spawn it on another side of the river with a different vector
                treeGO = (GameObject)Instantiate(tree, new Vector3(1.0f, -0.02f, 1.0f),
                                                     Quaternion.Euler(-90, 0, 0));
                treeGO.transform.localPosition = Vector3.MoveTowards(riverFirstPos + new Vector3(-randomOffset, -0.02f, randomOffset),
                                                     riverNewPos + new Vector3(-randomOffset, -0.02f, randomOffset), i);

                treeGO.transform.parent = trees.transform;
            }
            if (treeType == 2)
            {
                GenerateRandomOffset();
                //create trees on each side of the river, by creating parallel vectors
                treeGO = (GameObject)Instantiate(deadtree, new Vector3(1.0f, 1, 1.0f),
                                                 Quaternion.Euler(-90, 0, 0));
                treeGO.transform.position = Vector3.MoveTowards(riverFirstPos + new Vector3(randomOffset, 0.7f, -randomOffset),
                                                 riverNewPos + new Vector3(randomOffset, 0.7f, -randomOffset), i);

                treeGO.transform.parent = trees.transform;

            }
        }
    }

    /// <summary>
    /// Spawn a formualation of trees
    /// </summary>
    /// <param name="riverFirstPos"> Lets us know where the start point of each river segment is</param>
    /// <param name="riverNewPos"> Lets us know where the end point of each river segment is</param>
    /// <param name="i"> The current iterator</param>
    private void spawnTrees(Vector3 riverFirstPos, Vector3 riverNewPos, int i)
    {
        //create trees on each side of the river, by creating parallel vectors
        treeGO = (GameObject)Instantiate(tree, new Vector3(1.0f, -0.02f, 1.0f),
                                         Quaternion.Euler(-90, 0, 0));
        treeGO.transform.localPosition = Vector3.MoveTowards(riverFirstPos + new Vector3(randomOffset, -0.02f, randomOffset),
                                         riverNewPos + new Vector3(randomOffset, -0.02f, randomOffset), i);

        treeGO.transform.parent = trees.transform;
    }

    /// <summary>
    /// Void densities are created to control the densities, to ensure not everywhere has a large build up of buildings
    /// </summary>
    private void createVoidSpaces()
    {
        //find the largest X + Z positions
        scaleX = currentLargestX;
        scaleZ = currentLargestZ;

        //and scale the colliders appropriately depending on the size of the city, larger cities need larger spaces
        if (currentLargestX > currentLargestZ)
        {
            Debug.Log("X LARGEST");
            //small city = small spaces 
            if (scaleX > 0 && scaleX <= 150.0f || scaleX < 0 && scaleX >= -100.0f)
            {
                scaleX = 15.0f;
            }

            //large city = large spaces
            if (scaleX >= 150.0f || scaleX <= -100.0f)
            {
                scaleX = 50.0f;
            }
        }

        if (currentLargestZ > currentLargestX)
        {
            Debug.Log("Z LARGEST");
            if (scaleZ > 0 && scaleZ <= 150.0f || scaleZ < 0 && scaleZ >= -100.0f)
            {
                scaleX = 15.0f;
            }

            if (scaleZ >= 150.0f || scaleZ <= -200.0f)
            {
                scaleX = 50.0f;
            }
        }

        //what are the current params
        Debug.Log("Largest X" + currentLargestX + "Largest Z" + currentLargestZ + "Scale " + scaleX);
        Debug.Log("Smallest X" + currentSmallestX + "Smallest Z" + currentSmallestZ);

        setSpaces(scaleX);
       

    }

    /// <summary>
    /// Find random spaces in the current world
    /// </summary>
    private void getRandomSpace()
    {
        spaceX = Random.Range(currentSmallestX, currentLargestX);
        spaceZ = Random.Range(currentSmallestZ, currentLargestZ);
    }

    /// <summary>
    /// Scale the amount of spaces we want in the city depending on the size of the city
    /// </summary>
    /// <param name="scale"></param>
    private void setSpaces(float scale)
    {
        if(currentLargestX <= 30.0f && currentLargestZ <= 30.0f)
        {
            //if the city is small we want 3 voids
            for (int i = 0; i <= 2; i++)
            {
                getRandomSpace();
                createSpaces(spaceX, spaceZ, scale);
            }
        }

        if ((currentLargestX >= 31.0f && currentLargestX <=100))
        {
            //if the city is medium we want 7 voids
            for (int i = 0; i <= 6; i++)
            {
                getRandomSpace();
                createSpaces(spaceX, spaceZ, scale);
            }
        }

        if ((currentLargestX >= 101.0f && currentLargestX <= 500))
        {
            //if the city is large we want 9 voids
            for (int i = 0; i <= 8; i++)
            {
                getRandomSpace();
                createSpaces(spaceX, spaceZ, scale);
            }

        }
    }

    /// <summary>
    /// Create the spaces by adjusting the colliders
    /// </summary>
    /// <param name="xCo"> Position of the collider in the X axis</param>
    /// <param name="zCo"> Position of the collider in the Z axis</param>
    /// <param name="coScale"> Scale of the collider depending on the size of the city</param>
    private void createSpaces(float xCo, float zCo, float coScale)
    {
        //void spaces are created because it allows for city densities to be captured.
        testStart = new Vector3(xCo, 0, zCo);
        spaceGO = (GameObject)Instantiate(voidSpace, testStart + offset, transform.rotation);
        newSize = spaceGO.GetComponent<BoxCollider>().size = new Vector3(coScale, 10, coScale);
        setDynamicMons();
    }

    /// <summary>
    /// Spawn in dynamic monuments
    /// </summary>
    public void setDynamicMons()
    {
        int randValueInListPos = Random.Range(0, XposList.Count);
        int findRandomMon = Random.Range(0, 2);

        //spawn the monuments depending on which number has been generated
        if (findRandomMon == 0)
        {
            //spawn it by syncing the positions of the lists to ensure we spawn it near a road
            GameObject mon = (GameObject)Instantiate(monumentTower, new Vector3(XposList[randValueInListPos] - 5.0f, 0, ZposList[randValueInListPos] - 5.0f), transform.rotation);
        }

        if (findRandomMon == 1)
        {
            GameObject mon2 = (GameObject)Instantiate(monumentChurch, new Vector3(XposList[randValueInListPos] + 5.0f, 0, ZposList[randValueInListPos] + 5.0f), transform.rotation);
        }
    }
}
