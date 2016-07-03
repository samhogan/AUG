using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.Collections.Generic;
using System;
using System.Threading;

//one instance per planet, controls simplex noise based things such as altitude, population density, etc.
public class NoiseHandler
{
	private float radius;//the radius of the planet

	//module that outputsj
	private ModuleBase finalTerrain;
	private ModuleBase finalTexture;

	private List<ModuleBase> substanceNoise;

	//the temperature gradient(later dependent on distance from star and atmosphere)
	private ModuleBase finalTemp;

	public NoiseHandler(float r, ModuleBase terrain, ModuleBase texture)
	{
		radius = r;
		finalTerrain = terrain;
		finalTexture = texture;
		//substanceNoise = new List<ModuleBase>();

	/*	TempGrad basetemp = new TempGrad(110, -30, r);
		Perlin tempOffest = new Perlin(0.00001, 2, .5, 3, 52546, QualityMode.High);
		finalTemp = new Add(basetemp, new Multiply(tempOffest, new Const(30)));


		Const baseTexture = new Const(3);
		substanceNoise.Add(baseTexture);
		//finalTerrain = new Const(0, 0);

		//ModuleBase text = new Const(Sub.VEGITATION);
		ModuleBase grass = addTexture(Sub.VEGITATION, Sub.VEGITATION2, new Perlin(.0001, 2, .5, 1, 35426, QualityMode.High), .5);
		grass = addTexture(Sub.VEGITATION3, grass, new Perlin(.0001, 2, .5, 1, 326, QualityMode.High), .3);
		ModuleBase muddirt = addTexture(Sub.DIRT, Sub.MUD, new Perlin(.001, 2, .5, 1, 4546, QualityMode.High), .5);
		ModuleBase ftext = addTexture(grass, muddirt, new Perlin(.0001, 2, .5, 1, 3926, QualityMode.High), .7);
		//ftext = new Const(Sub.MUD);
		substanceNoise.Add(ftext);

		//heightmap
		ModuleBase heightMap = new Perlin(.001, 2, .5, 1, 64, QualityMode.High, substanceNoise.Count-1);
		heightMap = new Multiply(heightMap, new Const(100));
		//Const heightMap = new Const(0, substanceNoise.Count-1);
		finalTerrain = heightMap;*/

		/*for(int i = 0; i < 20; i++)
		{
			Debug.Log(eDist(10,100000));

		}*/



		//nonePreset();
		//buildTerrain();
		//PlanetBuilder.nonePreset(out finalTerrain, out finalTexture, out substanceNoise);
		//PlanetBuilder.marsPreset(out finalTerrain, out finalTexture, out substanceNoise);
		//PlanetBuilder.buildTerrain(out finalTerrain, out finalTexture, out substanceNoise);


		//PlanetBuilder.genPlanetData(out finalTerrain, out finalTexture);
		//PlanetBuilder.testPreset(out finalTerrain, out finalTexture);
		//addMountains();
		//addDeserts();
		//addIce();
	//	addContinents();
		//create some terrain height noise
		/*RidgedMultifractal rmf = new RidgedMultifractal(.0001, 2, 4, 1, QualityMode.High, new Const(Sub.ICE));
		Multiply mounts = new Multiply(rmf, new Const(2000, new Const(0)));
		Perlin hills = new Perlin(.0001, 2, .5, 4, 324, QualityMode.High);
		Multiply hillsmult = new Multiply(hills, new Const(10));
		//finalTerrain = new Max(mult, new Const(0));//, new Perlin(.001, 2, 2, 2, 1, QualityMode.High));
		Select sel = new Select(hillsmult, mounts, new Perlin(0.000001, 2, .5, 6, 34, QualityMode.High));
		sel.Maximum = 0;d
		sel.Minimum = -5;
		sel.FallOff = 5;

		Perlin continents = new Perlin(.000001, 2, .5, 4, 6734, QualityMode.High);
		finalTerrain = new Add(sel, new Multiply(continents, new Const(2000)));


		//terrain texture noise
		substanceNoise = new List<ModuleBase>();
		ModuleBase rock = new Select(new Const(2), new Const(4), new Billow(.001, 2, .5, 2, 1, QualityMode.High));
		ModuleBase rinoise = new Select(new Const(1), rock, new Perlin(.00001, 2, .5, 2, 1, QualityMode.High));
		ModuleBase veg = new Select(new Const(3), rinoise, new Perlin(.000006, 2, .5, 2, 4352, QualityMode.High));
		substanceNoise.Add(veg);*/


		/*Perlin testperl = new Perlin(.1, 2, .5, 6, 2, QualityMode.High);
		for(int i = 0; i<20; i++)
			Debug.Log(testperl.GetValue(Random.Range(-10000,10000), Random.Range(-10000,10000), Random.Range(-10000,10000))); 
			*/
	}


	//List<

	//a cache for height data to be reused
	//Dictionary <SurfacePos, float> heightData = new Dictionary<SurfacePos, float>();
	Dictionary <Vector3, float> heightData = new Dictionary<Vector3, float>();
	List<Vector3> cachedPoints = new List<Vector3>();
	//Hashtable hd = new Hashtable();

	public static int calcs = 0;
	//returns the voxel data for mc
	public float getVoxVal(Vector3 pos, int scale)
	{
		//the distance from the center of the planet to the current voxel
		float distxyz = pos.magnitude;

		Vector3 surfPos = pos.normalized*radius;// get the spot at sea level of the planet(this is to prevent weird overhangs and stuff




		/*SurfacePos surf = UnitConverter.getSP(surfPos, 98174*4);//(int)(radius * .5f * Mathf.PI/TerrainObject.wsRatio));
		//SurfacePos surf = new SurfacePos(PSide.BACK, 1, 1);//UnitConverter.getSP(surfPos, 98174);//(int)(radius * .5f * Mathf.PI/TerrainObject.wsRatio));

		//round to the nearest lev
		surf.u = Mathf.Round(surf.u);// / scale) * scale;
		surf.v = Mathf.Round(surf.v);// / scale) * scale;
*/
		//surfPos.x = Mathf.Round(surfPos.x / scale /4) * scale*4;

		float noise;
		//if(heightData.TryGetValue(surf, out noise))
		if(heightData.TryGetValue(pos, out noise))
		{
			//noise = heightData[surf];
			calcs++;
		}
		else
		{
			//Vector3 newSurf = UnitConverter.getWP(surf, radius, 98174*4);
			//noise = (float)finalTerrain.GetValue(newSurf.x, newSurf.y, newSurf.z);
			noise = (float)finalTerrain.GetValue(surfPos.x, surfPos.y, surfPos.z);
			heightData.Add(pos, noise);
			cachedPoints.Add(pos);
			//MyDebug.placeMarker(Unitracker.getFloatingPos(newSurf));
			//Debug.Log(surf.ToString());
			//calcs++;
			//Debug.Log(calcs);
		}

		//if the list is big, remove the oldest value
		if(heightData.Count>4000)
		{
			heightData.Remove(cachedPoints[0]);
			cachedPoints.RemoveAt(0);
		}
		
		//the marching cubes value is the distance to the voxel / the altitude(point on the surface) above or below that voxel
		return distxyz/(radius + noise);
	}


	public Sub getSubstance(Vector3 pos)
	{
		return (Sub)finalTexture.GetValue(pos);
	}

	//returns the voxel val(float value used to build the mesh with marching cubes) and type(substance) at a specific voxel
	public void getVoxData(Vector3 pos, out float val, out Sub sub)
	{
		//the distance from the center of the planet to the current voxel
		float distxyz = Vector3.Distance(Vector3.zero, pos);

		Vector3 surfPos = pos.normalized*radius;// get the spot at sea level of the planet(this is to prevent weird overhangs and stuff

		//the substance noise to use at this voxel
		//int tid;//the id of the texture module to use from substanceNoise
		//sub = Sub.ICE;
		//SurfacePos surf = UnitConverter.getSP(surfPos, 98174);//(int)(radius * .5f * Mathf.PI/TerrainObject.wsRatio));

		//round to the nearest lev
		//surf.u = Mathf.Round(surf.u / 134) * 123;
		//surf.v = Mathf.Round(surf.v / 5) * 3;

		//float noisea;
		//heightData.ContainsKey(new SurfacePos());
	

		float noise = (float)finalTerrain.GetValue(surfPos.x, surfPos.y, surfPos.z);
		//heightData.Add(surf, noise);

		//float noise = 2;//(float)finalTerrain.GetValue(surfPos.x, surfPos.y, surfPos.z);

		//TODO: for chunks larger than 0 lod, use the texture value at the surface of the planet

		//Debug.Log(tid);
		//////int tid = (int)finalTexture.GetValue(surfPos.x, surfPos.y, surfPos.z);
		//sub = Sub.SAND;//(Sub)substanceNoise[tid].GetValue(pos.x, pos.y, pos.z);
		//////sub = (Sub)substanceNoise[tid].GetValue(pos.x, pos.y, pos.z);
		//sub = (Sub)finalTexture.GetValue(surfPos.x, surfPos.y, surfPos.z);
		sub = (Sub)finalTexture.GetValue(pos.x, pos.y, pos.z);
		//the marching cubes value is the distance to the voxel / the altitude(point on the surface) above or below that voxel
		val = distxyz/(radius + noise);

	}

	//seriously sam? REMOVE OR CHANGE THIS OR SOMETHING GOSH!!!!!!!!
	//returns the altitude of the terrain at a given position
	public float getAltitude(Vector3 pos)
	{
		pos = pos.normalized*radius;// get the spot at sea level of the planet(this is to prevent weird overhangs and stuff


		/*float noise = Noise.GetNoise(pos.x/tScale1,pos.y/tScale1,pos.z/tScale1);
		noise-=0.5f;//adjust so the range is from -0.5 to +0.5 so terrain extends above and below sea level
		noise*=tHeight1;//5 is height, extends range from -2.5 to 2.5 for larger hills and stuff
		*/
	
		float noise = (float)finalTerrain.GetValue(pos.x, pos.y, pos.z);

		return radius + noise;


	}

	//returns the altitude adjusted position given a position on the surface  
	public Vector3 altitudePos(Vector3 pos)
	{
		return pos*(getAltitude(pos)/radius);
	}

	public double getTemp(Vector3 pos)
	{
		return finalTemp.GetValue(pos);
	}
	//returns the id of a sub at a specific position
	//later might return the substace itself
	/*public Substance getSubstace(Vector3 pos)
	{
		//if(finalTerrain.GetValue(pos.x, pos.y, pos.z) > 0)
		if(textureTest.GetValue(pos.x, pos.y, pos.z)>0)
			return Substance.subs[Sub.TEST];
		else
			return Substance.subs[Sub.ICE];
	}*/

}


/*pledge allegiance to the flag of binary
 * * * * * * * * * * 000000000000000000000000000
 * * * * * * * * * * 111111111111111111111111111
 * * * * * * * * * * 000000000000000000000000000
 * * * * * * * * * * 111111111111111111111111111
 * * * * * * * * * * 000000000000000000000000000
 11111111111111111111111111111111111111111111111
 00000000000000000000000000000000000000000000000
 11111111111111111111111111111111111111111111111
 00000000000000000000000000000000000000000000000
 11111111111111111111111111111111111111111111111
 00000000000000000000000000000000000000000000000
 */