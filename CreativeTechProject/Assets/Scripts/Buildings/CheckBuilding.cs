using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckBuilding : MonoBehaviour {

    //simple script that checks for a collider 
    public List<Collider> colliders = new List<Collider>();

    /// <summary>
    /// Add all of the monumnets that are being collided to a list
    /// This ensures thatr we cant place monuments on top of each other
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Monument")
        {
            colliders.Add(other);
        }

        //if (other.gameObject.name == "Collider(Clone)")
        //{
        //    Destroy(this.gameObject);
        //}

    }

    /// <summary>
    /// Only removed if there is no collison with other monuments
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Monument")
        {
            colliders.Remove(other);
        }
    }
}
