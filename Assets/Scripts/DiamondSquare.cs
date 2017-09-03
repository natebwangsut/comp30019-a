using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare : MonoBehaviour
{

	private Mesh mesh;
	
	// Use this for initialization
	void Start ()
	{
		CreateTerrain();
	}

	void CreateTerrain()
	{	
		// Create new mesh
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		
		// Add a renderer into it
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		// Assign a custom shader
		renderer.material.shader = Shader.Find("Unlit/PhongTest");
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
