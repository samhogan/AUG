using UnityEngine;
using System.Collections;

//holds all the information for the planet including terrain and civilization
public class Planet
{

	//private RoadSystem roads;//builds the roads/civilization organization
	public TerrainSystem terrain;//builds the terrain with voxels and marching cubes
	public SurfaceSystem surface;//builds the objects that are on the planet surface

	public NoiseHandler noise;//contains all perlin noise info 

	public float radius;//radius of the planet
	private float scaledRadius;//the radius of the scaledRep in unispace

	//the position of the planet in unispace (measured in 10000s or whatever the scale is)
	public UniPos scaledPos;


	//the large scale representation of the planet in unispace
	public GameObject scaledRep;

	//later will have many parameters
	public Planet(float r)//Vector3 sp, float r)
	{
		radius = r;
		scaledRadius = r/Unitracker.uniscale;

		terrain = new TerrainSystem(this, radius);
		//surface = new SurfaceSystem(radius, 400);
		surface = new SurfaceSystem(this, radius, (int)(radius/50));//number of surface units per side is radius/50

		noise = new NoiseHandler(radius);

		createRep();
	}

	//instantiates the unispace rep of the planet
	void createRep()
	{
		scaledRep = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		GameObject.Destroy(scaledRep.GetComponent<SphereCollider>());//remove this pesky component
		scaledRep.layer = 8;//add to Unispace layer

		//arbitrary unipos for testing
		scaledPos = new UniPos(new Vector3(0,0,0), 100, 0, 100);
		scaledRep.transform.position = Unitracker.UniToAbs(scaledPos);
		scaledRep.transform.localScale = new Vector3(scaledRadius*2, scaledRadius*2, scaledRadius*2);
	}

}
