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
	//private Planet planet;//the planet the terrainloader is building for, will not be public eventually(curplanet variable or something)
	//TerrainSystem terrain;//the terrain system of the planet

	//positions of 'chunks' of objects that have already been requested
	//the chunks in here contain objects that are RENDERED whereas those in objects Dictionary contain rendered & unrendered
	public static List<WorldPos> requestedChunks = new List<WorldPos>();

	//each worldpos is associated with a list that contatins referenses to objects within it's domain that may or may not have been rendered
	public static Dictionary<WorldPos, List<WorldObject>> builtObjects = new Dictionary<WorldPos, List<WorldObject>>();

	//unorganized list of objects that are added in the order they will be rendered in
	public static List<WorldObject> objectsToRender = new List<WorldObject>();

	//terrain pieces of all lod are in a separate render list because all terrain is rendered first
	//public static List<TerrainObject> terrainToRender = new List<TerrainObject>();

	//terrain chunks to be deactivated and its pieces rendered
	//public static List<TerrainObject> terrainToSplitRender = new List<TerrainObject>();

	//length in unity units of the side of a chunk
	//NOTE: change all other chunksizes to reference this one
	public static int chunkSize = 16;

	// Use this for initialization
	void Start() 
	{
		/*for (int x = -2000; x < 2000; x += 10)
			for (int y = 249000; y < 251000; y += 10)
				for (int z = -2000; z < 2000; z += 10) {
					Vector3 pos = new Vector3 (x, y, z);
					SurfacePos spos = UnitConverter.getSP(pos, UniverseSystem.curPlanet.surface.sideLength);
					if (float.IsNaN (spos.u) || float.IsNaN (spos.v))
						MyDebug.placeMarker (Unitracker.getFloatingPos(pos), 20);
				
				
				}
					
		Debug.Log(UnitConverter.getSP(new WorldPos(16,250016,16).toVector3(), UniverseSystem.curPlanet.surface.sideLength));
		Debug.Log(UnitConverter.getSP(new Vector3(16,250016,15), UniverseSystem.curPlanet.surface.sideLength));
		SurfacePos sp = UnitConverter.getSP(new WorldPos(16,250016,0).toVector3(), UniverseSystem.curPlanet.surface.sideLength);
		//print (float.IsNaN (sp.v));
		Debug.Log(UnitConverter.getSP(new WorldPos(16,250016,10).toVector3(), UniverseSystem.curPlanet.surface.sideLength));
		Debug.Log(UnitConverter.getSP(new WorldPos(16,250016,143).toVector3(), UniverseSystem.curPlanet.surface.sideLength));
		Debug.Log(UnitConverter.getSP(new WorldPos(16,250016,16).toVector3(), UniverseSystem.curPlanet.surface.sideLength));
*/
		//set planet reference to appropriate planet
		//planet = UniverseSystem.curPlanet;
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


	WorldPos currentChunk = new WorldPos(0,0,0);
	//deletes chunks every some frames
	int delTimer = 0;

	// Update is called once per frame
	//(for now) every frame up to 1 chunk is requested and up to 1 object is rendered
	//NOTE: lets put this in a coroutine later once I know how one works
	void Update() 
	{
		//calculate the player's real position from the floating position
		Vector3 realPos = Unitracker.getRealPos(transform.position);

		//only generate stuff for a planet if you are on a planet
		if(Unitracker.onPlanet)
		{
			//the current chunk the player is in
			WorldPos curChunkPos = UnitConverter.getChunk(realPos);

			//only request chunk generation (of surface objects) if the chunk is within the build heigth
			if(curChunkPos.toVector3().magnitude < UniverseSystem.curPlanet.buildHeight)
			{
				findChunk(curChunkPos);

			}
			//if the player is in a new chunk, update the lod
			if(curChunkPos!=currentChunk)
			{

				currentChunk = curChunkPos;
				//Debug.Log("lod updated");
				//UniverseSystem.curPlanet.lod.updateLOD(new WorldPos(curChunkPos.x/16, curChunkPos.y/16, curChunkPos.z/16));	
			}
			//Debug.Log("CMON!!!!!!!!!!!!");
			UniverseSystem.curPlanet.lod.updateLOD(new WorldPos(curChunkPos.x/16, curChunkPos.y/16, curChunkPos.z/16));	
			
				//UniverseSystem.curPlanet.lod.updateLOD(new WorldPos(curChunkPos.x/16, curChunkPos.y/16, curChunkPos.z/16));	
			
			//print("objects to render " + objectsToRender.Count);

			//if the terrain render list contains terrain, render one, if not,
			//it the render list contains objects, render the first one in the list and remove it
			/*if(terrainToRender.Count > 0)
			{
				terrainToRender[0].Render();
				terrainToRender.RemoveAt(0);
			}*/
			//Debug.Log(UniverseSystem.curPlanet.lod.chunksToSplitRender.ToString());

			//render terrain if there is some, or else render an object
			if(UniverseSystem.curPlanet.lod.chunksToSplitRender.Count > 0)
			{
				//Debug.Log("(rs) Chunks contains key :" + UniverseSystem.curPlanet.lod.chunksToSplitRender[0].ToString() + " " 
				  //        + UniverseSystem.curPlanet.lod.chunks.ContainsKey(UniverseSystem.curPlanet.lod.chunksToSplitRender[0]));
				UniverseSystem.curPlanet.lod.splitRender(UniverseSystem.curPlanet.lod.chunksToSplitRender[0]);	

				UniverseSystem.curPlanet.lod.chunksToSplitRender.RemoveAt(0);
			}
			else if(objectsToRender.Count > 0)
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
				//requestTerrain(curChunkPos);
			}
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
							requestChunkGen(proposedChunk);
					}
				}
			}
		}
	}

	//request chunk generation
	//requests creation of items in all systems
	void requestChunkGen(WorldPos chunk)
	{
		//requestTerrain(chunk);//request terrain chunk generation
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
			if(Vector3.Distance(curChunk.toVector3(), pos.toVector3()) > 120)
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
		//UniverseSystem.curPlanet.terrain.CreateChunk(pos);
		//if(Time.time<3)

		//terrain chunks are on a unit scale rather than a 16 scale, so convert it
		LODPos adjustedPos = new LODPos(0, pos.x/16, pos.y/16, pos.z/16);
		//request the level 0 lodchunk 
		//only request it if it contains land!!!
		if(UniverseSystem.curPlanet.lod.containsLand(adjustedPos))
			UniverseSystem.curPlanet.lod.requestChunk(adjustedPos);
		//print(pos);
	}

	void deleteTerrain(WorldPos pos)
	{
		//NOTE: NEED to migrate this over to the worldhelper buildobject function
		//UniverseSystem.curPlanet.terrain.DestroyChunk(pos);
	}

	//this requests the generation of a surface unit that is at the center of the chunk
	void requestSurface(WorldPos pos)
	{
		//convert the worldpos to a surface pos
		SurfacePos surfp = UnitConverter.getSP(pos.toVector3(), UniverseSystem.curPlanet.surface.sideLength);
		//Debug.Log (pos);
		//convert the surfacepos to a surface unit then request its creation
		UniverseSystem.curPlanet.surface.CreateSurfaceObjects(surfp.toUnit());
	}

	void deleteSurface(WorldPos pos)
	{
		//convert the worldpos to a surface pos
		SurfacePos surfp = UnitConverter.getSP(pos.toVector3(), UniverseSystem.curPlanet.surface.sideLength);
		
		//convert the surfacepos to a surface unit then request its creation
		UniverseSystem.curPlanet.surface.deleteSurface(surfp.toUnit());

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
	/*static  WorldPos[] chunkPositions = new WorldPos[]{new WorldPos( 0, 0, 0), 
		new WorldPos( 1, 0, 0), new WorldPos( -1, 0, 0), 
		new WorldPos( 0, 1, 0),new WorldPos( 0, -1, 0), 
		new WorldPos( 0, 0, 1), new WorldPos( 0, 0, -1), 
		new WorldPos( 1, 1, 0), new WorldPos( 1, -1, 0), new WorldPos( -1, 1, 0), new WorldPos( -1, -1, 0), 
		new WorldPos( 1, 0, 1), new WorldPos( 1, 0, -1), new WorldPos( -1, 0, 1), new WorldPos( -1, 0, -1), 
		new WorldPos( 0, 1, 1), new WorldPos( 0, 1, -1), new WorldPos( 0, -1, 1), new WorldPos( 0, -1, -1), 
		new WorldPos( 1, 1, 1), new WorldPos( 1, 1, -1), new WorldPos( 1, -1, 1), new WorldPos( 1, -1, -1), 
		new WorldPos( -1, 1, 1), new WorldPos( -1, 1, -1), new WorldPos( -1, -1, 1), new WorldPos( -1, -1, -1), };*/

	static  WorldPos[] chunkPositions = new WorldPos[]{new WorldPos(0,0,0), new WorldPos( 1, 0, 0), new WorldPos( 0, 1, 0), new WorldPos( 0, 0, 1), new WorldPos( 1, 1, 0),
		new WorldPos( 1, 0, 1), new WorldPos( 0, 1, 1), new WorldPos( 1, 1, 1), new WorldPos( 2, 0, 0), new WorldPos( 0, 2, 0), new WorldPos( 0, 0, 2), new WorldPos( 2, 1, 0),
		new WorldPos( 2, 0, 1), new WorldPos( 1, 2, 0), new WorldPos( 0, 2, 1),  new WorldPos( 1, 0, 2), new WorldPos( 0, 1, 2), new WorldPos( 2, 1, 1), new WorldPos( 1, 2, 1),
		new WorldPos( 1, 1, 2), new WorldPos( 2, 2, 0), new WorldPos( 2, 0, 2), new WorldPos( 0, 2, 2), new WorldPos( 2, 2, 1), new WorldPos( 2, 1, 2), new WorldPos( 1, 2, 2),
		new WorldPos( 2, 2, 2)};

}
