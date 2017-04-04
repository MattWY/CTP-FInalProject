using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    float m_zoomFactor = 5.0f;
    float m_movementFactor = 1.0f;
    float m_zoomInFactor = 20.0f;
    float m_zoomOutFactor = 120.0f;
    
    /// <summary>
    /// Provide functionality for camera movement
    /// </summary>
    void Update()
    {
        //Debug.Log("X: " + Input.mousePosition.x + "Y: " + Input.mousePosition.y);
        if (/*Input.mousePosition.x > 850.0f*/ Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = new Vector3(transform.position.x + m_movementFactor, transform.position.y, transform.position.z);
        }

        if (/*Input.mousePosition.x <= 0.0f*/Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(transform.position.x - m_movementFactor, transform.position.y, transform.position.z);
        }

        if (/*Input.mousePosition.y > 350.0f*/ Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + m_movementFactor);
        }

        if (/*Input.mousePosition.y <= 0.0f*/ Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - m_movementFactor);
        }

        //to zoom the camera out
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + m_zoomFactor, transform.position.z);
            //check its y position
            if (transform.position.y > m_zoomOutFactor)
            {
                //then lock its position
                transform.position = new Vector3(transform.position.x, m_zoomOutFactor, transform.position.z);
            }

        }

        //to zoom the camera in
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - m_zoomFactor, transform.position.z);
            //check its y position
            if (transform.position.y < m_zoomInFactor)
            {
                //then lock its position
                transform.position = new Vector3(transform.position.x, m_zoomInFactor, transform.position.z);
            }
        }
    }
}
