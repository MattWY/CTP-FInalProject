//----------------------------------
//
// MATT WYNTER CREATIVE TECH PROJECT
//
//
//----------------------------------

using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class Turtle : MonoBehaviour
{
    //turtle singleton
    public static Turtle turtle_instance = null;

    //initial points set to 0 as default
    public float xPos;
    public float zPos;
    public float angle;

    private float newX, newZ;

    //our intial road vectors
    public Vector3 newPos;
    public Vector3 firstPos;

    //gameobject representations
    [HideInInspector]
    public GameObject cubeGO;
    public GameObject colliderGO;
    public GameObject riverColliderGO;

    //floats for stroing current angles
    public float currentZ;
    public float currentX;
    public float currentAngle;
    public float currentAngle1;

    //string for storing the L-system axiom
    public string current = "A";

    //stack functionality
    public bool pushToStack = false;
    public bool popFromStack = false;

    //stop generating the process
    public bool stopGenerating = false;

    //string builder to appeand the result
    StringBuilder builder = new StringBuilder();

    //stack to store road values into 
    Stack stackRoad = new Stack();

    //material for the road
    public Material roadMaterial;

    //gameobjects for the buildings
    public GameObject cube;
    public GameObject tower;
    public GameObject house;

    //create a more generated landscape
    private int randomBuildingSize;
    private int randomBuilding;
    public int count = 0;

    //the intial road using line renders
    GameObject line;

    //keep the inspector tidy
    public Transform roadPieces;
    public Transform towerPieces;
    public Transform roadCollPiececs;
    public Transform housePiececs;
    public Transform buildingPieces;
    public Transform riverColls;

    ///<summary> This function is used to draw the current road segment to the program
    ///          as it utilises the line render from within unity to draw. 
    ///</summary>
    public void createLine(Vector3 firstPos, Vector3 newPos, bool isRiver)
    {
        //create a new line gameobject 
        line = new GameObject("Road");
        line.transform.position = firstPos;

        //create a new render component
        line.AddComponent<LineRenderer>();
        line.AddComponent<MeshCollider>();
        line.AddComponent<Rigidbody>();
        line.AddComponent<MeshRenderer>();

        //set the lines position to be that of the new and first positions: This then draws a road piece
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.SetPosition(0, firstPos);
        lr.SetPosition(1, newPos);
        lr.material.color = new Color(50, 0, 0);

        if (isRiver == false)
        {
            lr.material = new Material(roadMaterial);
            lr.enabled = false;
        }

        if (isRiver == true)
        {
            //lr.material = new Material();
            lr.enabled = false;
        }

        //enabled the mesh of the line render
        MeshRenderer mr = line.GetComponent<MeshRenderer>();
        mr.enabled = true;

        //setup the convex colliders of the component
        MeshCollider mc = line.GetComponent<MeshCollider>();
        mc.convex = true;
        mc.isTrigger = true;

        //and its rigid bodies
        Rigidbody rb = line.GetComponent<Rigidbody>();
        rb.mass = 1;
        rb.useGravity = false;

        line.transform.parent = roadPieces.transform;
    }

    /// <summary>
    /// A function that is called whenever we are moving forward, it checks the current angle agaianst the current left and right turns to ensure we 
    /// are headed in the correct direction. 
    /// </summary>
    /// <param name="value"> Parameter to tell us how much we need to move forward by</param>
    public void goForward(float value)
    {
        //set new positions as conversion to radians

        //float newX = xPos + value * Mathf.Cos(angle / 360 * 2 * Mathf.PI);
        //float newZ = zPos + value * Mathf.Sin(angle / 360 * 2  * Mathf.PI);

        //convert angle to radians and add the forward value to it
        //this sets up the new positions for the next road piece with the angles stored as radians
        newX = xPos + value * Mathf.Cos(angle  / 180 * Mathf.PI);
        newZ = zPos + value * Mathf.Sin(angle / 180 * Mathf.PI);

        Debug.Log(newX + newZ);

        //the certain vectors for the roads
        firstPos = new Vector3(xPos, 0, zPos);
        newPos = new Vector3(newX, 0, newZ);
        
        //draw the line
        createLine(firstPos, newPos, false);

        //check if we need branching patterns
        checkForBranching();

        GameObject.Find("RiverManager").GetComponent<River>().addPointsToList(zPos, xPos);

        //create buildings and road colliders for the road
        createBuilding();
        createColliderAngle();
    }

    /// <summary>
    /// If we are turning left then apply the degree turn to the current angle
    /// </summary>
    /// <param name="value"> How much we want to turn left by</param>
    public void goLeft(float value)
    {
        //adjust the angle by decrementing the set angle from the overall one
        //this simulates left turns
        angle -= value;
        currentAngle1 = value;
    }

    /// <summary>
    /// If we are turning right then apply the degree turn to the current angle
    /// </summary>
    /// <param name="value"> How much we want to turn right by</param>
    public void goRight(float value)
    {
        //adjust the angle by incrementing the set angle from the overall one
        //this simulates right turns
        angle += value;
        currentAngle1 = value;
    }

    /// <summary>
    /// Populates the stack with known positions, also lets us know if we have pushed to it
    /// </summary>
    /// <param name="roadSegment"> The current road piece we are pushing</param>
    public void pushRoadPosition(Vector3 roadSegment)
    {
        //push the road pos to the stack 
        stackRoad.Push(roadSegment);

        if (stackRoad != null)
        {
           Debug.Log("Pushing = " + roadSegment);
        }

        //ensure that we arent automatically popping or pushing again
        pushToStack = false;
        popFromStack = false;
    }

    /// <summary>
    /// Pops the most recent push off the stack to be used as the current road segment
    /// </summary>
    /// <param name="newFirstPos"> Creates a new firstpos based off the recent pop</param>
    public void popRoadPosition(Vector3 newFirstPos)
    {
        //only pop the road position if the stack is occupied
        if (stackRoad.Count > 0)
        {
            //set the new first pos to the current value in the pop
            newFirstPos = (Vector3)stackRoad.Pop();

            //and create a new road piece with this new pos
            createLine(newFirstPos, newPos, false);

            //and create a collider for it
            createColliderAngle();

        }

    }

    /// <summary>
    /// Decide whether we need to use the common branching style for a city. 
    /// For example, a grid style will not need it whereas other styles will do
    /// </summary>
    private void checkForBranching()
    {
        //push the selected road position onto the stack
        if (pushToStack == true)
        {
            pushRoadPosition(newPos);
        }

        //ensure that we are getting the new positions from the last pop 
        if (popFromStack == true)
        {
            popRoadPosition(firstPos);
            xPos = firstPos.x;
            zPos = firstPos.z;
        }

        if (ButtonManager.button_instance.currentRightAngle == 45 || ButtonManager.button_instance.currentLeftAngle == 45 ||
            ButtonManager.button_instance.currentRightAngle == 90 && ButtonManager.button_instance.currentLeftAngle == 90)
        {
            xPos = newX;
            zPos = newZ;
            popFromStack = false;
        }

        //if we havent popped recently
        if (popFromStack == false)
        {
            //reset the x position to the last endPos
            xPos = newX;
            //reset the y position to the last endPos
            zPos = newZ;
        }

        popFromStack = false;
    }

    /// <summary>
    /// Next three functions clear the various gameobjects from the screen to ensure we have a complete reset
    /// </summary>
    public void clearGameObjectList()
    {
        foreach (GameObject road in GameObject.FindObjectsOfType<GameObject>())
        {
            //remove the roads and the bridges
            if (road.name == "Road" || road.name == "Bridge(Clone)")
            {
                Destroy(road);
            }
            
            //remove the rivers and thier start points
            if(road.name == "RiverCollider(Clone)" || road.name == "RiverPoint 1(Clone)")
            {
                Destroy(road);
            }

            if(road.name == "DeadTree(Clone)" || road.name == "Tree(Clone)")
            {
                Destroy(road);
            }
        }

        clearBuilder();    
    }

    public void clearCubeList()
    {
        //remove all of the housing from the scene
        foreach (GameObject cube in GameObject.FindObjectsOfType<GameObject>())
        {
            if (cube.name == "Cube(Clone)")
            {
                Destroy(cube);
            }

            if (cube.name == "Tower(Clone)")
            {
                Destroy(cube);
            }

            if (cube.name == "House(Clone)")
            {
                Destroy(cube);
            }

            if(cube.name == "TowerMon(Clone)")
            {
                Destroy(cube);
            }

            if (cube.name == "ChurchMon(Clone)")
            {
                Destroy(cube);
            }
        }
    }

    public void clearColliderList()
    {
        foreach (GameObject collider in GameObject.FindObjectsOfType<GameObject>())
        {
            if (collider.name == "Collider(Clone)")
            {
                Destroy(collider);
            }
            if (collider.name == "VoidSpaces(Clone)")
            {
                Destroy(collider);
            }
        }
    }

    /// <summary>
    /// Reset the string builder aswell as some other parameters
    /// Called upon city reset.
    /// </summary>
    public void clearBuilder()
    {
        builder.Length = 0;
        builder.Capacity = 0;
        stackRoad.Clear();
        current = "A";
        count = 0;
        angle = 0;
    }

    /// <summary>
    /// Find out if the road is facing directly up or down
    /// This was a probelm because buildings would generate differently in this case
    /// </summary>
    /// <param name="buildingGO"> The building object</param>
    /// <param name="rotate"> A value to roatte by</param>
    /// <param name="i"> The current iterator value</param>
    private void findIfUpwardsFacing(GameObject buildingGO, float rotate, int i)
    {
        //if the road is pointing the other direction adjust the X params instead of the Z
        if (firstPos.x == newPos.x)
        {
            //create buildings on each side of the road, but creating parallel vectors
            cubeGO = (GameObject)Instantiate(buildingGO, new Vector3(returnCurrentX(), 1.0f, returnCurrentZ()),
                                             Quaternion.Euler(0, rotate, 0));
            cubeGO.transform.localPosition = Vector3.MoveTowards(firstPos + new Vector3(1.0f, 0.5f, -1.5f),
                                             newPos + new Vector3(1.0f, 0.7f, -1.5f), i);

            cubeGO.transform.parent = buildingPieces.transform;

            cubeGO = (GameObject)Instantiate(buildingGO, new Vector3(returnCurrentX(), 1.0f, returnCurrentZ()),
                                             Quaternion.Euler(0, rotate, 0));
            cubeGO.transform.localPosition = Vector3.MoveTowards(firstPos + new Vector3(1.5f, 0.5f, -1.5f),
                                             newPos + new Vector3(1.5f, 0.7f, -1.5f), i);

            cubeGO.transform.parent = buildingPieces.transform;

        }

    }

    /// <summary>
    /// Find out what style complex of buildings we want
    /// If a grid style has been generated, then we want an even representation 
    /// else we want something a little more dynamic
    /// </summary>
    public void createBuilding()
    {
        //set the rotation value = to our current road angle
        float rotate = returnCurrentAngle();

        if (currentAngle1 == 90)
        {
            rotate = 0.0f;
        }

        //how many buildings do we want in a line?
        GetRandomBuildingComplex();

        //if we do have a complete 90 degree road structure then apply same values to building arrangements - evenly dense. 
        if (ButtonManager.button_instance.currentRightAngle == 90 && ButtonManager.button_instance.currentLeftAngle == 90)
        {
            //cap the building iterator for an even result
            randomBuildingSize = 11;

            for (int i = 0; i < randomBuildingSize; i = i + 2)
            {
                //what size building do we want?
                GetRandomBuilding();

                placeBuildingsBasedOffPattern(rotate, i);
                 
            }
            rotate = 0;
        }

        //if we dont have a complete 90 degree structure then apply random values to building arrangements - less dense.
        if((ButtonManager.button_instance.currentRightAngle != 90 && ButtonManager.button_instance.currentLeftAngle != 90) || 
            (ButtonManager.button_instance.currentRightAngle == 90 && ButtonManager.button_instance.currentLeftAngle != 90) || 
            (ButtonManager.button_instance.currentRightAngle != 90 && ButtonManager.button_instance.currentLeftAngle == 90))
        {
            randomBuildingSize = 10;
            //use the varying building iterator for a varied result
            for (int i = 0; i < randomBuildingSize; i = i + 2)
            {
                GetRandomBuilding();

                placeBuildingsBasedOffPattern(rotate, i);

            }

            rotate = 0;
        }
    }

    /// <summary>
    /// Slighlty change the building size for variety
    /// </summary>
    /// <param name="buildingSize"> What size shall we alter the building to? </param>
    /// <param name="randomBuilding"> The building type that we are altering </param>
    private void changeBuildingSize(float buildingSize, int randomBuilding)
    {
        //is it a house
        if (randomBuilding == 1 || randomBuilding == 2)
        {
            //change the scale of it
            cubeGO.transform.localScale = new Vector3(buildingSize, 1.42f, 1.2f);

            if (buildingSize > 1.5)
            {
                //and push the building down to ensure it is on the floor
                cubeGO.transform.localPosition = cubeGO.transform.position + new Vector3(0, 0.1f, 0);
            }
        }

        //is it a tower
        if (randomBuilding == 3)
        {
            //chnage the scale of it
            cubeGO.transform.localScale = new Vector3(1.2f, buildingSize, 1.2f);

            if (buildingSize > 3)
            {
                //and push the building down to ensure it is on the floor
                cubeGO.transform.localPosition = cubeGO.transform.position + new Vector3(0, 0.2f, 0);
            }
        }

    }

    /// <summary>
    /// Place the buildings down, called from create building
    /// </summary>
    /// <param name="rotate"> A value to roatet the building by</param>
    /// <param name="i"> The current iterator </param>
    private void placeBuildingsBasedOffPattern(float rotate, int i)
    {
        if (randomBuilding == 1 || randomBuilding == 2)
        {
            float buildingSize = Random.Range(1.2f, 1.5f);
            //is the road pointing the other direction? if it is then pass to forwards facing func
            findIfUpwardsFacing(cube, rotate, i);

            //create buildings on each side of the road, but creating parallel vectors
            cubeGO = (GameObject)Instantiate(cube, new Vector3(returnCurrentX(), 1.0f, returnCurrentZ()),
                                             Quaternion.Euler(0, rotate, 0));
            cubeGO.transform.localPosition = Vector3.MoveTowards(firstPos + new Vector3(-1.5f, 0.7f, 1.0f),
                                             newPos + new Vector3(-1.5f, 0.7f, 1.0f), i);

            changeBuildingSize(buildingSize, randomBuilding);
            cubeGO.transform.parent = buildingPieces.transform;

            //spawn it on the other side of the road with the opposite vector
            cubeGO = (GameObject)Instantiate(cube, new Vector3(returnCurrentX(), 1.0f, returnCurrentZ()),
                                             Quaternion.Euler(0, rotate, 0));
            cubeGO.transform.localPosition = Vector3.MoveTowards(firstPos + new Vector3(-1.5f, 0.7f, -1.5f),
                                             newPos + new Vector3(-1.5f, 0.7f, -1.5f), i);

            changeBuildingSize(buildingSize, randomBuilding);
            cubeGO.transform.parent = buildingPieces.transform;

        }

        //change the type of building to be generated
        if (randomBuilding == 3)
        {
            float buildingSize = Random.Range(2.52f, 3.60f);
            findIfUpwardsFacing(tower, rotate, i);

            //create buildings on each side of the road, but creating parallel vectors
            cubeGO = (GameObject)Instantiate(tower, new Vector3(returnCurrentX(), 1.0f, returnCurrentZ()),
                                         Quaternion.Euler(0, rotate, 0));
            cubeGO.transform.localPosition = Vector3.MoveTowards(firstPos + new Vector3(-1.5f, 1.39f, 1.0f),
                                         newPos + new Vector3(-1.5f, 1.39f, 1.0f), i);

            changeBuildingSize(buildingSize, randomBuilding);
            cubeGO.transform.parent = towerPieces.transform;

            //spawn it on the other side of the road with the opposite vector
            cubeGO = (GameObject)Instantiate(tower, new Vector3(returnCurrentX(), 1.0f, returnCurrentZ()),
                                         Quaternion.Euler(0, rotate, 0));
            cubeGO.transform.localPosition = Vector3.MoveTowards(firstPos + new Vector3(-1.5f, 1.39f, -1.5f),
                                         newPos + new Vector3(-1.5f, 1.39f, -1.5f), i);

            changeBuildingSize(buildingSize, randomBuilding);
            cubeGO.transform.parent = towerPieces.transform;
        }

        if (randomBuilding == 4 || randomBuilding == 5)
        {
            findIfUpwardsFacing(house, rotate, i);

            //create buildings on each side of the road, but creating parallel vectors
            cubeGO = (GameObject)Instantiate(house, new Vector3(returnCurrentX(), 1.0f, returnCurrentZ()),
                                             Quaternion.Euler(0, rotate, 0));
            cubeGO.transform.position = Vector3.MoveTowards(firstPos + new Vector3(-1.5f, 0.4f, 1.0f),
                                             newPos + new Vector3(-1.5f, 0.4f, 1.0f), i);

            cubeGO.transform.parent = housePiececs.transform;

            //spawn it on the other side of the road with the opposite vector
            cubeGO = (GameObject)Instantiate(house, new Vector3(returnCurrentX(), 1.0f, returnCurrentZ()),
                                             Quaternion.Euler(0, rotate, 0));
            cubeGO.transform.position = Vector3.MoveTowards(firstPos + new Vector3(-1.5f, 0.4f, -1.5f),
                                             newPos + new Vector3(-1.5f, 0.4f, -1.5f), i);

            cubeGO.transform.parent = housePiececs.transform;
        }
    }

    /// <summary>
    /// This function creates colliders over the top of the line renders to ensure we have no buildings clipping into the road 
    /// Used for the roads
    /// </summary>
    public void createColliderAngle()
    {
        //find the middle of each line
        Vector3 colliderPos = firstPos + (newPos - firstPos) / 2;

        //find the angle between the two vectors
        float newAngle = (firstPos.z - newPos.z) / (firstPos.x - newPos.x);

        //if the line is not at all lined up, re-structure it until it is
        if ((firstPos.z < newPos.z && firstPos.x > newPos.x) || 
                (newPos.z < firstPos.z && newPos.x > firstPos.x) || 
                    (firstPos.z > newPos.z || firstPos.x < newPos.x))
        {
            //re-form the angle
            newAngle *= -1;
        }

        //convert the angle into radians so we can use it, as our road angles are measured in radians
        newAngle = Mathf.Rad2Deg * Mathf.Atan(newAngle); 

        //and finally create a new rotation with the new angle
        Quaternion colliderRot = Quaternion.Euler(0.0f, newAngle, 0.0f);

        //create the collider at the calculated positions
        GameObject roadColliderGO = Instantiate(colliderGO, colliderPos, colliderRot) as GameObject;

        roadColliderGO.transform.parent = roadCollPiececs.transform;
    }

    /// <summary>
    /// An overload function that ensures we are doing the same thing as the roads but for the rivers
    /// </summary>
    /// <param name="firstPos"> The firstpos of the river</param>
    /// <param name="newPos"> Where we need to draw to</param>
    /// <param name="dist"> How far do we need the collider to be</param>
    public void createColliderAngle(Vector3 firstPos, Vector3 newPos, float dist)
    {
        //find the middle of each line
        Vector3 colliderPos = firstPos + (newPos - firstPos) / 2;

        //find the angle between the two vectors
        float newAngle = (firstPos.z - newPos.z) / (firstPos.x - newPos.x);

        //if the line is not at all lined up, re-structure it until it is
        if ((firstPos.z < newPos.z && firstPos.x > newPos.x) ||
                (newPos.z < firstPos.z && newPos.x > firstPos.x) ||
                    (firstPos.z > newPos.z || firstPos.x < newPos.x))
        {
            //re-form the angle
            newAngle *= -1;
        }

        //convert the angle into radians so we can use it
        newAngle = Mathf.Rad2Deg * Mathf.Atan(newAngle);

        //and finally create a new rotation with the new angle
        Quaternion colliderRot = Quaternion.Euler(0.0f, newAngle, 0.0f);

        //create the collider at the calculated positions
        //colliderGO.GetComponent<Renderer>().sharedMaterial = riverMat;

        GameObject roadColliderGO = Instantiate(riverColliderGO, colliderPos + new Vector3(0,-0.09f,0), colliderRot) as GameObject;
        roadColliderGO.transform.localScale = new Vector3(dist, 0.2f, 1.0f);
        roadColliderGO.transform.parent = riverColls.transform;
    }

    /// <summary>
    /// The current Z value
    /// </summary>
    /// <returns></returns>
    public float returnCurrentZ()
    {
        //current z transform of the line
        return currentZ = line.transform.position.z;
    }

    /// <summary>
    /// The current X value
    /// </summary>
    /// <returns></returns>
    public float returnCurrentX()
    {
        //current x transform of the line
        return currentX = line.transform.position.x;
    }

    /// <summary>
    /// Find the current angle we are at
    /// </summary>
    /// <returns></returns>
    public float returnCurrentAngle()
    {
        //estimation of how we need to rotate the line
        currentAngle = currentAngle1 / 3.4f;
        return currentAngle;
    }

    /// <summary>
    /// Next two functions create random behaviour for buildings
    /// </summary>
    public void GetRandomBuilding()
    {
       randomBuilding = Random.Range(1, 6);
    }

    public void GetRandomBuildingComplex()
    {
        randomBuildingSize = Random.Range(6, 13);
    }

    /// <summary>
    /// Set up for the singleton
    /// </summary>
    void Awake()
    {
        if (turtle_instance == null)
        {
            turtle_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Function that reads the L-system feed as defined by the user and applies actions based on what rules have been passed in
    /// </summary>
    void Update()
    {
        //main L-System loop for cycling through our strings
        if (stopGenerating == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //get the current length of the string
                for (int i = 0; i < current.Length; i++)
                {
                    //get the current letter
                    char c = current[i];

                    //check what char we have currently
                    if (c == 'A')
                    {
                        //go forward with a fixed distance
                        goForward(10);
                        builder.Append(ButtonManager.button_instance.roadSeed);
                    }

                    if (c == 'B')
                    {
                        //go right with the current right angle
                        goRight(ButtonManager.button_instance.currentRightAngle);
                        builder.Append(ButtonManager.button_instance.roadSeed2);
                    }

                    if (c == 'C')
                    {
                        //go left with the current left angle
                        goLeft(ButtonManager.button_instance.currentLeftAngle);
                    }

                    if (c == 'D')
                    {
                        //push the current position to a stack
                        pushToStack = true;
                        builder.Append("ACE");
                    }

                    if (c == 'E')
                    {
                        //use the lastly pushed item as the next position for a road 
                        popFromStack = true;
                    }

                }

                //reset the current string
                current = builder.ToString();

                //generation number
                count++;

                Debug.Log("Generation " + count);

                if(count >= 5)
                {
                    stopGenerating = true;
                }

            }
        }
    } 

}
