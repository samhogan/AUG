using UnityEngine;
using System.Collections;

public class MStest : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		float[,] vals = new float[20,20];
		for(int i = 0; i < vals.GetLength(0); i++)
		{
			for(int j = 0; j < vals.GetLength(0); j++)
			{
				vals[i, j] = Mathf.PerlinNoise(299+i/10f, 431+j/10f);
				print(vals[i,j]);
			}
		}

		MeshBuilder mb = MarchingSquares.buildMesh(vals);
		gameObject.GetComponent<MeshFilter>().mesh = mb.getMesh();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
