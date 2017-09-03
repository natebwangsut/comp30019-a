using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourTerrain : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		
		//renderer.material.color = Color.green;
		renderer.material.shader = Shader.Find("Custom/VertexShader");
		
		
		

		float _GroundHeight = 0;
		float _DirtHeight = 2;
		float _MaxHeight = 5;
		Color _sandBrown = new Color(242, 215, 160);
		Color _darkGrass = new Color(0, 102, 0);

		// create new colors array where the colors will be created.
		Color[] colors = new Color[vertices.Length];

		for (int i = 0; i < vertices.Length; i++)
		{

			// water level
			if (vertices[i].y < _GroundHeight)
			{
				colors[i] = Color.Lerp(Color.blue, _sandBrown, vertices[i].y);


			} 
			// sand to grass level
			else if (vertices[i].y >= _GroundHeight && vertices[i].y < _DirtHeight)
			{
				colors[i] = Color.Lerp(_sandBrown, Color.green, vertices[i].y);

			}
			// grass level
			else
			{
				colors[i] = Color.Lerp(Color.green, _darkGrass, vertices[i].y/_MaxHeight);

			}
			
			
			
			
			
		}


		// assign the array of colors to the Mesh.
		mesh.colors = colors;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
