using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	// Speed controls
	public float movementSpeed;
	public float rotationSpeed;

	public float radius; 
	
	private float maxX;
	private float maxZ;
	
	private float minX;
	private float minZ;

	// Use this for initialization
	void Start ()
	{
		movementSpeed = 5;
		rotationSpeed = movementSpeed * 10;

		radius = 5;
		
		maxX = radius;
		maxZ = radius;

		minX = -maxX;
		minZ = -maxZ;
	}
	
	// Update is called once per frame
	void Update() {
		
		// Camera movement on plane
		float translation = Input.GetAxis("Vertical") * movementSpeed;
		float rotation = Input.GetAxis("Horizontal") * movementSpeed;
		
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;

		if ((maxX < rotation + transform.position.x) 
		    || (minX > rotation + transform.position.x))
		{
			rotation = 0;
			if (transform.position.x > maxX)
			{
				rotation = (maxX - transform.position.x);
			}
			if (transform.position.x < minX)
			{
				rotation = (minX - transform.position.x);
			}
		}
		
		if ((maxZ < translation + transform.position.z) 
		    || (minZ > translation + transform.position.z))
		{
			translation = 0;
			if (transform.position.z > maxZ)
			{
				translation = maxZ - transform.position.z;
			}
			if (transform.position.z < minZ)
			{
				translation = minZ - transform.position.z;
			}
		}
		
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
}
