using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	private Vector3[] mVerts;
	private Vector2[] mUVs;
	private int[] mTris;

	// Use this for initialization
	void Start ()
	{

		mVerts = new Vector3[4];
		mUVs = new Vector2[4];
		mTris = new int[6];
		
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		
		mVerts[0] = new Vector3(-1.0f, 0.0f, 1.0f);
		mVerts[1] = new Vector3(1.0f, 0.0f, 1.0f);
		mVerts[2] = new Vector3(-1.0f, 0.0f, -1.0f);
		mVerts[3] = new Vector3(1.0f, 0.0f, -1.0f);
			
		
		mUVs[0] = new Vector2(0.0f, 0.0f);
		mUVs[1] = new Vector2(1.0f, 0.0f);
		mUVs[2] = new Vector2(0.0f, 1.0f);
		mUVs[3] = new Vector2(1.0f, 1.0f);


		mTris[0] = 0;
		mTris[1] = 1;
		mTris[2] = 3;




		mesh.vertices = mVerts;
		mesh.uv = mUVs;
		mesh.triangles = mTris;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();




	}
	
}
