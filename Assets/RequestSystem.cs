using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this monobehavior script facillitates the generation of every system(terrain, surface)
//and attaches to a gameobject that things will be generated around
//this picks 3d spots around the player that tell each system what to generate
//NOTE: a surface unit must be bigger than a chunk or the generation will not work properly
//Chunk size is static, surface unit size is dynamic
public class RequestSystem : MonoBehaviour 
{
	public Planet planet;//the planet the terrainloader is building for, will not be public eventually(curplanet variable or something)
	//TerrainSystem terrain;//the terrain system of the planet

	//positions of 'chunks' of objects that have already been requested
	private List<WorldPos> requestedChunks = new List<WorldPos>();

	//length in unity units of the side of a chunk
	//NOTE: change all other chunksizes to reference this one
	public static int chunkSize = 16;

	// Use this for initialization
	void Start() 
	{
		//surface position test
		/*for(int i = -4; i<=4; i++)
		{
			for(int j = -4; j<=4; j++)
			{

				SurfacePos sp = new SurfacePos(PSide.TOP, i, j);
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = UnitConverter.getWP(sp, 200, 8);
				
				
			}
		}*/
		//terrain = planet.terrain;
	}
	
	// Update is called once per frame
	void Update() 
	{
		//the current chunk the player is in
		WorldPos curChunkPos = posToChunk();

		//if the chunk the player/object is in has not already been requested, request it
		if(!requestedChunks.Contains(curChunkPos))
		{
			requestSurface(curChunkPos);
			requestTerrain(curChunkPos);
			requestedChunks.Add(curChunkPos);
		}

		//testing
		//print(UnitConverter.getSP(transform.position, 8));
	}
	
	//Get the unit chunk position of the current chunk that the player is in
	//basically it rounds every position down to the nearest 16
	WorldPos posToChunk()
	{

		WorldPos curChunkPos = new WorldPos (
			Mathf.FloorToInt(transform.position.x / chunkSize)*chunkSize,
			Mathf.FloorToInt(transform.position.y / chunkSize)*chunkSize,
			Mathf.FloorToInt(transform.position.z / chunkSize)*chunkSize );

		return curChunkPos;
	}

	//the size of a terrain chunk is the same as a request chunk to keep it organized, and makes requesting it fairly simple
	void requestTerrain(WorldPos pos)
	{
		planet.terrain.CreateChunk(pos.toVector3());
	}

	//this requests the generation of a surface unit that is at the center of the chunk
	void requestSurface(WorldPos pos)
	{
		//convert the worldpos to a surface pos
		SurfacePos surfp = UnitConverter.getSP(pos.toVector3(), planet.surface.sideLength);

		//convert the surfacepos to a surface unit then request its creation
		planet.surface.CreateSurfaceObjects(surfp.toUnit());
	}

	//contains the order that 'chunks' around the player should be loaded
	static  WorldPos[] chunkPositions = new WorldPos[]{new WorldPos( 0, 0, 0), 
		new WorldPos( 1, 0, 0), new WorldPos( -1, 0, 0), 
		new WorldPos( 0, 1, 0),new WorldPos( 0, -1, 0), 
		new WorldPos( 0, 0, 1), new WorldPos( 0, 0, -1), 
		new WorldPos( 1, 1, 0), new WorldPos( 1, -1, 0), new WorldPos( -1, 1, 0), new WorldPos( -1, -1, 0), 
		new WorldPos( 1, 0, 1), new WorldPos( 1, 0, -1), new WorldPos( -1, 0, 1), new WorldPos( -1, 0, -1), 
		new WorldPos( 0, 1, 1), new WorldPos( 0, 1, -1), new WorldPos( 0, -1, 1), new WorldPos( 0, -1, -1), 
		new WorldPos( 1, 1, 1), new WorldPos( 1, 1, -1), new WorldPos( 1, -1, 1), new WorldPos( 1, -1, -1), 
		new WorldPos( -1, 1, 1), new WorldPos( -1, 1, -1), new WorldPos( -1, -1, 1), new WorldPos( -1, -1, -1), };
}
