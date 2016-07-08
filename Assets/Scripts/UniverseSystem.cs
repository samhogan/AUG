using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UniverseSystem
{

	//list of planets
	//public static List<Planet> planets = new List<Planet>();

	public static List<CelestialBody> bodies = new List<CelestialBody>();
	//the planet that things will be built for
	public static Planet curPlanet;

	//is the player in space or on a planet?
	//things are generated slightly different in space
//	public static bool inSpace;





	public UniverseSystem()
	{
	//	inSpace = true;
		populate();

	}

	//populates the universe with planets and such
	public void populate()
	{
		//Planet earth = new Planet(10000);
		//Planet earth = new Planet(200);

	/*	Planet earth = new Planet(250000, new UniPos(new Vector3(0, 0, 0), 9010, 009, 3109), 456);//Random.Range(int.MinValue, int.MaxValue));
		//planets.Add(earth);
		bodies.Add(earth);
		//curPlanet = earth;

		Planet planetfun = new Planet(1000000, new UniPos(new Vector3(0,0,0), 2000, 200, 50), 987165);
		bodies.Add(planetfun);
		//planets.Add(planetfun);

		//Planet planetnomy = new Planet(100000, new UniPos(new Vector3(0,0,0), 1000, 5000, 5000), 34532);
		//planets.Add(planetnomy);
*/

		for(int i = 0; i < 12; i++)
		{
			Planet planet = new Planet(PlanetBuilder.eDist(100000,1000000, Random.value), 
				new UniPos(new Vector3(0,0,0), Random.Range(-30000, 30000), Random.Range(-20, 20), Random.Range(-30000, 30000)), 
				Random.Range(int.MinValue, int.MaxValue));
			bodies.Add(planet);
		}

		Star sun = new Star(3452, 23000000, new UniPos(new Vector3(0,0,0), 0, 0, 0));
		bodies.Add(sun);



		/*for(int i = 0; i<20; i++)
		{
			Planet planetfun = new Planet(Random.Range(10000,1000000), new UniPos(new Vector3(0,0,0), 200*i, 00, 200));
			planets.Add(planetfun);
		}*/

		//Planet planetfun = new Planet(6000000, new UniPos(new Vector3(0,0,0), 1000, 200, 50));
		//planets.Add(planetfun);
	}

}
