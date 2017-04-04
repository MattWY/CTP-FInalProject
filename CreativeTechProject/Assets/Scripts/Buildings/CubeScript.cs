using UnityEngine;
using System.Collections;

public class CubeScript : MonoBehaviour {

    public GameObject cube;
    public GameObject house;
    public GameObject tower;

    /// <summary>
    /// Checks for all collisons with regualr buildings 
    /// </summary>
    /// <param name="collision"> The collider of the object we are checking </param>
    private void OnTriggerEnter(Collider collision)
    {
        if (gameObject.name == "Cube(Clone)")
        {
            if (collision.gameObject.name == "Tower(Clone)")
            {
                Destroy(cube);
            }

            otherChecks(cube, collision);
        }

        if (gameObject.name == "Tower(Clone)")
        {
            if (collision.gameObject.name == "Tower(Clone)")
            {
                Destroy(tower);
            }

            if (collision.gameObject.name == "House(Clone)")
            {
                Destroy(tower);
            }

            otherChecks(tower, collision);
        }

        if (gameObject.name == "House(Clone)")
        {
            if (collision.gameObject.name == "Cube(Clone)")
            {
                Destroy(house);
            }
            otherChecks(house, collision);
        }

    }

    /// <summary>
    /// Checks thta apply to all three models
    /// </summary>
    /// <param name="building"> The building to be checked</param>
    /// <param name="collision"> Its collider</param>
    private void otherChecks(GameObject building, Collider collision)
    {
        if (collision.gameObject.name == "RiverCollider(Clone)")
        {
            Destroy(building);
        }

        if (collision.gameObject.name == "Bridge(Clone)")
        {
            Destroy(building);
        }

        if (collision.gameObject.name == "Collider(Clone)")
        {
            Destroy(building);
        }
    }
}
