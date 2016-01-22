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
	private float atmosRadius;//the radius of the atmosphere/direct gravitational influence(distance from planet to make it curplanet)
	public float scaledAtmosRadius;//atmosRadius in unispace

	//the position of the planet in unispace (measured in 10000s or whatever the scale is)
	public UniPos scaledPos;


	//the large scale representation of the planet in unispace
	public GameObject scaledRep;

	//later will have many parameters
	public Planet(float r, UniPos pos)//Vector3 sp, float r)
	{
		radius = r;
		scaledRadius = r/Unitracker.uniscale;

		atmosRadius = r+5000;//atmosphere is 5 km above surface
		scaledAtmosRadius = atmosRadius/Unitracker.uniscale;

		terrain = new TerrainSystem(this, radius);
		//surface = new SurfaceSystem(radius, 400);
		surface = new SurfaceSystem(this, radius, (int)(radius/50));//number of surface units per side is radius/50

		noise = new NoiseHandler(radius);

		createRep(pos);
	}

	//instantiates the unispace rep of the planet
	void createRep(UniPos pos)
	{
		scaledRep =  new GameObject("Planet 8493yuhbgo86");


		//the gameobject that holds the scaledRep's mesh data so it can be scaled
		GameObject meshobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		meshobj.transform.SetParent(scaledRep.transform);
		GameObject.Destroy(meshobj.GetComponent<SphereCollider>());//remove this pesky component


		scaledRep.layer = 8;//add to Unispace layer
		meshobj.layer = 8;

		//arbitrary unipos for testing
		scaledPos = pos;
		scaledRep.transform.position = Unitracker.UniToAbs(scaledPos);
		scaledRep.transform.rotation = Quaternion.Euler(0,45,0);

		meshobj.transform.localScale = new Vector3(scaledRadius*2, scaledRadius*2, scaledRadius*2);



		//add some fun color
		meshobj.GetComponent<MeshRenderer>().material = Resources.Load("TestMaterial") as Material;//loads the default material, will remove this






	}

}
