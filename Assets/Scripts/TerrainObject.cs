using UnityEngine;
using System.Collections;

//the script attached to the terrain gameobject that controlls everything about it
//this is basically a terrain 'chunk'
public class TerrainObject : WorldObject
{
	public static int chunkSize = 16; //the size of the chunk in voxel units (ex. 16x16x16)
	public static int chunkWidth = 16; //the width of the chunk in unity units (8 and 16 results in 2x2x2 voxels
	public static float wsRatio = chunkWidth/chunkSize;//the ratio of the chunk width to the chunk size(scale of each voxel)
	//public static float chunkRes = .5f; //the resolution that the chunk is rendered at (.5 would result in an 8

	//voxel values for generating terrain surface+1 to link all the meshes
	//these values are decimal numbers used by the marching cubes algorithm to generate a mesh
	public float[ , , ] voxVals;// = new float[chunkSize+1, chunkSize+1, chunkSize+1];

	//the type of texture that will be rendered at each voxel
	//NOTE: may later change to hold Subs rather than vectors
	public Vector2[ , , ] voxType;// = new Vector2[chunkSize+1, chunkSize+1, chunkSize+1];

	private MeshCollider coll;

	//the scale of the terrain object (for LOD)
	public float scale;
	//if this terrain object is currently inactive and "split" into 8 smaller terrain objects
	public bool isSplit;

	void Awake()//onenable makes these references immediately after being created instead of in the next frame
	{
		coll = gameObject.AddComponent<MeshCollider>();
		setReferences();
		voxVals = new float[chunkSize+1, chunkSize+1, chunkSize+1];
		voxType = new Vector2[chunkSize+1, chunkSize+1, chunkSize+1];
		/*for(int x = 0; x < 8; x++)
			for(int y = 0; y < 8; y++)
				for(int z = 0; z < 8; z++)
				{
					voxVals[x, y, z] = y < 4 ? 0 : 2;
					voxType[x, y, z] = new Vector2(.3f,.4f);
				}*/
		reset();
	}


	//resets all instance variables of this terrainObject
	public void reset()
	{
		//voxVals = new float[chunkSize+1, chunkSize+1, chunkSize+1];
		//voxType = new Vector2[chunkSize+1, chunkSize+1, chunkSize+1];
		isSplit = false;
		scale = -1;
		filter.mesh = null;
		//maybe do some clever preserving with this later idk
		coll.sharedMesh = null;
		transform.parent = null;


	}

	//creates the mesh from the voxel data and assigns it to the mesh filter and mesh collider
	public override void Render()
	{
		//if(Time.time<10)
		//{
		Mesh mesh = MarchingCubes.CreateMesh(voxVals, voxType);
		//Mesh mesh = MarchingCubes2.CreateMesh(voxVals);
		mesh.RecalculateNormals();//not sure what this does at the moment
		filter.mesh = mesh;

		//only add colliders for level 0 terrain objects (scale 1)
		if(scale==1)
		{
			//coll = gameObject.AddComponent<MeshCollider>();
			coll.sharedMesh = mesh;
		}
		//transform.position.localScale = new Vector3(scale, scale, scale);
		//}
	}
}
