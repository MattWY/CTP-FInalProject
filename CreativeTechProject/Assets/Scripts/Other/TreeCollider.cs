using UnityEngine;
using System.Collections;

public class TreeCollider : MonoBehaviour {

    public GameObject tree;
	
    /// <summary>
    /// Will remove trees if they come into contact with these objects
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Cube(Clone)" || other.gameObject.name == "Bridge(Clone)")
        {
            Destroy(tree);
        }

        if(other.gameObject. name == "RiverCollider(Clone)" || other.gameObject.name == "Collider(Clone)")
        {
            Destroy(tree);
        }
    }
}
