using UnityEngine;
using System.Collections;

public class MeshTester : WorldObject
{

	// Use this for initialization
	void Start () 
	{
		setReferences();
		MeshBuilder mb = new MeshBuilder();
		ProceduralPrimitives.Quad(mb);
		filter.mesh = mb.getMesh();
	//filter.mesh.RecalculateBounds();
	//	filter.mesh.RecalculateNormals();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
