using UnityEngine;
using System.Collections;

//holds all the information for the planet including terrain and civilization
public class Planet
{

	//private RoadSystem roads;//builds the roads/civilization organization
	public TerrainSystem terrain;//builds the terrain with voxels and marching cubes
	public SurfaceSystem surface;//builds the objects that are on the planet surface

	public NoiseHandler noise;//contains all perlin noise info 

	public float radius = 200f;//radius of the planet

	private float genScale = 5f;//the perlin scale for general elevation

	//later will have many parameters
	public Planet(float r)
	{
		radius = r;
		terrain = new TerrainSystem(this, radius);
		//surface = new SurfaceSystem(radius, 400);
		surface = new SurfaceSystem(this, radius, (int)(radius/50));//number of surface units per side is radius/50

		noise = new NoiseHandler(radius);

	}

}
