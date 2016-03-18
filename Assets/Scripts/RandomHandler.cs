using UnityEngine;
using System.Collections;
//using System;
//handles the random number systems for a planet(or all planets, haven't decided yet)
public class RandomHandler 
{

	public static XXHash hash;

	public RandomHandler()
	{
		//hash = new XXHash(1);
	}

	//returns an rng for each surface unit
	public static System.Random surfaceRandom(SurfaceUnit su)
	{
		//system.random takes a seed that is calculated from the hash function
		//1 is the planet num, will eventually need a way to calculate this
		return new System.Random((int)hash.GetHash(su.u, su.v, (int)su.side, 1));
	}

	//returns an rng for each transport unit
	public static System.Random transportRandom(SurfaceUnit su, TLev lev)
	{
		System.Random rand = new System.Random((int)hash.GetHash(su.u, su.v, (int)su.side, (int)lev, 1));

		return rand;
	}


}
