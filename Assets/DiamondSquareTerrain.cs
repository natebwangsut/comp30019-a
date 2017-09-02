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

		float halfSize = mSize * 0.5f;
		float divisionSize = mSize / mDivisions;

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		
		
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
		mVerts[mDivisions].y = Random.Range(-mHeight, mHeight); // top right
		mVerts[mVerts.Length - 1].y = Random.Range(-mHeight, mHeight); // bottom right
		mVerts[mVerts.Length - 1 - mDivisions].y = Random.Range(-mHeight, mHeight); // bottom left

		
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
			mHeight *= 0.5f;

		}
		// setting the mesh
		mesh.vertices = mVerts;
		mesh.uv = uvs;
		mesh.triangles = tris;


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
		int mid = (int) (row + halfSize) * (mDivisions + 1) + (int) (col + halfSize);
		// multiplication is faster than division
		// offset is the random value added to average
		mVerts[mid].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[botLeft].y + mVerts[botLeft + size].y)*0.25f + Random.Range(-offset,offset);
			
		
		// SQUARE STEP
		// Average out between the three surrounding vertices
		mVerts[topLeft + halfSize].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[mid].y) / 3 + Random.Range(-offset, offset);
		mVerts[mid - halfSize].y = (mVerts[topLeft].y + mVerts[mid].y + mVerts[botLeft].y) / 3 + Random.Range(-offset, offset);

		mVerts[mid + halfSize].y = (mVerts[topLeft + size].y + mVerts[mid].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);

		mVerts[botLeft + halfSize].y = (mVerts[botLeft].y + mVerts[mid].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
	}
}
