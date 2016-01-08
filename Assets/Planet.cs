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

	//the position of the planet in unispace (measured in 10000s or whatever the scale is)
	private Vector3 scaledPos;
	//the large scale representation of the planet in unispace
	private GameObject scaledRep;

	//later will have many parameters
	public Planet(float r)//Vector3 sp, float r)
	{
		radius = r;
		terrain = new TerrainSystem(this, radius);
		//surface = new SurfaceSystem(radius, 400);
		surface = new SurfaceSystem(this, radius, (int)(radius/50));//number of surface units per side is radius/50

		noise = new NoiseHandler(radius);


		scaledPos = new Vector3(100,0,100);
		scaledRep = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		scaledRep.layer = 8;//add to Unispace layer
		scaledRep.transform.position = scaledPos;
		
	}

}
