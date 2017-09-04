using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DiamondSquare : MonoBehaviour
{
	public Mesh mesh;

	public int seed;
	public int divisions;
	
	public float terrainSize;
	public float maxHeight;
	
	// Default to 0.45, adjust for rate of change in height of the algorithms
	// adjust this variable to determine how fast/slow the height goes down
	// smaller number represents slower height change (a smoother terrain)
	// higher number represents faster height change (a more jagged terrain)
	public float gradient = 0.45f;
	
	
	// Storing vertex data & color
	private int numVertices;
	private Vector3[] meshVertices;
	private Color[] color;
	
	// Use this for initialization
	void Start ()
	{
		// Initialised the seed first
		Random.InitState(seed);
		InitTerrain();
	}

	void InitTerrain()
	{	
		// Create new mesh
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		
		// Add a renderer into it
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		// Assign a custom shader
		renderer.material.shader = Shader.Find("Unlit/PhongTest");

		
		//
		numVertices = (divisions + 1) * (divisions + 1);
		meshVertices = new Vector3[numVertices];
		
		Vector2[] uvs = new Vector2[numVertices]; // storing 2d data
		int[] triangles = new int[divisions * divisions * 6];

		color = new Color[numVertices];

		float divisionSize = terrainSize / divisions;
		float halfTerrainSize = terrainSize * 0.5f; // (0.5f = 1/2)
		
		//
		int offset = 0;
		for (int i=0; i<=divisions; i++)
		{
			for (int j = 0; j <= divisions; j++)
			{
				int currentIteration = i * (divisions + 1) + j;
				
				// new vertex for each iteration
				meshVertices[currentIteration] = new Vector3(
					-halfTerrainSize + (j * divisionSize), 
					0f, 
					halfTerrainSize - (i * divisionSize));
				
				// Assigning into 2D array
				uvs[currentIteration] = new Vector2(
					(float) i / divisions,
					(float) j / divisions);
				
				// Making a square by two triangles
				if (i < divisions && j < divisions)
				{
					int topLeft = i * (divisions + 1) + j;
					int botLeft = (i + 1) * (divisions + 1) + j;

					// First triangle
					triangles[offset] = topLeft;
					triangles[offset + 1] = topLeft + 1;
					triangles[offset + 2] = botLeft + 1;

					// First second
					triangles[offset + 3] = topLeft;
					triangles[offset + 4] = botLeft + 1;
					triangles[offset + 5] = botLeft;

					offset += 6;
				}
			}
		}

		RandomiseHeight();
		
		// setting the mesh
		mesh.vertices 		= meshVertices;
		mesh.uv 			= uvs;
		mesh.triangles 		= triangles;
		mesh.colors 		= color;
		
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		
		// Add MeshCollider
		AddMeshCollider(mesh);
	}

	void RandomiseHeight()
	{
		// Assign a maximum height such that the global var does not change with the function
		float maximumHeight = maxHeight;
		
		// Initialise for D-S Algorithm
		// Top left
		meshVertices[0].y = Random.Range(-maximumHeight, maximumHeight);
		color[0] = assignColor(meshVertices[0].y);
		
		// Top right
		meshVertices[divisions].y = Random.Range(-maximumHeight, maximumHeight);
		color[divisions] = assignColor(meshVertices[divisions].y);
		
		// Bottom right
		meshVertices[meshVertices.Length - 1].y = Random.Range(-maximumHeight, maximumHeight);
		color[meshVertices.Length - 1] = assignColor(meshVertices[meshVertices.Length - 1].y);
		
		// Bottom left
		meshVertices[meshVertices.Length - 1 - divisions].y = Random.Range(-maximumHeight, maximumHeight);
		color[meshVertices.Length - 1 - divisions] = assignColor(meshVertices[meshVertices.Length - 1 - divisions].y);

		// Number of iterations
		int iterations = (int) Mathf.Log(divisions, 2);
		int numSquares = 1;	// counter
		
		// Size of the current square
		int squareSize = divisions;

		// Do D-S for [iterations]
		for (int i = 0; i < iterations; i++)
		{
			int row = 0;
			for (int j = 0; j < numSquares; j++)
			{
				int col = 0;
				for (int k = 0; k < numSquares; k++)
				{
					DiamondStep(row, col, squareSize, maximumHeight);
					SquareStep(row, col, squareSize, maximumHeight);
					col += squareSize;
				}
				row += squareSize;
			}
			numSquares *= 2;
			squareSize /= 2;
		
			maximumHeight *= gradient;
		}
	}
	
	// Executing diamond square algorithm
	// height will be reduced every iteration; offset variable to keep track of that
	void DiamondStep(int row, int col, int size, float offset)
     {
		int halfSize = (int) (size * 0.5f);
		int topLeft = row * (divisions + 1) + col;
		int botLeft = (row + size) * (divisions + 1) + col;
	 	int mid =  (row + halfSize) * (divisions + 1) +  (col + halfSize);
	     
		// DIAMOND STEP
		// multiplication is faster than division
		// offset is the random value added to average
		meshVertices[mid].y = (meshVertices[topLeft].y + meshVertices[topLeft + size].y + meshVertices[botLeft].y + meshVertices[botLeft + size].y)*0.25f + Random.Range(-offset,offset);
		color[mid] = assignColor(meshVertices[mid].y);
	}

	void SquareStep(int row, int col, int size, float offset)
	{
		int halfSize = (int) (size * 0.5f);
		int topLeft = row * (divisions + 1) + col;
		int botLeft = (row + size) * (divisions + 1) + col;
		int mid =  (row + halfSize) * (divisions + 1) +  (col + halfSize);

		// SQUARE STEP
		// Average out between the three surrounding vertices
		meshVertices[topLeft + halfSize].y = 
			(meshVertices[topLeft].y + meshVertices[topLeft + size].y + meshVertices[mid].y) / 3 + Random.Range(-offset, offset);
		color[topLeft + halfSize] = assignColor(meshVertices[topLeft + halfSize].y);
		
		meshVertices[mid - halfSize].y =
			(meshVertices[topLeft].y + meshVertices[mid].y + meshVertices[botLeft].y) / 3 + Random.Range(-offset, offset);
		color[mid - halfSize] = assignColor(meshVertices[mid - halfSize].y);
		
		meshVertices[mid + halfSize].y =
			(meshVertices[topLeft + size].y + meshVertices[mid].y + meshVertices[botLeft + size].y) / 3 + Random.Range(-offset, offset);
		color[mid + halfSize] = assignColor(meshVertices[mid + halfSize].y);
		
		meshVertices[botLeft + halfSize].y =
			(meshVertices[botLeft].y + meshVertices[mid].y + meshVertices[botLeft + size].y) / 3 + Random.Range(-offset, offset);
		color[botLeft + halfSize] = assignColor(meshVertices[botLeft + halfSize].y);
	}
	
	
	void AddMeshCollider(Mesh mesh)
	{
		// Add MeshCollider
		MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshc.sharedMesh = mesh;
	}
	
	Color assignColor(float height)
	{
		float _GroundHeight = -1.5f;
		float _SandHeight = (float)-0.5;
		float _RockyHeight = (float) (maxHeight - 1.5f);
		float _heightOffset = (float) 1.5;
		Color color;
		Color _sandBrown = new Color32(242, 215, 160, 255);
		Color _darkGrass = new Color32(0, 102, 0, 255);
		Color _rocky = new Color32(228, 225,223, 255);
		Color _baseWater = new Color32(102,178,255,255);
		Color _deepWater = new Color32(0, 0, 102, 255);

		
		// water level
		if (height < _GroundHeight)
		{
			color = Color.Lerp(_baseWater, _deepWater, (height/-5.0f)-0.15f);
		}
		// sand to grass level
		else if (height >= _GroundHeight && height < _SandHeight)
		{
			color = Color.Lerp(_sandBrown, Color.green, height);
		}
		// grass level	
		else if (height < _RockyHeight)
		{
			color = Color.Lerp(Color.green, _darkGrass, height);
		}
		// rocky/snow level
		else
		{
			color = Color.Lerp(_darkGrass,_rocky, (float)(height - 1));
		}
		
		return color;
	}
}
