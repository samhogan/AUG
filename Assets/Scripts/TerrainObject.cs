using UnityEngine;
using System.Collections;

//the script attached to the terrain gameobject that controlls everything about it
//this is basically a terrain 'chunk'
public class TerrainObject : WorldObject
{
	public static int chunkSize = 4; //the size of the chunk in voxel units (ex. 16x16x16)
	public static int chunkWidth = 16; //the width of the chunk in unity units (8 and 16 results in 2x2x2 voxels
	public static float wsRatio = ((float)chunkWidth)/chunkSize;//the ratio of the chunk width to the chunk size(scale of each voxel)
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

	//a reference to the planet that this chunk is a piece of
	private Planet planet;
	//the chunk's lodpos
	private LODPos pos;

	private Mesh mesh;

	//could move contents down to init (maybe idk)
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



	//initialize the gameObject's things
	//I'm losing touch with reality
	public void init(LODPos lpos, Planet p)
	{

		planet = p;
		pos = lpos;
		//this lod chunk scale
		//level 0 has scale 1, level 1 has scale 2, level 2 has scale 4, etc.
		scale = (int)(Mathf.Pow(2,pos.level));//TerrainObject.wsRatio;



		//positions and scales the gameobject (maybe should move this elsewhere)
		//if it should be in unispace
		if(pos.level > LODSystem.uniCutoff)
		{

			//add it to unispace and make its parent proud
			transform.SetParent(planet.scaledRep.transform);
			gameObject.layer = 8;//add to Unispace layer
		

			//finds the adjusted scale of this terrain obj when in unispace
			float absScale = ((float)scale)/Unitracker.uniscale;
			gameObject.transform.localScale = new Vector3(absScale, absScale, absScale)*TerrainObject.wsRatio;

			//the adjusted position of this terrain obj whin in unispace
			gameObject.transform.localPosition = pos.toVector3()*TerrainObject.chunkWidth*absScale;
			gameObject.transform.localRotation = Quaternion.identity;

		}
		else//it is in normal space
		{
			gameObject.layer = 0;
			//gameObject.transform.parent = null;
			gameObject.transform.localScale = new Vector3(scale, scale, scale)*TerrainObject.wsRatio;
			gameObject.transform.localPosition = Unitracker.getFloatingPos(pos.toVector3()*scale*TerrainObject.chunkWidth);

		}

	}
	//gosh it's just a joke k?



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


	public void calculateNoise()
	{
		//loops through every voxel in the chunk (make own funtion later)
		for (int x = 0; x<=chunkSize; x++) 
		{
			for (int y = 0; y<=chunkSize; y++) 
			{
				for (int z = 0; z<=chunkSize; z++) 
				{

					//the world position of the current voxel
					Vector3 voxPos = new Vector3();//position of chunk+position of voxel within chunk
					voxPos.x = (pos.x*16+x*TerrainObject.wsRatio)*scale;
					voxPos.y = (pos.y*16+y*TerrainObject.wsRatio)*scale;
					voxPos.z = (pos.z*16+z*TerrainObject.wsRatio)*scale;



					Sub sub;//the substance of this voxel
					float voxVal;//the voxel value of this voxel for marching cubes

					//retrieve the voxel data from noise
					planet.noise.getVoxData(voxPos, out voxVal, out sub);

					//voxVal = y < 4 ? 0 : 2;
					//sub = Sub.ROCK2;

					//fills in the appropriate voxel data for marching cubes
					voxVals[x,y,z] = voxVal;//Noise.GetNoise((x+pos.x)/scale,(y+pos.y)/scale,(z+pos.z)/scale);
					//if(x+voxPos.x > 0)
					//	chunk.voxVals[x, y, z] = 2;
					//get the texture point of the substace at this vector
					voxType[x, y, z] = Substance.subs[sub].colorPoint;
					//puts a hole in the planet(just for fun
					//if(voxPos.x<10 && voxPos.x>-10 && voxPos.z<10 && voxPos.z>-10)
					//chunk.voxVals[x,y,z] = 2;


				}

			}

		}
	}

	public void calculateMesh()
	{
		mesh = MarchingCubes.CreateMesh(voxVals, voxType);
		//Mesh mesh = MarchingCubes2.CreateMesh(voxVals);
		//mesh.RecalculateNormals();//not sure what this does at the moment
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
