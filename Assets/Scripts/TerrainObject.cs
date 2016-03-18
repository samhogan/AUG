﻿using UnityEngine;
using System.Collections;

//the script attached to the terrain gameobject that controlls everything about it
//this is basically a terrain 'chunk'
public class TerrainObject : WorldObject
{
	public static int chunkSize = 8; //the size of the chunk in voxel units (ex. 16x16x16)
	public static int chunkWidth = 16; //the width of the chunk in unity units (8 and 16 results in 2x2x2 voxels
	public static float wsRatio = chunkWidth/chunkSize;//the ratio of the chunk width to the chunk size(scale of each voxel)
	//public static float chunkRes = .5f; //the resolution that the chunk is rendered at (.5 would result in an 8

	//voxel values for generating terrain surface+1 to link all the meshes
	//these values are decimal numbers used by the marching cubes algorithm to generate a mesh
	public float[ , , ] voxVals = new float[chunkSize+1, chunkSize+1, chunkSize+1];
	//public Vector2[ , , ] voxType = new Vector2[chunkSize+1, chunkSize+1, chunkSize+1];//the type of texture that will be rendered at each voxel

	private MeshCollider coll;

	//the scale of the terrain object (for LOD)
	public float scale;
	//if this terrain object is currently inactive and "split" into 8 smaller terrain objects
	public bool isSplit;

	void OnEnable()//onenable makes these references immediately after being created instead of in the next frame
	{
		setReferences();

		isSplit = false;
	}

	//creates the mesh from the voxel data and assigns it to the mesh filter and mesh collider
	public override void Render()
	{
		//if(Time.time<10)
		//{
		Mesh mesh = MarchingCubes2.CreateMesh(voxVals);
		mesh.RecalculateNormals();//not sure what this does at the moment
		filter.mesh = mesh;

		//only add colliders for level 0 terrain objects (scale 1)
		if(scale==1)
		{
			coll = gameObject.AddComponent<MeshCollider>();
			coll.sharedMesh = mesh;
		}
		//transform.position.localScale = new Vector3(scale, scale, scale);
		//}
	}
}