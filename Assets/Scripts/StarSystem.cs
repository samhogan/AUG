using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarSystem 
{
	//a reference to the star and planets in this solar system
	//TODO: multiple stars, did you know that 1/3 to 1/2 of all star systems are binary/more than 1 stars?
	public Star star;
	public List<Planet> planets = new List<Planet>();

	public StarSystem()
	{
		generateStuff();
	}

	public void generateStuff()
	{

		System.Random rand = new System.Random(Random.Range(int.MinValue, int.MaxValue));

		for(int i = 0; i < 5; i++)
		{
			Planet planet = new Planet(PlanetBuilder.eDist(100000,1000000, rand.NextDouble()), 
				new LongPos(rand.Next(-30000, 30000)*10000L, rand.Next(-20, 20)*10000L, rand.Next(-30000, 30000)*10000L),
				rand.Next(int.MinValue, int.MaxValue));
			planets.Add(planet);
		}

		Star sun = new Star(3452, 23000000, new LongPos(0,0,0));
	}
}
