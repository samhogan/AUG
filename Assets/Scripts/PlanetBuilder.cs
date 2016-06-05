using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.Collections.Generic;

//contains parameters and generator for creating the noise modules that generate planetary terrain
public class PlanetBuilder 
{


	//height noise probability (perlin, billow, ridged)
	private static ProbItems hnProb = new ProbItems(new double[]{3,1,1});


	//should probably overload buildFeature, but this improves readability i think
	public static void genPlanetData(out ModuleBase finalTerrain, out ModuleBase finalTexture)
	{
		float maxNoiseScale;//pretty much useless info for the final terrain
		buildFeature(out finalTerrain, out finalTexture, out maxNoiseScale, 1);
	}



	//a feature is either some noise with a texture or a composition of two features
	//the final terrain of a planet is a very complex feature made up of many features
	//this is funny: a feature is a composition of features; recursive logic and the function is recursive!
	//noiseScale is the max scale of noise from the inner iterations to prevent large mountains from being selected on small scales
	public static void buildFeature(out ModuleBase terrain, out ModuleBase texture, out float noiseScale, int lev)
	{
		Debug.Log("level " + lev);
		if(Random.value < 1.0/lev && lev<4)
		{

			ModuleBase terrain1, terrain2, texture1, texture2;
			float nScale1, nScale2;

			buildFeature(out terrain1, out texture1, out nScale1, lev+1);
			buildFeature(out terrain2, out texture2, out nScale2, lev+1);

			noiseScale = Mathf.Max(nScale1, nScale2);

			//TODO: some probability that edist lower bound is lower than noisescale
			double controlScale = eDist(Mathf.Max(noiseScale, 100), 1000000);
			//the base control for the selector that adds two new features
			ModuleBase baseControl = getGradientNoise(hnProb, Random.value, controlScale);
			//baseControl = new Scale(50, 1, 1, baseControl);
			//create a cache module because this value will be calculated twice (once for terrain and once for texture)(possibly)
			baseControl = new Cache(baseControl);
			//make possible edge controller
			//loop and make inner controllers

			//the amount to add of this feature to the biome(0 is add none, 1 is completely cover)
			//NOTE: later amount will be somewhat dependant on the feature number(feature #6 will have an average lower amount than feature #2)
			double amount = .5;//Random.value;
			double falloff = Random.value;


			terrain = addModule(terrain1, terrain2, baseControl, amount, falloff);
			texture = addModule(texture1, texture2, baseControl, amount, 0);
			
		}
		else
		{
			//scale is the inverse of the frequency and is used to influence amplitude
			float scale = eDist(1, 15000);
			//scale = 100;
			//the starting noise for the final feature that will be modified

			terrain = getGradientNoise(hnProb, Random.value, scale);

			//the amplidude or max height of the terrain
			//NOTE: later will be related to the frequency
			double amplitude = eDist(.5,scale/4);//randDoub(2, 100);
			//bias is the number added to the noise before multiplying
			//-1 makes canyons/indentions, 1 makes all feautures above sea level
			//NOTE: later make a greater chance to be 1 or -1
			double bias = 1;//randDoub(-1, 1);

			//terrain = new Displace(terrain, getGradientNoise(hnProb, Random.value, 100), getGradientNoise(hnProb, Random.value, 100), getGradientNoise(hnProb, Random.value, 100));
			//terrain = new Scale(50,1,1,terrain);
			/*Curve c = new Curve(terrain);
			c.Add(.5, 1);
			terrain = c;*/

			//terrain = new Invert(terrain);
			/*Terrace cliffthings = new Terrace(terrain);
			cliffthings.Add(-1);
			cliffthings.Add(-.875);
			cliffthings.Add(-.75);
			cliffthings.Add(-.5);
			cliffthings.Add(0);
			cliffthings.Add(1);
			terrain = cliffthings;*/

			terrain = new ScaleBias(amplitude, bias * amplitude, terrain);
			texture = new Const(Random.Range(0,14));
			noiseScale = scale;
		}


	}

	//get a random gradient noise function(perlin, billow, ridged, maybe voronoi later)
	//TODO: paramatize all other properties
	private static ModuleBase getGradientNoise(ProbItems prob, double val, double scale)
	{
		switch(1)//(int)prob.getValue(Random.value))
		{
		case 0: 
			return new Perlin(1/scale,//randDoub(.00001, 0.1), 
				randDoub(1.8, 2.2), 
				randDoub(.4, .6), 
				Random.Range(2, 6), 
				Random.Range(int.MinValue, int.MaxValue), 
				QualityMode.High);
			break;
		case 1:
			return new Billow(1/scale,
				randDoub(1.8, 2.2), 
				randDoub(.4, .6), 
				Random.Range(2, 6), 
				Random.Range(int.MinValue, int.MaxValue), 
				QualityMode.High);
			break;
		case 2:
			return new RidgedMultifractal(1/scale,
				randDoub(1.8, 2.2),
				Random.Range(2, 6), 
				Random.Range(int.MinValue, int.MaxValue), 
				QualityMode.High);
			break;
		default:
			return new Const(0.0);
			break;
		}
	}




	//generates finalTerrain and finalTexture for a planet
	public static void buildTerrain(out ModuleBase finalTerrain, /*out ModuleBase finalTexture,*/ out ModuleBase biomeSelector, out List<ModuleBase> biomeTextures)
	{

		finalTerrain = new Const(0.0);

		//finalTexture = new Const(0.0);
		biomeSelector = new Const(0.0);
		biomeTextures = new List<ModuleBase>();


		//list that contains all substances that have been used
		List<Sub> subList = new List<Sub>();

		//the number of "biomes" or types of terrain that use different texture ids
		int numBiomes = 3;

		//loop through and create all the biomes and compose them
		for(int biome = 1; biome<=numBiomes; biome++)
		{



			//if in first iteration, generate a base layer of rock (although can have any terrain features)
			//generate texture

			//height map and textures for this biome
			ModuleBase biomeTerrain = null;
			ModuleBase biomeTexture = null;

			//the number of terrain features that will be composed(selected)
			int numFeatures = 4;// Random.Range(1,6);

			//loop through and create all the features
			for(int feature = 1; feature <= numFeatures; feature++)
			{
				//scale is the inverse of the frequency and is used to influence amplitude
				double scale = eDist(1, 15000);
				//scale = 100;
				//the starting noise for the final feature that will be modified
				ModuleBase featureTerrain = new Perlin(1/scale,//randDoub(.00001, 0.1), 
					randDoub(1.8, 2.2), 
					randDoub(.4, .6), 
					Random.Range(2, 6), 
					Random.Range(int.MinValue, int.MaxValue), 
					QualityMode.High);
				
				//the amplidude or max height of the terrain
				//NOTE: later will be related to the frequency
				double amplitude = scale/4;//eDist(.5,scale/2);//randDoub(2, 100);
				//bias is the number added to the noise before multiplying
				//-1 makes canyons/indentions, 1 makes all feautures above sea level
				//NOTE: later make a greater chance to be 1 or -1
				double bias = 1;//randDoub(-1, 1);

				featureTerrain = new ScaleBias(amplitude, bias * amplitude, featureTerrain);

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
					biomeTerrain = featureTerrain;
					biomeTexture = new Const(Random.Range(0,14));
				}
				else
				{

					double controlScale = eDist(Mathf.Max((float)scale,100), 20000);
					//the base control for the selector that adds this feature to the biome
					ModuleBase baseControl = new Perlin(1/controlScale, 
						randDoub(1.8, 2.2), 
						randDoub(.4, .6), 
						3,//Random.Range(1, 3), 
						Random.Range(int.MinValue, int.MaxValue), QualityMode.High);

					//make possible edge controller
					//loop and make inner controllers

					//the amount to add of this feature to the biome(0 is add none, 1 is completely cover)
					//NOTE: later amount will be somewhat dependant on the feature number(feature #6 will have an average lower amount than feature #2)
					double amount = 1/feature;//Random.value;
					double falloff = Random.value;
					biomeTerrain = addModule(featureTerrain, biomeTerrain, baseControl, amount, falloff);
				}
			}

			//if it is the first biome, add biome 100% to planet as a base
			if(biome == 1)
			{
				finalTerrain = biomeTerrain;
				//biomeSelector = new Const(0.0);
			}
			else
			{


				double controlScale = 100000;//eDist(100000, 100000);
				ModuleBase baseControl = new Perlin(1/controlScale, 
					randDoub(1.8, 2.2), 
					randDoub(.4, .6), 
					3,//Random.Range(1, 3), 
					Random.Range(int.MinValue, int.MaxValue), QualityMode.High);

				double amount = 1.0/biome;//Random.value;
		
				double falloff = Random.value;
				finalTerrain = addModule(biomeTerrain, finalTerrain, baseControl, amount, 0);//falloff);

				//add the biome number to the biome selector 
				biomeSelector = addModule(new Const(biome-1), biomeSelector, baseControl, amount, 0);
				//biomeTextures.Add(biomeTexture);

			}

			biomeTextures.Add(biomeTexture);


		}

		//Const testTexture = new Const(Random.Range(0,14));
		//biomeTextures.Add(testTexture);
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
		ModuleBase outerControl = new Perlin(.001, 2, .4, 3, 235, QualityMode.High);
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
