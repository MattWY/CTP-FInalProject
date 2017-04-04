using UnityEngine;
using System.Collections;

public class PlaceMonument : MonoBehaviour {

    //private Monument monument;
    private CheckBuilding check;
    private Transform currentMonument;
    public Camera cam;
    public bool monPlaced;
    public bool hasPressed = false;

    public Vector3 monVec;
	
	// Update is called once per frame
	void Update ()
    {
        if(currentMonument != null && monPlaced == false)
        {
            //find the mouse position 
            monVec = Input.mousePosition;
            monVec = new Vector3(monVec.x, monVec.y, transform.position.y);

            monVec.z = 30.0f;

            if (Input.GetKeyUp(KeyCode.Q))
            {
                hasPressed = true;
                if (hasPressed == true)
                {
                    monVec.z = monVec.z + 10;
                }
                hasPressed = false;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                monVec.z = monVec.z - 10;
            }


            //find the world point in the program and convert
            Vector3 findWorldPoint = cam.ScreenToWorldPoint(monVec);
            currentMonument.position = new Vector3(findWorldPoint.x, 0.0f, findWorldPoint.z);

            if (Input.GetMouseButtonDown(0))
            {
                if (isLegalPosition() == true)
                {
                    monPlaced = true;
                }
            }
            
        }	  
	}

    /// <summary>
    /// Checks colliders to ensure we arent trying to place a building on top of the other one
    /// </summary>
    /// <returns></returns>
    public bool isLegalPosition()
    {
        if(check.colliders.Count > 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Make a copy of the building to be placed
    /// </summary>
    /// <param name="_monGO"> The type of building we are using: so a church or a tower. </param>
    public void placeBuilding(GameObject _monGO)
    {
        //make a copy of the building
        monPlaced = false;
        currentMonument = Instantiate(_monGO).transform;

        //now perfrom checks to enusre we can place it here
        check = currentMonument.GetComponent<CheckBuilding>();
        
        Debug.Log("Button Pressed");
    }
}
