using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monument : MonoBehaviour {

    public Material houseMat;
    public Material towerMat;
    public Vector3 dist;

    /// <summary>
    /// Check what gameobjects are in the vicinity of our monument and changes buildings accordingly
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        //get the bool that tells us when the monument has been placed only used for button press
        //if (GameObject.Find("ProgramUI").GetComponent<PlaceMonument>().monPlaced == true)

        if (gameObject.name == "TowerMon(Clone)")
        {
            if (other.name == "Cube(Clone)")
            {
                changeObjectToTower(other);
            }

            if (other.name == "House(Clone)")
            {
                changeObjectToTower(other);
            }

            otherChecks(other);
        }

        if(gameObject.name == "ChurchMon(Clone)")
        {
            if (other.name == "Tower(Clone)")
            {
                changeObjectToHouse(other);
            }

            if (other.name == "Cube(Clone)")
            {
                changeObjectToHouse(other);
            }

            otherChecks(other);
        }
     
    }

    /// <summary>
    /// Checks to be done that apply to both models
    /// </summary>
    /// <param name="other"> The collider of the mon </param>
    private void otherChecks(Collider other)
    {
        if (other.name == "RiverCollider(Clone)")
        {
            Destroy(this.gameObject);
        }

        if (other.name == "Collider(Clone)")
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Change the buildings in the vicinity to a tower
    /// </summary>
    /// <param name="other"></param>
    public void changeObjectToTower(Collider other)
    {
        //change the surrounding buildings to be that of a tower
        other.transform.localScale = new Vector3(1.2f, 2.82f, 1.2f);
        other.transform.TransformPoint(0.0f, 10.0f, 0.0f);

        dist = new Vector3(other.transform.position.x, 1.4f, other.transform.position.z);
        other.transform.position = Vector3.Lerp(other.transform.position, dist, Time.deltaTime);

        //change to the tower material
        other.GetComponent<Renderer>().sharedMaterial = towerMat;
    }

    /// <summary>
    /// If there are buildings in the vicinity, then change buildings to houses
    /// </summary>
    /// <param name="other"> The collider of the other gameobject </param>
    private void changeObjectToHouse(Collider other)
    {
        //change the surrounding buildings to be that of a tower
        other.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        other.transform.TransformPoint(0.0f, 10.0f, 0.0f);

        dist = new Vector3(other.transform.position.x, 0.5f, other.transform.position.z);
        other.transform.position = Vector3.Lerp(other.transform.position, dist, Time.deltaTime);

        //change to the tower material
        other.GetComponent<Renderer>().sharedMaterial = houseMat;
    }

}
