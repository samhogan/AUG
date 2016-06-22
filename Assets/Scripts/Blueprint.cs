using UnityEngine;
using System.Collections;

public abstract class Blueprint 
{
	//creates a random object from the blueprint based on a random generator(or later seed) and other information
	//public abstract WorldObject buildObject(System.Random rand);
	public abstract Mesh buildObject(System.Random rand);
}
