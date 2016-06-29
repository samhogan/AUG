using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Blueprint 
{
	//creates a random object from the blueprint based on a random generator(or later seed) and other information
	//public abstract WorldObject buildObject(System.Random rand);
	public abstract Mesh buildObject(int seed);

	public abstract int getAmount(List<Sub> subs, WorldPos pos, double random);

	//creates a random blueprint 
	//public static abstract Blueprint buildBlueprint(int seed);
}
