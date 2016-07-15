using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour {


	public static int chunkSize = 16;

	public static List<WorldPos> requestedChunks = new List<WorldPos>();

	// Use this for initialization
	void Start () 
	{
		WorldPos curChunkPos = CoordinateHandler.getChunk();
		print("initial" + curChunkPos);
		if(CoordinateHandler.curPlanet != null)
		{
			//print("aksdfaaasdfasdfasfasfasdhasdhsdxfasdfgusadhjasdujkashu");
			UniverseSystem.curPlanet.lod.updateAll(new WorldPos(curChunkPos.x/TerrainObject.chunkWidth, curChunkPos.y/TerrainObject.chunkWidth, curChunkPos.z/TerrainObject.chunkWidth));	
		}

	}
	
	// Update is called once per frame
	void Update() 
	{
		//calculate the player's real position from the floating position
		//Vector3 realPos = Unitracker.getRealPos(transform.position);

		//only generate stuff for a planet if you are on a planet
		if(CoordinateHandler.curPlanet != null)
		{
			//the current chunk the player is in
			WorldPos curChunkPos = CoordinateHandler.getChunk();

			//print(curChunkPos.toVector3().magnitude);
			//only request chunk generation (of surface objects) if the chunk is within the build heigth
			if(curChunkPos.toVector3().magnitude < CoordinateHandler.curPlanet.buildHeight)
			{
				findChunk(curChunkPos);

			}

			UniverseSystem.curPlanet.lod.updateWorld(new WorldPos(curChunkPos.x/TerrainObject.chunkWidth, curChunkPos.y/TerrainObject.chunkWidth, curChunkPos.z/TerrainObject.chunkWidth));	

			deleteChunks(curChunkPos);

		}


	
	}


	//finds a nearby chunk in need of requesting
	void findChunk(WorldPos curChunkPos)
	{

		//loop through all chunkpositions with all combinations of positive and negative components until one is found that has not been requested
		foreach(WorldPos pos in chunkPositions)
		{
			for(int xs=1; xs>=-1; xs-=2)//xs is x component sign
			{
				for(int ys=1; ys>=-1; ys-=2)
				{
					for(int zs=1; zs>=-1; zs-=2)
					{
						//-0 and zero are the same, so only check for positive zero
						if((pos.x==0 && xs==-1) || (pos.y==0 && ys==-1) || (pos.z==0 && zs==-1))
							continue;

						//calculates a proposed chunk to render based on order of chunkPositions and chunkSize
						WorldPos proposedChunk = new WorldPos(curChunkPos.x + xs*pos.x*chunkSize,
							curChunkPos.y + ys*pos.y*chunkSize,
							curChunkPos.z + zs*pos.z*chunkSize);

						//if the chunk the player/object is in has not already been requested, request it
						if(!requestedChunks.Contains(proposedChunk))
							buildSurfFromChunk(proposedChunk);
					}
				}
			}
		}
	}

	//builds the surface units from a chunk
	void buildSurfFromChunk(WorldPos chunk)
	{
		//convert the worldpos to a surface pos
		SurfacePos surfp = UnitConverter.getSP(chunk.toVector3(), UniverseSystem.curPlanet.surface.sideLength);
	
		//convert the surfacepos to a surface unit then request its creation
		UniverseSystem.curPlanet.surface.CreateSurfaceObjects(surfp.toUnit());
		requestedChunks.Add(chunk);//add it to the already requested chunks list
	}


	void deleteChunks(WorldPos curChunk)
	{
		//the list that will gather chunks to be deleted
		List<WorldPos> chunksToDelete = new List<WorldPos>();

		foreach(WorldPos pos in requestedChunks)
		{
			//NOTE: make 100 a definied variable later and change it to not 100
			//if the distance between the current chunk and one chunk still in the requestedchunks list is greater than an arbitrary value, request its destruction
			if(Vector3.Distance(curChunk.toVector3(), pos.toVector3()) > 120)
			{
				chunksToDelete.Add(pos);
			}
		}

		//now deleta all the chunks that were added to the list
		foreach(WorldPos pos in chunksToDelete) 
		{
			requestedChunks.Remove(pos);
			//convert the worldpos to a surface pos
			SurfacePos surfp = UnitConverter.getSP(pos.toVector3(), UniverseSystem.curPlanet.surface.sideLength);

			//convert the surfacepos to a surface unit then request its deletion
			UniverseSystem.curPlanet.surface.deleteSurface(surfp.toUnit());
		}
	}

	static  WorldPos[] chunkPositions = new WorldPos[]{new WorldPos(0,0,0), new WorldPos( 1, 0, 0), new WorldPos( 0, 1, 0), new WorldPos( 0, 0, 1), new WorldPos( 1, 1, 0),
		new WorldPos( 1, 0, 1), new WorldPos( 0, 1, 1), new WorldPos( 1, 1, 1), new WorldPos( 2, 0, 0), new WorldPos( 0, 2, 0), new WorldPos( 0, 0, 2), new WorldPos( 2, 1, 0),
		new WorldPos( 2, 0, 1), new WorldPos( 1, 2, 0), new WorldPos( 0, 2, 1),  new WorldPos( 1, 0, 2), new WorldPos( 0, 1, 2), new WorldPos( 2, 1, 1), new WorldPos( 1, 2, 1),
		new WorldPos( 1, 1, 2), new WorldPos( 2, 2, 0), new WorldPos( 2, 0, 2), new WorldPos( 0, 2, 2), new WorldPos( 2, 2, 1), new WorldPos( 2, 1, 2), new WorldPos( 1, 2, 2),
		new WorldPos( 2, 2, 2)};
}
