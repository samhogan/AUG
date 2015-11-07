using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadChunks : MonoBehaviour {
	
	public World world;

	List<WorldPos> updateList = new List<WorldPos>();//chunks to render
	List<WorldPos> buildList = new List<WorldPos>();//chunks to load voxel data

	// Update is called once per frame
	void Update () {
		if (DeleteChunks()) //Checks if something was deleted so no new chunks are loaded this frame
			return;                 
		
		FindChunksToLoad();
		LoadAndRenderChunks();
	}

	//finds chunks around the current chunk that are not rendered or in the list to load
	void FindChunksToLoad()
	{
		//Get the unit chunk position of the current chunk that the player is in
		WorldPos curChunkPos = new WorldPos(
			Mathf.FloorToInt(transform.position.x/Chunk.absChunkSize),
			Mathf.FloorToInt(transform.position.y/Chunk.absChunkSize),
			Mathf.FloorToInt(transform.position.z/Chunk.absChunkSize)
			);
		
		//If there aren't already chunks left to render, add new chunks to render(if any)
		if (updateList.Count == 0)
		{
			//this loop version does not use chunkPositions but it does not load closest chunks first
			/*for(int x = -2; x<=2; x++)
			{
				for(int y = -2; y<=2; y++)
				{
					for(int z = -2; z<=2; z++)
					{*/

			//Cycle through the array of positions
			for (int i = 0; i < chunkPositions.Length; i++)//uses the precreated chunk position array to determine which chunks to load
			{
				//finds the position of one of the nearby chunks specified in chunkPositions
				WorldPos newChunkPos = new WorldPos(
					curChunkPos.x + chunkPositions[i].x,
					curChunkPos.y + chunkPositions[i].y,
					curChunkPos.z + chunkPositions[i].z
					);
				
				//Gets the chunk in the defined position
				Chunk newChunk = world.GetChunk(newChunkPos.x, newChunkPos.y, newChunkPos.z);
				
				//If the chunk already exists and it's already
				//rendered or in queue to be rendered continue
				if (newChunk != null && (newChunk.rendered || updateList.Contains(newChunkPos)))
					continue;

				//add the position of a chunk that will be build and rendered later
				buildList.Add(newChunkPos);

				return;
			}
		}
	}

	//builds the voxel info for the chunk and adds to the render list
	void BuildChunk(WorldPos pos)
	{
		if (world.GetChunk(pos.x, pos.y, pos.z) == null)//can remove this check later
			world.CreateChunk(pos.x, pos.y, pos.z);
			
		updateList.Add(pos);//this chunk is now build, so send it off to the render factory
	}

	void LoadAndRenderChunks()
	{
		//if there are chunks to be built, build up to 8 of them, if not, render 1 chunk
		if(buildList.Count != 0)
		{
			for (int i = 0; i < buildList.Count && i < 8; i++)
			{
				BuildChunk(buildList[0]);//builds the chunk at index 0 in the list
				buildList.RemoveAt(0);
			}
		}
		else if( updateList.Count!=0)
		{
			Chunk chunk = world.GetChunk(updateList[0].x, updateList[0].y, updateList[0].z);
			if (chunk != null)
				chunk.update = true;
			updateList.RemoveAt(0);
		}
	}

	//timer for deleting chunks
	int timer = 0;

	//deletes chunks that are too far away and returns if any chunks were deleted
	bool DeleteChunks()
	{
		
		if (timer == 10)
		{
			var chunksToDelete = new List<WorldPos>();
			foreach (var chunk in world.chunks)
			{
				float distance = Vector3.Distance(chunk.Value.transform.position, transform.position);
				
				if (distance > 128)//adds the position of the chunk to the list if it is too far away from the player
					chunksToDelete.Add(chunk.Key);
			}
			
			foreach (var chunk in chunksToDelete)
				world.DestroyChunk(chunk.x, chunk.y, chunk.z);
			
			timer = 0;
			return true;
		}
		
		timer++;
		return false;
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