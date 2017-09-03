using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	// Speed controls
	public float movementSpeed;
	public float rotationSpeed;

	// Use this for initialization
	void Start ()
	{
		movementSpeed = 5;
		rotationSpeed = movementSpeed * 10;
	}
	
	// Update is called once per frame
	void Update() {
		
		// Camera movement on plane
		float translation = Input.GetAxis("Vertical") * movementSpeed;
		float rotation = Input.GetAxis("Horizontal") * movementSpeed;
		
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;
		
		transform.Translate(rotation, 0, translation);

		// Roll is custom created in input manager, be sure to see compatibility
		float r = rotationSpeed * Input.GetAxis("Roll");
		float h = rotationSpeed * Input.GetAxis("Mouse X");
		float v = rotationSpeed * Input.GetAxis("Mouse Y");

		r *= Time.deltaTime;
		h *= Time.deltaTime;
		v *= Time.deltaTime;
		
		transform.Rotate(v, h, r);
	}
	
	
	// Help ?
	void OnCollisionEnter(Collision theCollision)
	{
		if(theCollision.gameObject.name == "Terrain")
		{
			Debug.Log("Stop moving");
			movementSpeed = 0;
		} 
	}
}
