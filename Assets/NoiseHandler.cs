using UnityEngine;
using System.Collections;

//one instance per planet, controls simplex noise based things such as altitude, population density, etc.
public class NoiseHandler
{
	private float radius;//the radius of the planet

	private float tScale1;//level 1 terrain scale(continents, big noise)
	private float tHeight1;//level 1 terrain height


	public NoiseHandler(float r)
	{
		radius = r;
		tScale1 = 250000;
		//tScale1 = 1030;
		tHeight1 = 20000;
		//tHeight1 = 200;
	}

	//returns the altitude of the terrain at a given position
	public float getAltitude(Vector3 pos)
	{
		pos = pos.normalized*radius;// get the spot at sea level of the planet(this is to prevent weird overhangs and stuff


		float noise = Noise.GetNoise(pos.x/tScale1,pos.y/tScale1,pos.z/tScale1);
		noise-=0.5f;//adjust so the range is from -0.5 to +0.5 so terrain extends above and below sea level
		noise*=tHeight1;//5 is height, extends range from -2.5 to 2.5 for larger hills and stuff
		return radius + noise;
	}

	//returns the altitude adjusted position given a position on the surface  
	public Vector3 altitudePos(Vector3 pos)
	{
		return pos*(getAltitude(pos)/radius);
	}

}
