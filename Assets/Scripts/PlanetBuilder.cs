using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.Collections.Generic;

//contains parameters and generator for creating the noise modules that generate planetary terrain
public class PlanetBuilder 
{


	//generates finalTerrain and finalTexture for a planet
	public static void buildTerrain(out ModuleBase finalTerrain, out ModuleBase finalTexture, out List<ModuleBase> substanceNoise)
	{

		finalTerrain = new Const(0.0);
		substanceNoise = new List<ModuleBase>();


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
			int numFeatures = 8;// Random.Range(1,6);
			//loop through and create all the features
			for(int feature = 1; feature <= numFeatures; feature++)
			{
				//scale is the inverse of the frequency and is used to influence amplitude
				double scale = eDist(5, 10000);
				//scale = 100;
				//the starting noise for the final feature that will be modified
				ModuleBase finalFeature = new Perlin(1/scale,//randDoub(.00001, 0.1), 
					randDoub(1.8, 2.2), 
					randDoub(.4, .6), 
					Random.Range(2, 6), 
					Random.Range(int.MinValue, int.MaxValue), 
					QualityMode.High);

				//the amplidude or max height of the terrain
				//NOTE: later will be related to the frequency
				double amplitude = eDist(.5,scale/2);//randDoub(2, 100);
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

					double controlScale = eDist(100, 10000);
					//the base control for the selector that adds this feature to the biome
					ModuleBase baseControl = new Perlin(1/controlScale, 
						randDoub(1.8, 2.2), 
						randDoub(.4, .6), 
						Random.Range(1, 3), 
						Random.Range(int.MinValue, int.MaxValue), QualityMode.High);

					//make possible edge controller
					//loop and make inner controllers

					//the amount to add of this feature to the biome(0 is add none, 1 is completely cover)
					//NOTE: later amount will be somewhat dependant on the feature number(feature #6 will have an average lower amount than feature #2)
					double amount = 1/feature;//Random.value;
					double falloff = Random.value;
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
		//finalTerrain = new Const(0.0);
	}

	//adds a module on top of another (creates a selector) 
	//adds module addedMod to module baseMod based on control in a certain amount, amount ranges from 0(add none) to 1 (completely cover)
	private static Select addModule(ModuleBase addedMod, ModuleBase baseMod, ModuleBase control, double amount, double falloff)
	{
		Select newMod = new Select(baseMod, addedMod, control);
		newMod.Minimum = -5;
		newMod.Maximum = amount * 2 - 1;//puts it in the range[-1,1]
		newMod.FallOff = falloff;

		return newMod;
	}

	//OVERLOADED!!!! Yes I know, my comments are very helpful
	//these two are used for composing texture modules
	private static Select addModule(Sub addedSub, Sub baseSub, ModuleBase control, double amount)
	{
		return addModule(new Const(addedSub), new Const(baseSub), control, amount, 0);
	}

	private static Select addModule(Sub addedSub, ModuleBase baseText, ModuleBase control, double amount)
	{
		return addModule(new Const(addedSub), baseText, control, amount, 0);
	}

	//returns a random float between the two values in an exponential distribution
	//(could use log base anything but ln is available so why not)
	private static float eDist(double min, double max)
	{
		double emin = Mathf.Log((float)min);
		double emax = Mathf.Log((float)max);

		return Mathf.Exp((float)randDoub(emin, emax));
	}

	//returns a random double between the two values
	private static double randDoub(double min, double max)
	{
		return Random.value*(max-min)+min;
	}



	//a test preset that creates a mars like planet used to figure out how to build this planet generator
	public static void marsPreset(out ModuleBase finalTerrain, out ModuleBase finalTexture, out List<ModuleBase> substanceNoise)
	{

		substanceNoise = new List<ModuleBase>();

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

	public static void nonePreset(out ModuleBase finalTerrain, out ModuleBase finalTexture, out List<ModuleBase> substanceNoise)
	{

		substanceNoise = new List<ModuleBase>();
		substanceNoise.Add(new Const(Sub.DIRT));

		finalTexture = new Const(0.0);

		finalTerrain = new Const(0.0);


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
}
