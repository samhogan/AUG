using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this monobehavior script facillitates the generation of every system(terrain, surface)
//and attaches to a gameobject that things will be generated around
//this picks 3d spots around the player that tell each system what to generate
public class RequestSystem : MonoBehaviour 
{
	public Planet planet;//the planet the terrainloader is building for, will not be public eventually(curplanet variable or something)
	TerrainSystem terrain;//the terrain system of the planet

	//positions of 'chunks' of objects that have already been requested
	private List<WorldPos> requestedChunks = new List<WorldPos>();

	//length in unity units of the side of a chunk
	//NOTE: change all other chunksizes to reference this one
	public static int chunkSize = 16;

	// Use this for initialization
	void Start() 
	{
		terrain = planet.terrain;
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
		terrain.CreateChunk(pos.toVector3());
	}

	//this requests the generation of a surface unit that is at the center of the chunk
	void requestSurface(WorldPos pos)
	{

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
