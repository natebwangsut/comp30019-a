using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquareTerrain : MonoBehaviour
{

	// number of faces on each edge
	// mDivision +1 represents the number of vertices on each edge
	public int mDivisions;
	// size of terrain
	public float mSize;
	// maximum height of terrain
	public float mHeight;


	private Vector3[] mVerts;
	private int mVertCount;
	private Color[] clrs;
	
	
	
	// Use this for initialization
	void Start ()
	{
		CreateTerrain();
	}

	
	// Use this to create the terrain
	void CreateTerrain()
	{


		
		
		mVertCount = (mDivisions + 1) * (mDivisions + 1);
		mVerts = new Vector3[mVertCount];
		Vector2[]uvs = new Vector2[mVertCount];
		int[] tris = new int[mDivisions * mDivisions * 6];
		clrs = new Color[mVertCount];

		float halfSize = mSize * 0.5f;
		float divisionSize = mSize / mDivisions;

		Mesh mesh = new Mesh();
		
		GetComponent<MeshFilter>().mesh = mesh;
		
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		
		//renderer.material.color = Color.green;
		renderer.material.shader = Shader.Find("Unlit/PhongTest");

		
		
		// assisting variable to help set up triangles without needing to keep track of where we are at
		int triOffset = 0;

		
		for (int i = 0; i <= mDivisions; i++)
		{
			for (int j = 0; j <= mDivisions; j++)
			{
				
				// creating the vertices for each division
				mVerts[i * (mDivisions + 1) + j] = new Vector3(-halfSize+j*divisionSize, 0.0f, halfSize-i*divisionSize);
				
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
		
		
		mVerts[0].y = Random.Range(-mHeight, mHeight); // top left
		clrs[0] = assignColor(mVerts[0].y);
		
		mVerts[mDivisions].y = Random.Range(-mHeight, mHeight); // top right
		clrs[mDivisions] = assignColor(mVerts[mDivisions].y);
		
		mVerts[mVerts.Length - 1].y = Random.Range(-mHeight, mHeight); // bottom right
		clrs[mVerts.Length - 1] = assignColor(mVerts[mVerts.Length - 1].y);
		
		mVerts[mVerts.Length - 1 - mDivisions].y = Random.Range(-mHeight, mHeight); // bottom left
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
					DiamondSquare(row,col,squareSize, mHeight);
					col += squareSize;
				}

				row += squareSize;
			}

			numSquares *= 2;
			squareSize /= 2;
			
			// adjust this variable to determine how fast/slow the height goes down
			// smaller number represents slower height change (a smoother terrain)
			// higher number represents faster height change (a more jagged terrain)
			mHeight *= 0.45f;

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
		float _GroundHeight = -1;
		float _SandHeight = (float)-0.5;
		float _RockyHeight = (float) (mHeight - 0.1);
		float _heightOffset = (float) 1.5;
		Color color;
		Color _sandBrown = new Color32(242, 215, 160, 255);
		Color _darkGrass = new Color32(0, 102, 0, 255);
		Color _rocky = new Color32(228, 225,223, 255);
		Color _rocky2 = new Color32(212, 209, 205, 255);

		
		// water level
		if (height < _GroundHeight)
		{
			color = Color.blue;
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
		// rocky level
		else
		{
			color = Color.Lerp(_darkGrass,_rocky, (float)(height - 1));
		}
		
		
		
		
		return color;
	}
}
