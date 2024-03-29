﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Script class for simulating camera

public class CameraMovement : MonoBehaviour
{
	// Speed controls
	public float movementSpeed;
	public float rotationSpeed;

	public bool useController;

	private float _GroundHeight = -1;
	
	// Use this for initialization
	void Start ()
	{
		movementSpeed = 5;
		rotationSpeed = movementSpeed * 10;
		useController = true;
		
		// Initiate position
		transform.position = Vector3.up * 20;
		transform.rotation = Quaternion.Euler(Vector3.right * 90);
	}
	
	// Update is called once per frame
	void Update()
	{

		// Simulate slower motion when underwater
		if (transform.position.y < _GroundHeight)
		{
			movementSpeed = 1.5f;
		}
		else
		{
			movementSpeed = 5f;
		}
		
		// Camera movement on plane
		if (!useController)
		{
			float translation = Input.GetAxis("Vertical") * movementSpeed;
			float rotation = Input.GetAxis("Horizontal") * movementSpeed;

			translation *= Time.deltaTime;
			rotation *= Time.deltaTime;

			transform.Translate(rotation, 0, translation);
		}

		// Use character controller
		if (useController)
		{
			CharacterController controller = GetComponent<CharacterController>();
			Vector3 moveDirection = Vector3.zero;
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= movementSpeed;
			controller.Move(moveDirection * Time.deltaTime);
		}
		
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
