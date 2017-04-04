using UnityEngine;
using System.Collections;

public class RemoveBuildings : MonoBehaviour {

    /// <summary>
    /// Used for the void densities
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Cube(Clone)")
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.name == "House(Clone)")
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.name == "Tower(Clone)")
        {
            Destroy(other.gameObject);
        }
    }
}

