using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarSystem : AstroObject
{
	//a reference to the star and planets in this solar system
	//TODO: multiple stars, did you know that 1/3 to 1/2 of all star systems are binary/more than 1 stars?
	public Star star;
	public List<Planet> planets = new List<Planet>();

	public StarSystem(LongPos pos)
	{
        scaledPos = pos;
        createRep();
		//generateStuff();
	}

	public void generateStuff()
	{

		System.Random rand = new System.Random(Random.Range(int.MinValue, int.MaxValue));

		for(int i = 0; i < 10; i++)
		{
			Planet planet = new Planet(PlanetBuilder.eDist(100000,1000000, rand.NextDouble()), 
				new LongPos(rand.Next(-30000, 30000)*10000L, rand.Next(-20, 20)*10000L, rand.Next(-30000, 30000)*10000L),
				rand.Next(int.MinValue, int.MaxValue));
			planets.Add(planet);
		}

		star = new Star(3452, 23000000, new LongPos(0,0,0));
	}

    private void createRep()
    {
        scaledRep = new GameObject("Star System");



        //the gameobject that holds the scaledRep's mesh data so it can be scaled
        GameObject meshobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        meshobj.transform.SetParent(scaledRep.transform);
        GameObject.Destroy(meshobj.GetComponent<SphereCollider>());//remove this pesky component
        meshobj.transform.localScale = new Vector3(100,100,100);

        //TODO: move some of these things to a function in celestialbody
        scaledRep.transform.position = CoordinateHandler.stellarSpace.getFloatingPos(scaledPos);

        scaledRep.layer = (int)spaces.Galactic;//add to stellar space
        meshobj.layer = (int)spaces.Galactic;

        meshobj.GetComponent<MeshRenderer>().material = Resources.Load("Star") as Material;
        meshobj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

    }
}
