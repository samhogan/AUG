using UnityEngine;
using System.Collections;

//holds all the information for the planet including terrain and civilization
public class Planet
{

	//private RoadSystem roads;//builds the roads/civilization organization
	public TerrainSystem terrain;//builds the terrain with voxels and marching cubes
	public SurfaceSystem surface;//builds the objects that are on the planet surface

	private float radius = 200f;//radius of the planet
	//private int globalSUWith = 4;//how many large street units are on one(of 6) side of the planet
	//private int largeSUWith = 8;//how many mid street units are on one side of a large street
	//private int midSUWith = 8;//how many base street units are on one side of a mid street unit

	private float genScale = 5f;//the perlin scale for general elevation

	//later will have many parameters
	public Planet(float r)
	{
		radius = r;
		terrain = new TerrainSystem (radius);
		surface = new SurfaceSystem (radius, 8);
	}

}
