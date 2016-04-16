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
	private ModuleBase finalTexture;

	private List<ModuleBase> substanceNoise;

	//the temperature gradient(later dependent on distance from star and atmosphere)
	private ModuleBase finalTemp;

	public NoiseHandler(float r)
	{
		radius = r;
		substanceNoise = new List<ModuleBase>();

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

		for(int i = 0; i < 20; i++)
		{
			Debug.Log(eDist(10,100000));

		}




		buildTerrain();
		//marsPreset();
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

	//generates finalTerrain and finalTexture
	private void buildTerrain()
	{
		//list that contains all substances that have been used
		List<Sub> subList = new List<Sub>();

		//the number of "biomes" or types of terrain that use different texture ids
		int numBiomes = 1;

		//loop through and create all the biomes and compose them
		for(int biome = 1; biome<=numBiomes; biome++)
		{
			//if in first iteration, generate a base layer of rock (although can have any terrain features)
			//generate texture


			//the heightmap for this biome
			ModuleBase heightMap = null;
			//the number of terrain features that will be composed(selected)
			int numFeatures = 1;//Random.Range(1,6);
			//loop through and create all the features
			for(int feature = 1; feature <= numFeatures; feature++)
			{
				double scale = eDist(10, 10000);
				//scale = 100;
				//the starting noise for the final feature that will be modified
				ModuleBase finalFeature = new Perlin(1/scale,//randDoub(.00001, 0.1), 
													randDoub(1.8, 2.2), 
													randDoub(.4, .6), 
													Random.Range(1, 6), 
													Random.Range(int.MinValue, int.MaxValue), 
													QualityMode.High);
				
				//the amplidude or max height of the terrain
				//NOTE: later will be related to the frequency
				double amplitude = scale/4;//randDoub(2, 100);
				//bias is the number added to the noise before multiplying
				//-1 makes canyons/indentions, 1 makes all feautures above sea level
				//NOTE: later make a greater chance to be 1 or -1
				double bias = randDoub(-1, 1);

				finalFeature = new ScaleBias(amplitude, bias * amplitude, finalFeature);

				//the number of subfeatures to add
				//a subfeature can be adding more noise, terracing, exponentiation, etc. but NOT selecting
				int numSubFeatures = 0;
				for(int subfeature = 1; subfeature <= numSubFeatures; subfeature++)
				{
					//
				}


				//if this is the first feature, make it the entire finalFeature to be added to in the next iteration
				if(feature == 1)
				{
					heightMap = finalFeature;
				}
				else
				{
					//the base control for the selector that adds this feature to the biome
					ModuleBase baseControl = new Perlin(randDoub(.000001, 0.001), 
						                        randDoub(1.8, 2.2), 
						                        randDoub(.4, .6), 
						                        Random.Range(1, 6), 
						Random.Range(int.MinValue, int.MaxValue), QualityMode.High);

					//make possible edge controller
					//loop and make inner controllers

					//the amount to add of this feature to the biome(0 is add none, 1 is completely cover)
					//NOTE: later amount will be somewhat dependant on the feature number(feature #6 will have an average lower amount than feature #2)
					double amount = Random.value;
					double falloff = .5;
					heightMap = addModule(finalFeature, heightMap, baseControl, amount, falloff);
				}
			}

			//if it is the first biome, add biome 100% to planet as a base
			if(biome == 1)
			{
				finalTerrain = heightMap;
			}
			else
			{
			}

		}

		Const testTexture = new Const(Sub.BASALT2);
		substanceNoise.Add(testTexture);
		finalTexture = new Const(0.0);
	}
		
	//returns a random float between the two values in an e^x distribution
	private float eDist(double min, double max)
	{
		double emin = Mathf.Log((float)min);
		double emax = Mathf.Log((float)max);

		return Mathf.Exp((float)randDoub(emin, emax));
	}
	//returns a random double between the two values
	private double randDoub(double min, double max)
	{
		return Random.value*(max-min)+min;
	}

	//a test preset that creates a mars like planet used to figure out how to build this planet generator
	private void marsPreset()
	{

		ModuleBase mainControl = new Perlin(.0001, 2, .5, 4, 634234, QualityMode.High);
		ModuleBase edgeControl = new RidgedMultifractal(.001, 2, 3, 5723, QualityMode.High);
		edgeControl = new ScaleBias(.0, 0, edgeControl);
		ModuleBase finalControl = new Add(mainControl, edgeControl);
		ModuleBase text = addModule(Sub.IRONOXIDE, Sub.IRONOXIDE2, finalControl, .5);
		substanceNoise.Add(text);

	/*	ModuleBase hills = new Perlin(.001, 2, 0.5, 3, 4353, QualityMode.Low, substanceNoise.Count-1);
		hills = new Add(hills, new Const(1));
		hills = new Multiply(hills, new Const(100));

		ModuleBase plains = new Perlin(.001, 2, .5, 3, 724, QualityMode.High, substanceNoise.Count-1);
		plains = new Multiply(plains, new Const(3));

		ModuleBase hpcontrol = new Perlin(.0005, 2, .5, 5, 45623, QualityMode.High);

		Select hpselector = new Select(hills, plains, hpcontrol);
		hpselector.FallOff = 1;*/

		ModuleBase plains = new Perlin(.001, 2, .5, 3, 724, QualityMode.High, substanceNoise.Count-1);
		plains = new Multiply(plains, new Const(3));

		//ModuleBase cliffthingsbase = new Perlin(.001, 2, .5, 4, 63443, QualityMode.High);
		ModuleBase cliffthingsbase = new RidgedMultifractal(.001, 2, 4, 63443, QualityMode.High);
		Terrace cliffthings = new Terrace(cliffthingsbase);
		cliffthings.Add(-1);
		cliffthings.Add(-.875);
		cliffthings.Add(-.75);
		cliffthings.Add(-.5);
		cliffthings.Add(0);
		cliffthings.Add(1);


		ModuleBase finalcliff = new ScaleBias(50, 50, cliffthings);
		ModuleBase innerControl = new Perlin(.005, 2, .4, 3, 2356, QualityMode.High);
		ModuleBase outerControl = new Perlin(.001, 2, .4, 3, 2356, QualityMode.High);
		Select cliffSelector = addModule(finalcliff, plains, innerControl, .5, .1);
		Select cliffSelectorouter = addModule(cliffSelector, plains, outerControl, .2, .3);

		finalTexture = new Const(substanceNoise.Count - 1);
		finalTerrain = cliffSelectorouter;
		//finalTerrain = hpselector;
		
		//finalTerrain = new Const(0, substanceNoise.Count - 1);
		
	}

	//adds a module on top of another (creates a selector) 
	//adds module addedMod to module baseMod based on control in a certain amount, amount ranges from 0(add none) to 1 (completely cover)
	private Select addModule(ModuleBase addedMod, ModuleBase baseMod, ModuleBase control, double amount, double falloff)
	{
		Select newMod = new Select(baseMod, addedMod, control);
		newMod.Minimum = -5;
		newMod.Maximum = amount * 2 - 1;//puts it in the range[-1,1]
		newMod.FallOff = falloff;

		return newMod;
	}

	//OVERLOADED!!!! Yes I know, my comments are very helpful
	//these two are used for composing texture modules
	private Select addModule(Sub addedSub, Sub baseSub, ModuleBase control, double amount)
	{
		return addModule(new Const(addedSub), new Const(baseSub), control, amount, 0);
	}

	private Select addModule(Sub addedSub, ModuleBase baseText, ModuleBase control, double amount)
	{
		return addModule(new Const(addedSub), baseText, control, amount, 0);
	}

	/*private void addContinents()
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

	}*/

	//returns the voxel val(float value used to build the mesh with marching cubes) and type(substance) at a specific voxel
	public void getVoxData(Vector3 pos, out float val, out Sub sub)
	{
		//the distance from the center of the planet to the current voxel
		float distxyz = Vector3.Distance(Vector3.zero, pos);

		Vector3 surfPos = pos.normalized*radius;// get the spot at sea level of the planet(this is to prevent weird overhangs and stuff

		//the substance noise to use at this voxel
		//int tid;//the id of the texture module to use from substanceNoise
		//sub = Sub.ICE;
		float noise = (float)finalTerrain.GetValue(surfPos.x, surfPos.y, surfPos.z);
		//Debug.Log(tid);
		int tid = (int)finalTexture.GetValue(surfPos.x, surfPos.y, surfPos.z);
		sub = (Sub)substanceNoise[tid].GetValue(pos.x, pos.y, pos.z);
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