using UnityEngine;
using System.Collections;

public class MStest : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		float[,] vals = new float[40,40];
		for(int i = 0; i < vals.GetLength(0); i++)
		{
			for(int j = 0; j < vals.GetLength(0); j++)
			{
				vals[i, j] = Mathf.PerlinNoise(299+i/2f, 431+j/2f);
				print(vals[i,j]);
			}
		}

		//MeshBuilder mb = MarchingSquares.buildMesh(vals);
		//MeshBuilder mb2 = MarchingSquares.buildMesh(vals);
		//mb.addMesh(mb2, Vector3.zero, Quaternion.Euler(0,90,0));
		//gameObject.GetComponent<MeshFilter>().mesh = mb.getMesh();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
