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
	private Planet planet;//the planet the terrainloader is building for, will not be public eventually(curplanet variable or something)
	//TerrainSystem terrain;//the terrain system of the planet

	//positions of 'chunks' of objects that have already been requested
	//the chunks in here contain objects that are RENDERED whereas those in objects Dictionary contain rendered & unrendered
	public static List<WorldPos> requestedChunks = new List<WorldPos>();

	//each worldpos is associated with a list that contatins referenses to objects within it's domain that may or may not have been rendered
	public static Dictionary<WorldPos, List<WorldObject>> builtObjects = new Dictionary<WorldPos, List<WorldObject>>();

	//unorganized list of objects that are added in the order they will be rendered in
	public static List<WorldObject> objectsToRender = new List<WorldObject>();

	//length in unity units of the side of a chunk
	//NOTE: change all other chunksizes to reference this one
	public static int chunkSize = 16;

	// Use this for initialization
	void Start() 
	{
		//set planet reference to appropriate planet
		planet = UniverseSystem.curPlanet;
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

	//deletes chunks every some frames
	int delTimer = 0;

	// Update is called once per frame
	//(for now) every frame up to 1 chunk is requested and up to 1 object is rendered
	//NOTE: lets put this in a coroutine later once I know how one works
	void Update() 
	{
		//calculate the player's real position from the floating position
		Vector3 realPos = FloatingOrigin.getRealPos(transform.position);

		//the current chunk the player is in
		WorldPos curChunkPos = UnitConverter.getChunk(realPos);

		//loop through all chunkpositions until one is found that has not been requested
		for(int i = 0; i<chunkPositions.Length; i++)
		{
			//calculates a proposed chunk to render based on order of chunkPositions and chunkSize
			WorldPos proposedChunk = new WorldPos(curChunkPos.x + chunkPositions[i].x*chunkSize,
			                                      curChunkPos.y + chunkPositions[i].y*chunkSize,
			                                      curChunkPos.z + chunkPositions[i].z*chunkSize);

			//if the chunk the player/object is in has not already been requested, request it
			if(!requestedChunks.Contains(proposedChunk))
			{
				//NOTE: might change back
				requestChunkGen(proposedChunk);
				/*//print("Player is in chunk " + curChunkPos);
				requestTerrain(proposedChunk);//request terrain chunk generation
				requestSurface(proposedChunk);//request surface generation
				renderObjectsInChunk(proposedChunk);//request objects in this position to be rendered

				//print("Chunk added to requested chunks " + curChunkPos);
				requestedChunks.Add(proposedChunk);//add it to the already requested chunks list
				*/
			}
		}
		//print("objects to render " + objectsToRender.Count);
		//it the render list contains objects, render the first one in the list and remove it
		if(objectsToRender.Count > 0)
		{
			objectsToRender[0].Render();
			objectsToRender.RemoveAt(0);
		}

		//every 10th frame request deletion of chunks
		delTimer++;
		if(delTimer>=100)
		{
			delTimer = 0;
			requestChunkDeletion(curChunkPos);
		}

		//testing

		/*SurfacePos pos = UnitConverter.getSP(transform.position, 1024);
		//pos.toUnit();
		MyDebug.placeMarker(UnitConverter.getWP(new SurfacePos(pos.side, pos.toUnit().u, pos.toUnit().v), 10000, 1024), 2);
		TUBase baseU = WorldManager.curPlanet.surface.transport.getBase(pos.toUnit());
		if(baseU!=null)
			print(baseU.indexI + " " + baseU.indexJ + " conUp:" + baseU.conUp + " conRight:" + baseU.conRight + 
		  	   " conUpRight:" + baseU.conUpRight + " conUpLeft" + baseU.conUpLeft);
		else
			print(pos.toUnit().u + " " + pos.toUnit().u + " base is null");
		//print(UnitConverter.getSP(transform.position, 64*16));*/
	}

	//request chunk generation
	//requests creation of items in all systems
	void requestChunkGen(WorldPos chunk)
	{
		requestTerrain(chunk);//request terrain chunk generation
		requestSurface(chunk);//request surface generation
		renderObjectsInChunk(chunk);//request objects in this position to be rendered

		requestedChunks.Add(chunk);//add it to the already requested chunks list
	}

	//request chunk deletion of all chunks out of range
	void requestChunkDeletion(WorldPos curChunk)//cur chunk is the chunk the player is currently in
	{

		//the list that will gather chunks to be deleted
		List<WorldPos> chunksToDelete = new List<WorldPos>();

		foreach(WorldPos pos in requestedChunks)
		{
			//NOTE: make 100 a definied variable later and change it to not 100
			//if the distance between the current chunk and one chunk still in the requestedchunks list is greater than an arbitrary value, request its destruction
			if(Vector3.Distance(curChunk.toVector3(), pos.toVector3()) > 50)
			{
				chunksToDelete.Add(pos);
			}
		}

		//now deleta all the chunks that were added to the list
		foreach(WorldPos pos in chunksToDelete)
		{
			deleteTerrain(pos);
			deleteSurface(pos);
			requestedChunks.Remove(pos);
			
			//the list that contains the objects in the current chunk being deleted
			List<WorldObject> refList = null;
			if(builtObjects.TryGetValue(pos, out refList))
			{
				//disable all objects withing this chunk(make them invisible in the game world)
				//some objects will be deactivated then reactivated, but some may get deleted
				foreach(WorldObject wo in refList)
				{
					wo.gameObject.SetActive(false);
					//print
				}
			}
		}
	}
	
	//Get the unit chunk position of the current chunk that the player is in
	//basically it rounds every position down to the nearest 16
	//NOTE: Delete this later by migrating to unit converter method
/*	WorldPos posToChunk()
	{

		WorldPos curChunkPos = new WorldPos (
			Mathf.FloorToInt(transform.position.x / chunkSize)*chunkSize,
			Mathf.FloorToInt(transform.position.y / chunkSize)*chunkSize,
			Mathf.FloorToInt(transform.position.z / chunkSize)*chunkSize );

		return curChunkPos;
	}*/

	//the size of a terrain chunk is the same as a request chunk to keep it organized, and makes requesting it fairly simple
	void requestTerrain(WorldPos pos)
	{
		//NOTE: NEED to migrate this over to the worldhelper buildobject function
		planet.terrain.CreateChunk(pos);
	}

	void deleteTerrain(WorldPos pos)
	{
		//NOTE: NEED to migrate this over to the worldhelper buildobject function
		planet.terrain.DestroyChunk(pos);
	}

	//this requests the generation of a surface unit that is at the center of the chunk
	void requestSurface(WorldPos pos)
	{
		//convert the worldpos to a surface pos
		SurfacePos surfp = UnitConverter.getSP(pos.toVector3(), planet.surface.sideLength);

		//convert the surfacepos to a surface unit then request its creation
		planet.surface.CreateSurfaceObjects(surfp.toUnit());
	}

	void deleteSurface(WorldPos pos)
	{
		//convert the worldpos to a surface pos
		SurfacePos surfp = UnitConverter.getSP(pos.toVector3(), planet.surface.sideLength);
		
		//convert the surfacepos to a surface unit then request its creation
		planet.surface.deleteSurface(surfp.toUnit());

	}

	//adds all objects in a specified chunk to the objectstoRender list to be rendered later or activates them if they have previously been deactivated
	void renderObjectsInChunk(WorldPos wp)
	{ 
		//reference to list in dictionary
		List<WorldObject> refList = null;
		//if the worldpos key exists, render stuff in it
		//print(builtObjects.Count);
		if(builtObjects.TryGetValue(wp, out refList))
		{
			//print("builtObjects contains worldpos " + wp);
			//adds every object to the render list
			foreach(WorldObject wo in refList)
			{
				//if the object has previously been rendered but just disabled, activate it, if not, it has never been rendered and add it to the render list
				if(!wo.gameObject.activeSelf)
				{
					wo.gameObject.SetActive(true);
				}
				else
					objectsToRender.Add(wo);
			}

		} 
		else
		{
			//print("builtObjects does not contain " + wp);
		}
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
