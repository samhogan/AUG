using UnityEngine;
using System.Collections;

public class MeshTester : WorldObject
{

	// Use this for initialization
	void Start () 
	{
		setReferences();
		MeshBuilder mb = new MeshBuilder();
		//ProceduralPrimitives.Quad(mb, Vector3.zero, new Vector3(4,0,0), new Vector3(0,0,4), Sub.Limestone);
		//ProceduralPrimitives.addCube(mb, Vector3.zero, Vector3.right, Vector3.forward, Vector3.up, Sub.Foyaite);
		ProceduralPrimitives.addCube(mb, Vector3.zero, new Vector3(5,5,5), new Vector3(0,-10,4), Vector3.up, Sub.Foyaite);
		filter.mesh = mb.getMesh();
	//filter.mesh.RecalculateBounds();
	//	filter.mesh.RecalculateNormals();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
