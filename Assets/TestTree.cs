using UnityEngine;
using System.Collections;

//this is my test object 
//want to hear a joke? I didn't think so.
public class TestTree : MobileObjects
{

	public override void init()
	{

	}

	void OnEnable()//onenable makes these references immediately after being created instead of in the next frame
	{
		setReferences();
		
		//coll = gameObject.GetComponent<MeshCollider>();
		
	}

	public override void Render()
	{
		//print("RENDERING OBJECT");
		//Mesh mesh = MarchingCubes2.CreateMesh(voxVals);
		//mesh.RecalculateNormals();//not sure what this does at the moment
		//Mesh mesh = Resources.Load("tree", typeof(Mesh)) as Mesh;
		//filter.mesh = mesh;
		//coll.sharedMesh = mesh;

		//load a tree and parent it to this gameobject
		GameObject go = Resources.Load("Test things/Tree") as GameObject;

		Quaternion rot = Quaternion.FromToRotation(Vector3.up, transform.position);
		GameObject goInst = GameObject.Instantiate(go, transform.position, rot) as GameObject;
		goInst.transform.parent = transform;
		//gameObject.SetActive(true);
		
	}


}
