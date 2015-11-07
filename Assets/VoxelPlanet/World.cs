using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Noise;

public class World : MonoBehaviour {

	public int worldSize = 3;
	public float scale = 5;
	public float planetRadius = 100;

	public GameObject chunkPrefab;
	//Chunk[,,] chunks = new Chunk[worldSize, worldSize, worldSize];

	public Dictionary<WorldPos, Chunk> chunks = new Dictionary<WorldPos, Chunk>();
	// Use this for initialization
	void Start () {

		MarchingCubes.SetTarget(1);
		MarchingCubes.SetWindingOrder(2, 1, 0);
		//MarchingCubes.SetModeToCubes();

		/*for(int x=-5; x<5; x++)
		{
			for(int z=-5; z<5; z++)
			{
				for(int y=-5; y<5; y++)
				{
					CreateChunk(x, y, z);
				//print ("something");
				}
			}
		}*/
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreateChunk(int x, int y, int z) 
	{
		//the coordinates of this chunk in the world
		WorldPos worldPos = new WorldPos(x, y, z);
		
		//Instantiate the chunk at the coordinates using the chunk prefab
		GameObject newChunkObject = Instantiate(
			chunkPrefab, new Vector3(worldPos.x*Chunk.absChunkSize, worldPos.y*Chunk.absChunkSize, worldPos.z*Chunk.absChunkSize),
			Quaternion.Euler(Vector3.zero)
			) as GameObject;
		
		//Get the object's chunk component
		Chunk newChunk = newChunkObject.GetComponent<Chunk>();
		
		//Assign its values
		newChunk.pos = worldPos;
		//newChunk.world = this;
		
		//Add it to the chunks dictionary with the position as the key
		chunks.Add(worldPos, newChunk);

		PlanetChunk(newChunk);
		//newChunk.RenderChunk ();

		/*
		var terrainGen = new TerrainGen();
		newChunk = terrainGen.ChunkGen(newChunk);
		
		newChunk.SetBlocksUnmodified();
		
		bool loaded = Serialization.Load(newChunk);*/
	}

	public void DestroyChunk(int x, int y, int z)
	{
		Chunk chunk = null;
		if (chunks.TryGetValue(new WorldPos(x, y, z), out chunk))
		{
			Object.Destroy(chunk.gameObject);
			chunks.Remove(new WorldPos(x, y, z));
		}
	}
	
	
	/*public Chunk GenChunk(Chunk chunk)
	{
		for (int x = chunk.pos.x; x < chunk.pos.x + Chunk.chunkSize; x++)
		{
			for (int z = chunk.pos.z; z < chunk.pos.z + Chunk.chunkSize; z++)
			{
				chunk = ChunkColumnGen(chunk, x, z);
			}
		}
		return chunk;
	}*/

	/*float fineScale = 4;
	float fineHeight = 2f;
	
	float genScale = 20;
	float genHeight = 7;*/


	public void GenChunk(Chunk chunk)
	{
		//the unique world coordinates of the first (0,0) voxel in the chunk
		int worldX = chunk.pos.x * Chunk.chunkSize;
		int worldZ = chunk.pos.z * Chunk.chunkSize;

		for (int x = worldX; x <= worldX+Chunk.chunkSize; x++) //x is the unique world coordinate of the voxel
		{
			for (int z = worldZ; z <= worldZ+Chunk.chunkSize; z++)
			{
				float gen = Mathf.PerlinNoise(x/scale, z/scale) * genHeight; //general hill noise
				float fine = Mathf.PerlinNoise(x/fineScale, z/fineScale) * fineHeight; //fine bump noise
				float large = Mathf.PerlinNoise(x/80.0f, z/80.0f) * 30.0f; //even more general hightmap noise
				//perlin*=Mathf.PerlinNoise((chunk.pos.x*Chunk.chunkSize+x)/100.0f, (chunk.pos.z*Chunk.chunkSize+z)/100.0f);
				//print (perlin);
				for (int y = 0; y <= Chunk.chunkSize; y++) 
				{
					//x - worldx finds the local x coordinate within the chunk
					chunk.voxVals[x - worldX, y, z - worldZ] = (chunk.pos.y*Chunk.chunkSize+y)/(fine + gen + large);

				}
				//chunk = ChunkColumnGen(chunk, x, z);
			}
		}
		//return chunk;
	}


	float fineScale = 8;
	float fineHeight = 2f;
	
	float genScale = 35;
	float genHeight = 7;

	float contScale = 200;
	float contHeight = 15;

	float mtnScale = 100;
	float mtnHeight = 40;
	float mtnFrequency = 0.2f; //frequency: 0 = never occurs, 1 = always occurs

	float mtnfineScale = 4;
	float mtnfineHeight = 4f;

	//generates the voxel data for a chunk on a planet with planetRadius radius and the given noise values
	public void PlanetChunk(Chunk chunk)
	{
		NoiseGen noise = new NoiseGen ();
		//the unique world coordinates of the first (0,0) voxel in the chunk
		int worldX = chunk.pos.x * Chunk.chunkSize;
		int worldZ = chunk.pos.z * Chunk.chunkSize;
		int worldY = chunk.pos.y * Chunk.chunkSize;
		
		for (int x = worldX; x <= worldX+Chunk.chunkSize; x++) //x is the unique world coordinate of the voxel
		{
			for (int z = worldZ; z <= worldZ+Chunk.chunkSize; z++)
			{
				//float gen = Mathf.PerlinNoise(x/scale, z/scale) * genHeight; //general hill noise
				//float fine = Mathf.PerlinNoise(x/fineScale, z/fineScale) * fineHeight; //fine bump noise
				//float large = Mathf.PerlinNoise(x/80.0f, z/80.0f) * 30.0f; //even more general hightmap noise
				//perlin*=Mathf.PerlinNoise((chunk.pos.x*Chunk.chunkSize+x)/100.0f, (chunk.pos.z*Chunk.chunkSize+z)/100.0f);
				//print (perlin);
				for (int y = worldY; y <= worldY+Chunk.chunkSize; y++)
				{
					//x - worldx finds the local x coordinate within the chunk
					//chunk.voxVals[x - worldX, y, z - worldZ] = (chunk.pos.y*Chunk.chunkSize+y)/(fine + gen + large);
					float distxyS = x*x+y*y; //first part of distace formula
					float distxyz = Mathf.Sqrt(distxyS + z*z); //the distance from the center of the world to the current voxel

					Vector3 surface = new Vector3(x,y,z).normalized * planetRadius; //gives the point on face of unaltered planet sphere, directly below current voxel

					//print(surface.magnitude);

					float gen = Noise.GetNoise(x/genScale, y/genScale, z/genScale) * genHeight; //fine bumpy noise
					float fine  = Noise.GetNoise(x/fineScale, y/fineScale, z/fineScale) * fineHeight; //general hill noise
					float cont = Noise.GetNoise(x/contScale, y/contScale, z/contScale) * contHeight; //continent noise

					//gives wierd cave thing
					//float mts = (Noise.GetNoise(x/50.0f, y/50.0f, z/50.0f)-0.5f) * 20; //moountain noise 1.0f decrases frequency

					float mts = (Noise.GetNoise(surface.x/mtnScale, surface.y/mtnScale, surface.z/mtnScale)-(1f-mtnFrequency)) * mtnHeight; //moountain noise 1.0f decrases frequency

					if(mts<1f)//negatives are bad for pow, factions are bad because they result in very low numbers
						mts = 0f;
					else
					{
						mts = Mathf.Pow(mts, 1.5f);
						//fine = Noise.Noise.GetNoise(x/mtnfineScale, y/mtnfineScale, z/mtnfineScale) * mtnfineHeight;
						//gen = 0;
					}

					////layers all noise on planet radius (additive). distance of voxel/distance of noise gives final decimal voxel value for marching cubes
					chunk.voxVals[x - worldX, y - worldY, z - worldZ] = distxyz/(planetRadius + gen + cont + fine + mts); 

					Vector2 voxtype;

					if(distxyz>230)//if very high or at north and south pole, use snow texture
						voxtype = new Vector2(0.75f,0.25f);
					else if(distxyz>220)//if the distance is longer than 300, use stone/mountain texture
						voxtype = new Vector2(0.75f,0.75f);
					else if(distxyz>212)//if the distance is longer than 212(sea level), make the block grass
						voxtype = new Vector2(0.25f,0.75f);
					else//use sand/dirt texture
						voxtype = new Vector2(0.25f,0.25f);

					if(y>180 || y<-180) //the point that land is icey (north and south pole)
						voxtype = new Vector2(0.75f,0.25f);

					chunk.voxType[x - worldX, y - worldY, z - worldZ] = voxtype;
						
				}
				//chunk = ChunkColumnGen(chunk, x, z);
			}
		}


	}

	public Chunk GetChunk(int x, int y, int z)
	{
		WorldPos pos = new WorldPos(x, y, z);

		Chunk containerChunk = null;
		
		chunks.TryGetValue(pos, out containerChunk);
		
		return containerChunk;
	}
}
