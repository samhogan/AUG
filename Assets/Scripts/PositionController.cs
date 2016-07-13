using UnityEngine;
using System.Collections;

public class PositionController : MonoBehaviour 
{

	//the physical trackers in unity space that contain cameras for redering the different spaces
	public GameObject player, stellarTracker, galacticTracker, universalTracker;

	//the number of scaled units per unity unit
	//keeps celestial objects at precise positions and within a managable unity space
	int SUperUU = 10000;

	//the side length of a scaled unit in meters
	float planetarySU = 0.0001f;//100 micrometers, 1m per 1uu
	int stellarSU = 1;//1 meter, 10km per 1uu
	int galacticSU = 10000;//10km, 100,000km per 1uu
	int universalSU = 100000000;//100K km, 1B km per 1uu, might increase this later

	//the precise locations of the player/trackers in their respective spaces and floating origins
	LongPos planetPos, planetFloatOrigin;
	LongPos stellarPos, stellarFloatOrigin;
	LongPos galacticPos, galacticFloatOrigin;
	LongPos universalPos, universalFloatOrigin;

	//the distance from the floating origin in scaled units that a tracker must be to be shifted back
	//==1000 unity units
	static long floatThreshold = 1000L*SUperUU;

	Planet curPlanet;
	StarSystem curSystem;
	//Galaxy curGalaxy;


	// Use this for initialization
	void Start() 
	{
	
	}

	void setUp()
	{
		StarSystem test = new StarSystem();
		curPlanet = test.planets[Random.Range(0, test.planets.Count)];
		planetPos = new LongPos(0, curPlanet.noise.getAltitude(new Vector3(0,1,0))*SUperUU, 0);

		//calculate the floating origin by rounding to the nearest threshold
		planetFloatOrigin = new LongPos(System.Math.Round((double)planetPos.x / floatThreshold) * floatThreshold, 
			System.Math.Round((double)planetPos.y / floatThreshold) * floatThreshold, 
			System.Math.Round((double)planetPos.x / floatThreshold) * floatThreshold);

		//calculate the player floating position 
		player.transform.position = new Vector3((planetPos.x -planetFloatOrigin.x) / (float)SUperUU, 
			(planetPos.x - planetFloatOrigin.y) / (float)SUperUU, 
			(planetPos.x - planetFloatOrigin.z) / (float)SUperUU);
		


	}

	//updates the positions of all trackers
	void updateTrackerPos()
	{

		stellarPos = curPlanet.scaledPos + planetPos / (stellarSU / planetarySU);
		if(System.Math.Abs(stellarPos.x - stellarFloatOrigin.x) > floatThreshold)
		{
			//calculate how much to shift and shift everything in stellar space
		}
		stellarTracker.transform.position = ((stellarPos - stellarFloatOrigin) / SUperUU);//to vector3


	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
