using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {
    //Checks if the camera collides with something
    void OnTriggerStay(Collider other)
    {
        transform.position += new Vector3(0, 0.2f, 0);
    }
}
