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

	private float tScale1;//level 1 terrain scale(continents, big noise)
	private float tHeight1;//level 1 terrain height

	private Max finalTerrain;
	private Perlin textureTest;

	private List<ModuleBase> substanceNoise;

	public NoiseHandler(float r)
	{
		radius = r;

		//create some terrain noise
		RidgedMultifractal rmf = new RidgedMultifractal(.0001, 2, 4, 1, QualityMode.High, new Const((int)Sub.ICE));
		Multiply mult = new Multiply(rmf, new Const(2000, new Const(0)));
		finalTerrain = new Max(mult, new Const(0));//, new Perlin(.001, 2, 2, 2, 1, QualityMode.High));



		substanceNoise = new List<ModuleBase>();
		ModuleBase rock = new Select(new Const(2), new Const(4), new Billow(.001, 2, 2, 2, 1, QualityMode.High));
		ModuleBase rinoise = new Select(new Const(1), rock, new Perlin(.00001, 2, 2, 2, 1, QualityMode.High));
		ModuleBase veg = new Select(new Const(3), rinoise, new Perlin(.000006, 2, 2, 2, 4352, QualityMode.High));
		substanceNoise.Add(veg);
	}

	//returns the voxel val(float value used to build the mesh with marching cubes) and type(substance) at a specific voxel
	public void getVoxData(Vector3 pos, out float val, out Sub sub)
	{
		//the distance from the center of the planet to the current voxel
		float distxyz = Vector3.Distance(Vector3.zero, pos);

		Vector3 surfPos = pos.normalized*radius;// get the spot at sea level of the planet(this is to prevent weird overhangs and stuff

		//the substance noise to use at this voxel
		
		//sub = Sub.ICE;
		float noise = (float)finalTerrain.GetValue(surfPos.x, surfPos.y, surfPos.z);

		sub = (Sub)substanceNoise[0].GetValue(surfPos.x, surfPos.y, surfPos.z);
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
