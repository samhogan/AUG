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
	//(for now) every frame up to 1 chunk is requested and up to 1 object is rendered
	void Update() 
	{
		//the current chunk the player is in
		WorldPos curChunkPos = UnitConverter.toWorldPos(transform.position);


		//if the chunk the player/object is in has not already been requested, request it
		if(!requestedChunks.Contains(curChunkPos))
		{
			print("Player is in chunk " + curChunkPos);
			requestTerrain(curChunkPos);//request terrain chunk generation
			requestSurface(curChunkPos);//request surface generation
			renderObjectsInChunk(curChunkPos);//request objects in this position to be rendered

			print("Chunk added to requested chunks " + curChunkPos);
			requestedChunks.Add(curChunkPos);//add it to the already requested chunks list
		}

		//print("objects to render " + objectsToRender.Count);
		//it the render list contains objects, render the first one in the list and remove it
		if(objectsToRender.Count > 0)
		{
			objectsToRender[0].Render();
			objectsToRender.RemoveAt(0);
		}

		//testing
		//print(UnitConverter.getSP(transform.position, 8));
	}
	
	//Get the unit chunk position of the current chunk that the player is in
	//basically it rounds every position down to the nearest 16
	//NOTE: Delete this later by migrating to unit converter method
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
		//NOTE: NEED to migrate this over to the worldhelper buildobject function
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

	//adds all objects in a specified chunk to the objectstoRender list to be rendered later
	void renderObjectsInChunk(WorldPos wp)
	{ 
		//reference to list in dictionary
		List<WorldObject> refList = null;
		//if the worldpos key exists, render stuff in it
		print(builtObjects.Count);
		if(builtObjects.TryGetValue(wp, out refList))
		{
			print("builtObjects contains worldpos " + wp);
			//adds every object to the render list
			foreach(WorldObject wo in refList)
			{
				print("An object is added to the render list in renderOBjectsinchunk function");
				objectsToRender.Add(wo);
			}

		} 
		else
			print("builtObjects does not contain " + wp);
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
