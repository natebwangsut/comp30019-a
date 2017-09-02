using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunlight : MonoBehaviour
{
	public float speed;

	// Use this for initialization
	void Start()
	{
		speed = 10;
		transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	// Update is called once per frame
	void Update () {
		// Given x-axis is the of West-East
		transform.Rotate(speed * Time.deltaTime,0 ,0);
	}
}
