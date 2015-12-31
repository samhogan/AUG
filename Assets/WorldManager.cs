using UnityEngine;
using System.Collections;

//This will attach to an empty gameobject in the game world and initializes and holds importing stuff about the world
public class WorldManager : MonoBehaviour 
{
	//the planet that things will be built for
	public static Planet curPlanet;

	//the hash function
	//NOTE: implement defined 3 value function
	public static XXHash hash;

	// Use this for initialization
	void Start () 
	{

		//universal terrain stuff
		MarchingCubes2.SetWindingOrder (2, 1, 0);//the the visible side of the polygons 
		MarchingCubes2.SetTarget(1);//1 is the voxel surface value

		hash = new XXHash(1);//using seed 1 for testing(will later be randomly chosen

		//Planet earth = new Planet(10000);
		Planet earth = new Planet(200);
		curPlanet = earth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
