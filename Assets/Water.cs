using System.Collections;
using System.Collections.Generic;
using System.Linq;	//required inorder to convert list to array
using UnityEngine;

/* Source Code written by Kostas Sfikas, March 2017
Tested with Unity 5.5.0 f3 Pesonal edition*/

[RequireComponent(typeof(MeshFilter))]		// making sure that the gameobject has a MeshFilter component
[RequireComponent(typeof(MeshRenderer))]	// making sure that the gameobject has a MeshRenderer component
public class Water : MonoBehaviour {

	


	private Vector3[] mVerts;
	private int mVertCount;
	private Color[] clrs;
	private int mDivisions = 128;

	private float _baseWaterHeight = -1;
	private float _deepWaterHeight = -5.0f;
	private float _waterOffset = 0.001f;
	
	
	private float scale = 0.1f;
	private float speed = 1.0f;
	private float noiseStrength = 1.0f;
	
	private float noiseWalk = 1.0f;
 
	private Vector3[] baseHeight;

	private Mesh mesh;
	
	
	// Use this for initialization
	void Start ()
	{
		CreateTerrain();
	}

	
	// Use this to create the terrain
	void CreateTerrain()
	{

		
		
		float mSize = 10;
		
		mVertCount = (mDivisions + 1) * (mDivisions + 1);
		mVerts = new Vector3[mVertCount];
		Vector2[]uvs = new Vector2[mVertCount];
		int[] tris = new int[mDivisions * mDivisions * 6];
		clrs = new Color[mVertCount];

		float halfSize = mSize * 0.5f;
		float divisionSize = mSize / mDivisions;

		mesh = new Mesh();
		
		GetComponent<MeshFilter>().mesh = mesh;
		
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		
		//renderer.material.color = Color.green;
		renderer.material.shader = Shader.Find("Custom/WaterShader");

		
		
		// assisting variable to help set up triangles without needing to keep track of where we are at
		int triOffset = 0;

		
		for (int i = 0; i <= mDivisions; i++)
		{
			for (int j = 0; j <= mDivisions; j++)
			{
				
				// creating the vertices for each division
				mVerts[i * (mDivisions + 1) + j] = new Vector3(-halfSize+j*divisionSize, _baseWaterHeight, halfSize-i*divisionSize);

				clrs[i * (mDivisions + 1) + j] = assignColor(_baseWaterHeight);
				
				// the i*(mDivisions+1)+j] is meant to spread a 2D array into a 1D array
				uvs[i*(mDivisions+1)+j] = new Vector2((float)i/mDivisions, (float)j/mDivisions);

				if (i < mDivisions && j < mDivisions)
				{
					int topLeft = i * (mDivisions + 1) + j;
					int botLeft = (i + 1) * (mDivisions + 1) + j;

					tris[triOffset] = topLeft;
					tris[triOffset + 1] = topLeft + 1;
					tris[triOffset + 2] = botLeft + 1;

					tris[triOffset + 3] = topLeft;
					tris[triOffset + 4] = botLeft + 1;
					tris[triOffset + 5] = botLeft;

					triOffset += 6;
				}
			}
		}
		
		mVerts[0].y = Random.Range(_baseWaterHeight - _waterOffset, _baseWaterHeight); // top left
		clrs[0] = assignColor(mVerts[0].y);
		
		mVerts[mDivisions].y = Random.Range(_baseWaterHeight - _waterOffset, _baseWaterHeight); // top right
		clrs[mDivisions] = assignColor(mVerts[mDivisions].y);
		
		mVerts[mVerts.Length - 1].y = Random.Range(_baseWaterHeight - _waterOffset, _baseWaterHeight); // bottom right
		clrs[mVerts.Length - 1] = assignColor(mVerts[mVerts.Length - 1].y);
		
		mVerts[mVerts.Length - 1 - mDivisions].y = Random.Range(_baseWaterHeight - _waterOffset, _baseWaterHeight); // bottom left
		clrs[mVerts.Length - 1 - mDivisions] = assignColor(mVerts[mVerts.Length - 1 - mDivisions].y);

		
		// number of iterations to complete the diamond-square steps down to the centre
		int iterations = (int) Mathf.Log(mDivisions, 2);
		int numSquares = 1;
		
		// size of current square
		int squareSize = mDivisions;

		// current iteration that we are on
		for (int i = 0; i < iterations; i++)
		{
			int row = 0;
			for (int j = 0; j < numSquares; j++)
			{
				int col = 0;

				for (int k = 0; k < numSquares; k++)
				{
					DiamondSquare(row,col,squareSize, _baseWaterHeight);
					col += squareSize;
				}

				row += squareSize;
			}

			numSquares *= 2;
			squareSize /= 2;
			
			// adjust this variable to determine how fast/slow the height goes down
			// smaller number represents slower height change (a smoother terrain)
			// higher number represents faster height change (a more jagged terrain)
			_baseWaterHeight *= 0.45f;

		}
		
		
		
		// setting the mesh
		mesh.vertices = mVerts;
		mesh.uv = uvs;
		mesh.triangles = tris;
		mesh.colors = clrs;


		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
	
	// Executing diamond square algorithm
	// height will be reduced every iteration; offset variable to keep track of that
	
	void DiamondSquare(int row, int col, int size, float offset)
	{
		int halfSize = (int) (size * 0.5f);
		int topLeft = row * (mDivisions + 1) + col;
		int botLeft = (row + size) * (mDivisions + 1) + col;

		
		// DIAMOND STEP
		int mid =  (row + halfSize) * (mDivisions + 1) +  (col + halfSize);
		// multiplication is faster than division
		// offset is the random value added to average
		mVerts[mid].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[botLeft].y + mVerts[botLeft + size].y)*0.25f + Random.Range(-offset,offset);
		clrs[mid] = assignColor(mVerts[mid].y);
		
		// SQUARE STEP
		// Average out between the three surrounding vertices
		mVerts[topLeft + halfSize].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[mid].y) / 3 + Random.Range(-offset, offset);
		clrs[topLeft + halfSize] = assignColor(mVerts[topLeft + halfSize].y);
		
		mVerts[mid - halfSize].y = (mVerts[topLeft].y + mVerts[mid].y + mVerts[botLeft].y) / 3 + Random.Range(-offset, offset);
		clrs[mid - halfSize] = assignColor(mVerts[mid - halfSize].y);
		
		mVerts[mid + halfSize].y = (mVerts[topLeft + size].y + mVerts[mid].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
		clrs[mid + halfSize] = assignColor(mVerts[mid + halfSize].y);
		
		mVerts[botLeft + halfSize].y = (mVerts[botLeft].y + mVerts[mid].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
		clrs[botLeft + halfSize] = assignColor(mVerts[botLeft + halfSize].y);
	}
	
	Color assignColor(float height)
	{

		Color color;
		Color _baseWater = new Color32(102,178,255,255/2);
		Color _deepWater = new Color32(51, 153, 255, 255/2);

		color = Color.Lerp(_deepWater,_baseWater, Mathf.Abs(height/(_baseWaterHeight-_waterOffset)));
		
		
		
		
		return color;
	}
	
	
	
	
	void Update () {
		
		if (baseHeight == null)
			baseHeight = mesh.vertices;
  
		var vertices = new Vector3[baseHeight.Length];
		for (var i=0;i<vertices.Length;i++)
		{
			var vertex = baseHeight[i];
			vertex.y += Mathf.Sin(Time.time * speed+ baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
			vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)    ) * noiseStrength;
			vertices[i] = vertex;
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}
}