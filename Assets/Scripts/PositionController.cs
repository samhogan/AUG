using UnityEngine;
using System.Collections;

public class PositionController : MonoBehaviour 
{

	//the physical trackers in unity space that contain cameras for redering the different spaces
	public GameObject player, stellarTracker, galacticTracker, universalTracker;
	public GameObject playerCam, stellarCam, galacticCam, universalCam;

	//the number of scaled units per unity unit
	//keeps celestial objects at precise positions and within a managable unity space
	const int SUperUU = 10000;

	//the side length of a scaled unit in meters
	const float planetarySU = 0.0001f;//100 micrometers, 1m per 1uu
	public const int stellarSU = 1;//1 meter, 10km per 1uu
	const int galacticSU = 10000;//10km, 100,000km per 1uu
	const int universalSU = 100000000;//100K km, 1B km per 1uu, might increase this later

	//the precise locations of the player/trackers in their respective spaces and floating origins
	static LongPos planetPos, planetFloatOrigin;
	static LongPos stellarPos, stellarFloatOrigin;
	static LongPos galacticPos, galacticFloatOrigin;
	static LongPos universalPos, universalFloatOrigin;

	//the distance from the floating origin in scaled units that a tracker must be to be shifted back
	//==1000 unity units
	const long floatThreshold = 1000L*SUperUU;
	const long floatThresholddouble = 2*floatThreshold;//the full distance of the acceptable play area without shifting, what is rounded to to get the new floating origin

	Planet curPlanet;
	StarSystem curSystem;
	//Galaxy curGalaxy;


	// Use this for initialization
	void Start() 
	{
		setUp();
	}

	void setUp()
	{
		StarSystem test = new StarSystem();
		curPlanet = test.planets[Random.Range(0, test.planets.Count)];
		planetPos = new LongPos(0, (long)(curPlanet.noise.getAltitude(new Vector3(0,1,0))*SUperUU), 0);

		//calculate the floating origin by rounding to the nearest threshold
		planetFloatOrigin = calcOrigin(planetPos);

		//calculate the player floating position 
		player.transform.position = getPlanetFloatingPos(planetPos);


	}

	//updates the positions of all trackers
	void updateTrackerPos()
	{
		
		//calculate the stellar position based on the player position
		stellarPos = curPlanet.scaledPos + planetPos / (int)(stellarSU / planetarySU);//this last term should==10000
		//if the stellar tracker/stellar pos is outside the threshold of the origin, recalculate the origin and shift everything back
		if(System.Math.Abs(stellarPos.x - stellarFloatOrigin.x) > floatThreshold || System.Math.Abs(stellarPos.x - stellarFloatOrigin.x) > floatThreshold || System.Math.Abs(stellarPos.x - stellarFloatOrigin.x) > floatThreshold)
		{
			stellarFloatOrigin = calcOrigin(stellarPos);
			//calculate how much to shift and shift everything in stellar space
			foreach(Planet plan in curSystem.planets)
			{
				plan.scaledRep.transform.position = getStellarFloatingPos(plan.scaledPos);
			}
			curSystem.star.scaledRep.transform.position = getStellarFloatingPos(curSystem.star.scaledPos);
		}
		//move the stellar tracker to its appropriate position
		stellarTracker.transform.position = getStellarFloatingPos(stellarPos);


	}

	//TODO: condense these into 1 method/simplify
	//takes a longpos and returns the floating pos in unity space
	public static Vector3 getStellarFloatingPos(LongPos pos)
	{
		//finds relative position in su, divides by 10000 to convert to uu, converts to v3
		return ((pos - stellarFloatOrigin).toVector3() / SUperUU);
	}

	//takes a longpos and returns the floating pos in unity space
	public static Vector3 getPlanetFloatingPos(LongPos pos)
	{
		//finds relative position in su, divides by 10000 to convert to uu, converts to v3
		return ((pos - planetFloatOrigin).toVector3() / SUperUU);
	}

	//later either won't need this or will use a double precision vector
	public static Vector3 getPlanetFloatingPos(Vector3 pos)
	{
		//finds relative position in su, divides by 10000 to convert to uu, converts to v3
		return pos - (planetFloatOrigin.toVector3() / SUperUU);
	}

	//calculates the floating origin of a space given the current position of the player/tracker in that space
	//it just rounds to the nearest 1000/whatever threshold is
	LongPos calcOrigin(LongPos pos)
	{
		/*new LongPos(System.Math.Round((double)pos.x / floatThreshold) * floatThreshold, 
			System.Math.Round((double)pos.y / floatThreshold) * floatThreshold, 
			System.Math.Round((double)pos.z / floatThreshold) * floatThreshold);*/
		//first round down to the nearest 2000
		long rx = pos.x/floatThresholddouble*floatThresholddouble;
		//if it is closer to the higher value, add the nearest back
		if(pos.x%floatThresholddouble >= floatThreshold)
			rx += floatThresholddouble;

		long ry = pos.y/floatThresholddouble*floatThresholddouble;
		if(pos.y%floatThresholddouble >= floatThreshold)
			ry += floatThresholddouble;

		long rz = pos.z/floatThresholddouble*floatThresholddouble;
		if(pos.z%floatThresholddouble >= floatThreshold)
			rz += floatThresholddouble;

		return new LongPos(rx, ry, rz);
		
	}


	void updateTrackerRotation()
	{
		stellarCam.transform.rotation = playerCam.transform.rotation;
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
