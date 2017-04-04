using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverColliding : MonoBehaviour {

    Vector3 upPos = new Vector3(0,0.5f,0);
    public GameObject bridgeGO;
    public Transform bridges;

    /// <summary>
    /// Check whther the river is colliding with a road
    /// </summary>
    /// <param name="collision"> The collider of the other gameobject </param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "RiverCollider(Clone)")
        {
            //find that current road segament params
            Vector3 getAngleFromRoad = this.transform.eulerAngles;
            Quaternion convertAngle = Quaternion.Euler(getAngleFromRoad);

            //adjust the center transform 
            //this.transform.position = this.transform.position + new Vector3(0, 0.5f, 0);
            Destroy(this.transform.gameObject);

            //and create the bridge for it
            Vector3 findCenterFromRoad = this.transform.position;
            GameObject bridge = Instantiate(bridgeGO, findCenterFromRoad + upPos, convertAngle) as GameObject;

        }
    }
}
