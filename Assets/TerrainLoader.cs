using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//this monobehavoir script attaches to objects that will load terrain around them
//NOTE: this will probably not be used anymore
public class TerrainLoader : MonoBehaviour 
{

	public Planet planet;//the planet the terrainloader is building for, will not be public eventually(curplanet variable or something)
	TerrainSystem terrain;
	int chunkSize = TerrainObject.chunkSize;//voxels per chunk side
	private float destroyDistance = 128;

	//List<WorldPos> buildList = new List<WorldPos>();//chunks to load voxel data
	private static List<TerrainObject> renderList = new List<TerrainObject>();//references of chunks to render
	
	
	//Loader loader;

	void Start()
	{
		terrain = planet.terrain;
	}

	//timer for deleting chunks, every tenth frame is reserved for deleting chunks
	int timer = 0;

	void Update()
	{

		//if the timer is at 10, delete some chunks
		//if not, render max 1 object per frame and add it to the object list
		//if there is nothing to render, check if something needs to be created

		if (timer == 10) 
		{
			DeleteChunks ();
			timer=0;
		}
		else if (renderList.Count > 0) 
		{
			renderList [0].Render ();
			renderList.RemoveAt (0);
		} 
		else 
		{
			FindChunkToCreate ();
		}

		timer++;
	}

	//finds chunks around the player/object that are not rendered or in the already loaded list(chunks list in terrain system class)

	//finds a chunk that is near the player and has not been created yet and creates it
	void FindChunkToCreate()
	{

		//if (objList.Count > 0) //look at this if need optimization

		//Get the unit chunk position of the current chunk that the player is in
		//basically it rounds every position down to the nearest 16
		Vector3 curChunkPos = new Vector3 (
			Mathf.Floor(transform.position.x / chunkSize)*chunkSize,
			Mathf.Floor(transform.position.y / chunkSize)*chunkSize,
			Mathf.Floor(transform.position.z / chunkSize)*chunkSize
		);

		for (int i = 0; i<chunkPositions.Length; i++) 
		{
			Vector3 newPos = new Vector3(
				curChunkPos.x+chunkPositions[i].x*chunkSize,
				curChunkPos.y+chunkPositions[i].y*chunkSize,
				curChunkPos.z+chunkPositions[i].z*chunkSize);


			//checks if this chunk already exists
			if(TerrainSystem.chunks.ContainsKey(newPos))
			{
				//print ("contains works");
				continue;//continue to check for a different chunk that doesn't already exist
			}

			//create the chunk and end the loop
			terrain.CreateChunk(newPos);
			break;

		}

	}



	//finds chunks that are too far away from the player and deletes them
	void DeleteChunks()
	{
		List<Vector3> chunksToDelete = new List<Vector3> ();//the positions of all chunks that will be deleted! MWAHAHAHAHAHAHA!!!!!!!!!!!

		//first adds finds all chunks too far away and adds them to a list
		foreach (var chunk in TerrainSystem.chunks) //REMEMBER TO READ ABOUT HOW THE VAR KEYWORD WORKS!!!!!!!!!!!!!!!!!!!!!!!!!
		{
			//distance between the player/this object to the chunk
			float distance = Vector3.Distance(chunk.Value.transform.position, transform.position);
			
			if (distance > destroyDistance)//adds the position of the chunk to the list if it is 128+ units from the player
				chunksToDelete.Add(chunk.Key);//key is the position

		}
		
		//second deletes all those chunks in the list
		foreach (Vector3 pos in chunksToDelete)
			TerrainSystem.DestroyChunk (pos);
	}



	
	

	//adds an object to the render list so it will be rendered when ready
	public static void addToRender(TerrainObject obj)
	{
		renderList.Add (obj);
	}
	
	//contains the order that chunks around the player should be loaded
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
