using UnityEngine;
using System.Collections;

//the script attached to the terrain gameobject that controlls everything about it
//this is basically a terrain 'chunk'
public class TerrainObject : WorldObject
{
	public static int chunkSize = 16; //the size of the chunk in voxel units (ex. 16x16x16)

	//voxel values for generating terrain surface+1 to link all the meshes
	//these values are decimal numbers used by the marching cubes algorithm to generate a mesh
	public float[ , , ] voxVals = new float[chunkSize+1, chunkSize+1, chunkSize+1];
	//public Vector2[ , , ] voxType = new Vector2[chunkSize+1, chunkSize+1, chunkSize+1];//the type of texture that will be rendered at each voxel

	private MeshCollider coll;

	//the scale of the terrain object (for LOD)
	public int scale;
	//if this terrain object is currently inactive and "split" into 8 smaller terrain objects
	public bool isSplit;

	void OnEnable()//onenable makes these references immediately after being created instead of in the next frame
	{
		setReferences();
		
		coll = gameObject.GetComponent<MeshCollider>();
		isSplit = false;
	}

	//creates the mesh from the voxel data and assigns it to the mesh filter and mesh collider
	public override void Render()
	{
		Mesh mesh = MarchingCubes2.CreateMesh(voxVals);
		mesh.RecalculateNormals();//not sure what this does at the moment
		filter.mesh = mesh;
		coll.sharedMesh = mesh;
		//transform.position.localScale = new Vector3(scale, scale, scale);

	}
}
