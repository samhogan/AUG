using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.Collections.Generic;


//one instance per planet, controls simplex noise based things such as altitude, population density, etc.
public class NoiseHandler
{
	private float radius;//the radius of the planet

	//module that outputsj
	private ModuleBase finalTerrain;
	//private ModuleBase finalTexture;

	private List<ModuleBase> substanceNoise;

	//the temperature gradient(later dependent on distance from star and atmosphere)
	private ModuleBase finalTemp;

	public NoiseHandler(float r)
	{
		radius = r;
		substanceNoise = new List<ModuleBase>();

		TempGrad basetemp = new TempGrad(110, -30, r);
		Perlin tempOffest = new Perlin(0.00001, 2, .5, 3, 52546, QualityMode.High);
		finalTemp = new Add(basetemp, new Multiply(tempOffest, new Const(30)));


		Const baseTexture = new Const(3);
		substanceNoise.Add(baseTexture);
		finalTerrain = new Const(0, 0);
		addMountains();
		addDeserts();
		addIce();
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

	private void addContinents()
	{
		Perlin continents = new Perlin(.000001, 2, .5, 6, 6734, QualityMode.High);
		finalTerrain = new Add(finalTerrain, new Multiply(continents, new Const(10000)));

	}

	private void addIce()
	{
		//texture
		ModuleBase text = new Const(Sub.ICE);
		substanceNoise.Add(text);

		//heightmap
		Const heightmap = new Const(0, substanceNoise.Count-1);

		//combine with finalTerrain

		//add the deserts
		Select addedIce = new Select(finalTerrain, heightmap, new Perlin(.00001, 2, 0.5, 4, 234, QualityMode.High));
		addedIce.Maximum = .8;

		//confine the deserts to appropriate temperatures
		Select newTerrain = new Select(finalTerrain, addedIce, finalTemp);
		newTerrain.Maximum = 32;
		newTerrain.Minimum = -1000;

		finalTerrain = newTerrain;
	}

	private void addDeserts()
	{
		//texture
		ModuleBase sandtext = new Const(Sub.SAND);
		substanceNoise.Add(sandtext);

		//heightmap
		Const heightmap = new Const(0, substanceNoise.Count-1);

		//combine with finalTerrain

		//add the deserts
		Select addedDeserts = new Select(finalTerrain, heightmap, new Perlin(.00001, 2, 0.5, 4, 2334, QualityMode.High));
		addedDeserts.Maximum = .25;

		//confine the deserts to appropriate temperatures
		Select newTerrain = new Select(finalTerrain, addedDeserts, finalTemp);
		newTerrain.Maximum = 200;
		newTerrain.Minimum = 80;

		finalTerrain = newTerrain;
	}

	public void addMountains()
	{
		//create the mountain texture
		ModuleBase rocktext = new Select(new Const(2), new Const(4), new Billow(.001, 2, .5, 2, 1, QualityMode.High));
		substanceNoise.Add(rocktext);

		//Debug.Log(substanceNoise.Count);
		//create the mountain height noise
		RidgedMultifractal rmf = new RidgedMultifractal(.0001, 2, 4, 1, QualityMode.High, substanceNoise.Count-1);
		//Multiply mounts = new Multiply(rmf, new Const(2000));
		Multiply mounts = new Multiply(new Add(rmf, new Const(1)), new Const(2000));

		//add it to the final terrain
		//Max newTerrain = new Max(mounts, finalTerrain);
		Add selector = new Add(new Perlin(.00001, 2, .5, 2, 1, QualityMode.High), new Multiply(new Perlin(0.001, 2, .5, 2, 345, QualityMode.High), new Const(.01)));
		Select newTerrain = new Select(finalTerrain, mounts, selector);
		newTerrain.FallOff = 0.01;
		finalTerrain = newTerrain;

	}

	//returns the voxel val(float value used to build the mesh with marching cubes) and type(substance) at a specific voxel
	public void getVoxData(Vector3 pos, out float val, out Sub sub)
	{
		//the distance from the center of the planet to the current voxel
		float distxyz = Vector3.Distance(Vector3.zero, pos);

		Vector3 surfPos = pos.normalized*radius;// get the spot at sea level of the planet(this is to prevent weird overhangs and stuff

		//the substance noise to use at this voxel
		int tid;//the id of the texture module to use from substanceNoise
		//sub = Sub.ICE;
		float noise = (float)finalTerrain.GetValue(surfPos.x, surfPos.y, surfPos.z, out tid);
		//Debug.Log(tid);
		sub = (Sub)substanceNoise[tid].GetValue(surfPos.x, surfPos.y, surfPos.z);
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
