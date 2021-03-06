﻿using UnityEngine;
using System.Collections;

//This will attach to an empty gameobject in the game world and initializes and holds importing stuff about the world
public class WorldManager : MonoBehaviour 
{

	public UniverseSystem universe;

	//the hash function
	//NOTE: implement defined 3 value function
	//public static XXHash hash;

	// Use this for initialization
	public int seed;

	void Awake() 
	{

		//universal terrain stuff
		MarchingCubes.SetWindingOrder (2, 1, 0);//the the visible side of the polygons 
		//MarchingCubes.SetWindingOrder (0,1,2);//the the visible side of the polygons 
		MarchingCubes.SetTarget(1);//1 is the voxel surface value


		RandomHandler.hash = new XXHash(1);//using seed 1 for testing(will later be randomly chosen

		seed = Random.Range(int.MinValue, int.MaxValue);
		//initialize THE UNIVERSE!!!!!!!!!
		universe = new UniverseSystem(seed);
	}

	void Start()
	{
		GameUI.ui.setSeedText(seed);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
