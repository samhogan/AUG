using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;


//one instance per planet, controls simplex noise based things such as altitude, population density, etc.
public class NoiseHandler
{
	private float radius;//the radius of the planet

	private float tScale1;//level 1 terrain scale(continents, big noise)
	private float tHeight1;//level 1 terrain height

	private Max finalTerrain;

	public NoiseHandler(float r)
	{
		radius = r;
		tScale1 = 250000;
		tScale1 = 1030;
		tHeight1 = 20000;
		tHeight1 = 200;
		RidgedMultifractal rmf = new RidgedMultifractal(.001, 2, 2, 1, QualityMode.High);
		Multiply mult = new Multiply(rmf, new Const(200));
		finalTerrain = new Max(mult, new Const(0));//, new Perlin(.001, 2, 2, 2, 1, QualityMode.High));
	}

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
	public Substance getSubstace(Vector3 pos)
	{
		return Substance.subs[Sub.TEST];
	}

}
