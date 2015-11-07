using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour {

	public static int chunkSize = 16; //the size of the chunk in voxel units
	public float[ , , ] voxVals = new float[chunkSize+1, chunkSize+1, chunkSize+1];//voxel values for generating terrain surface+1 to link all the meshes
	public Vector2[ , , ] voxType = new Vector2[chunkSize+1, chunkSize+1, chunkSize+1];//the type of texture that will be rendered at each voxel
	//0=grass, 1=sand
	public WorldPos pos;

	MeshFilter filter;
	MeshCollider coll;

	public bool update = false;
	public bool rendered;

	public Vector3[] verts; //tests to figure out uv mapping
	public int[] tris;

	private static float scale = 1; //scales up the terrain to make polygons bigger 

	public static float absChunkSize = chunkSize*scale;//the size of the chunk in unity units after adjusted for scale, used for positioning chunks

	// Use this for initialization
	void Start () 
	{
		//filter = gameObject.GetComponent<MeshFilter>();
		//coll = gameObject.GetComponent<MeshCollider>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (update)
		{
			update = false;
			RenderChunk();
		}
	}
	
	public void RenderChunk()
	{
		Mesh mesh = MarchingCubes.CreateMesh(voxVals, voxType);


		mesh.RecalculateNormals();



		filter = gameObject.GetComponent<MeshFilter>();
		filter.mesh = mesh;

		coll = gameObject.GetComponent<MeshCollider>();
		coll.sharedMesh = mesh;

		rendered = true;

		transform.localScale = new Vector3 (scale, scale, scale);

		verts = mesh.vertices;
		tris = mesh.triangles;
	}
}
