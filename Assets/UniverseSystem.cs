using UnityEngine;
using System.Collections;

public class UniverseSystem
{

	//list of planets

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

		//Planet earth = new Planet(250000);
		//curPlanet = earth;
	}

}
