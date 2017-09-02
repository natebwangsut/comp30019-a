using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

	public GameObject terrain;

	private Vector3 offset;
	
	private float cameraSpeed = 2.5f;
	private bool smooth = true;
	
	// Use this for initialization
	void Start()
	{
		print("Initialised Camera Control.");
		transform.position = Vector3.up * 50;
		transform.rotation = Quaternion.Euler(Vector3.right * 90);
	}

	// Update is called once per frame
	void Update ()
	{
	}
}
