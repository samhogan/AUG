using UnityEngine;
using System.Collections;
using LibNoise;
using System.Collections.Generic;
//holds all the information for the planet including terrain and civilization
public class Planet : CelestialBody
{

	//private RoadSystem roads;//builds the roads/civilization organization
	//public TerrainSystem terrain;//builds the terrain with voxels and marching cubes
	public SurfaceSystem surface;//builds the objects that are on the planet surface
	public LODSystem lod;//builds terrain with voxels and marching cubes on multiple levels of detail!!!

	public NoiseHandler noise;//contains all perlin noise info 






	//the initial lod level (8 chunks of that level)
	public int startLev;



	//later will have many parameters
	public Planet(float r, LongPos pos, int _seed):base(_seed, r, pos)//Vector3 sp, float r)
	{
		

		//generate the planet surface data
		ModuleBase finalTerrain, finalTexture;
		List<Blueprint> blueprints;
		PlanetBuilder.genPlanetData(seed, out finalTerrain, out finalTexture, out blueprints);

		noise = new NoiseHandler(radius, finalTerrain, finalTexture);

		//terrain = new TerrainSystem(this, radius);
		//surface = new SurfaceSystem(radius, 400);
		surface = new SurfaceSystem(this, radius, (int)(radius/50), blueprints);//number of surface units per side is radius/50

		lod = new LODSystem(this);
		//TODO: does this 16 hold relavance?
		startLev = Mathf.CeilToInt(Mathf.Log(radius/TerrainObject.chunkWidth, 2))-1;



		createRep();
	}

	//instantiates the unispace rep of the planet
	protected override void createRep()
	{
		scaledRep =  new GameObject("Planet " + seed + "radius: " + radius);


		//the gameobject that holds the scaledRep's mesh data so it can be scaled
		GameObject meshobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		meshobj.transform.SetParent(scaledRep.transform);
		GameObject.Destroy(meshobj.GetComponent<SphereCollider>());//remove this pesky component


		scaledRep.layer = (int)spaces.Stellar;//add to Unispace layer
		meshobj.layer = (int)spaces.Stellar;

		//arbitrary unipos for testing
		////scaledRep.transform.position = PositionController.getStellarFloatingPos(scaledPos);
		//scaledRep.transform.rotation = Quaternion.Euler(0,45,0);
		scaledRep.transform.rotation = Quaternion.Euler(0,0,0);

		meshobj.transform.localScale = new Vector3(scaledRadius*2, scaledRadius*2, scaledRadius*2);

		/*for(int x = -1; x <= 0; x++)
			for(int y = -1; y <= 0; y++)
				for(int z = -1; z <= 0; z++)
				{
					LODPos lp = new LODPos(startLev, x, y, z);
					lod.CreateChunk(lp, true);
					lod.splitChunk(lp);
					lod.splitCalc(lp);
					lod.splitRender(lp);
				}*/
		//build all the starting lev chunks
		for(int x = -2; x <= 1; x++)
			for(int y = -2; y <= 1; y++)
				for(int z = -2; z <= 1; z++)
				{
					LODPos lp = new LODPos(startLev, x, y, z);
					if(lod.containsLand(lp))
					{
						lod.CreateChunk(lp, true);
						lod.splitChunk(lp);
						lod.splitCalc(lp);
						lod.splitRender(lp);
					}
				}

		startLev--;
		//lod.splitChunk(new LODPos(14,0,0,0));
		//lod.splitChunk(new LODPos(13,1,1,1));
		//lod.requestChunk(new LODPos(0, 3000,5400,9000));

		//Debug.Log(lod.containsLand(new LODPos(14,0,0,0)));
		//Debug.Log(lod.containsLand(new LODPos(12,3,3,3)));


		//add some fun color
		meshobj.GetComponent<MeshRenderer>().material = Resources.Load("Water") as Material;//loads the default material, will remove this
		meshobj.SetActive(false);
	}

}
