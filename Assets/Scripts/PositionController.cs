using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionController : MonoBehaviour 
{

	//the physical trackers in unity space that contain cameras for redering the different spaces
	public GameObject player, stellarTracker, galacticTracker, universalTracker;
	public GameObject playerCam, stellarCam, galacticCam, universalCam;

	//the number of scaled units per unity unit
	//keeps celestial objects at precise positions and within a managable unity space
	public const int SUperUU = 10000;

	//the side length of a scaled unit in meters
	public const float planetarySU = 0.0001f;//100 micrometers, 1m per 1uu
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
	const long floatThreshold = 10000L*SUperUU;
	const long floatThresholddouble = 2*floatThreshold;//the full distance of the acceptable play area without shifting, what is rounded to to get the new floating origin

	public static Planet curPlanet;
	StarSystem curSystem;
	//Galaxy curGalaxy;


	// Use this for initialization
	void Awake() 
	{
		setUp();
	}

	void setUp()
	{
		StarSystem test = new StarSystem();
		curSystem = test;
		curPlanet = test.planets[Random.Range(0, test.planets.Count)];
		UniverseSystem.curPlanet = curPlanet;


		Vector3 startPoint = Random.onUnitSphere;
		//startPoint = new Vector3(0, UniverseSystem.curPlanet.noise.getAltitude(startPoint)+20,0);
		startPoint *=curPlanet.noise.getAltitude(startPoint)+3;
	
	
		//planetPos = new LongPos((long)(startPoint.x*SUperUU), (long)(startPoint.y*SUperUU), (long)(startPoint.x*SUperUU));
		//print(planetPos.y);
		//calculate the floating origin by rounding to the nearest threshold
		planetFloatOrigin = calcOrigin(planetPos);

		//calculate the player floating position 
		player.transform.position = getPlanetFloatingPos(startPoint);
		updatePlanetTracker();
		updateTrackerPos();

		player.GetComponent<GravityController>().gravity = curPlanet.gravity;


	}


	// Update is called once per frame
	void Update () 
	{
		updatePlanetTracker();

		updateTrackerPos();
		updateTrackerRotation();
	}


	void updatePlanetTracker()
	{
		//set planetPos based on the player position
		planetPos.x = (long)(player.transform.position.x*SUperUU)+planetFloatOrigin.x;
		planetPos.y = (long)(player.transform.position.y*SUperUU)+planetFloatOrigin.y;
		planetPos.z = (long)(player.transform.position.z*SUperUU)+planetFloatOrigin.z;

		print(player.transform.position.y+" "+player.transform.position.y*SUperUU+" "+planetPos.y+" "+planetFloatOrigin.y);
		//Debug.Log("{0} {1}", 
		if(originNeedsUpdate(planetPos, planetFloatOrigin))
		{
			
			//calculate where the origin should now be
			LongPos newOrigin = calcOrigin(planetPos);

			//calcualte how much to shift the worldobjects
			Vector3 shift = ((planetFloatOrigin-newOrigin)/SUperUU).toVector3();
			foreach(KeyValuePair<WorldPos, List<WorldObject>> objectList in RequestSystem.builtObjects)
			{
				foreach(WorldObject wo in objectList.Value)
				{
					wo.transform.position+=shift;
				}
			}

			//move all terrain objects that are in planetary space (if the player is on a planet
			if(curPlanet!=null)
				foreach(KeyValuePair<LODPos, TerrainObject> chunk in UniverseSystem.curPlanet.lod.chunks)
					if(chunk.Key.level<=LODSystem.uniCutoff)
						chunk.Value.gameObject.transform.position+=shift;


			//ship.transform.position += shift;
			//also shift the player(should eventually not have to do this)
			//if(!Ship.playerOn)
			player.transform.position+=shift;

			//now update the origin
			planetFloatOrigin = newOrigin;

		}
	}

	//updates the positions of all trackers
	void updateTrackerPos()
	{
		
		updateStellarTracker();

	}

	void updateStellarTracker()
	{
		//calculate the stellar position based on the player position
		stellarPos = curPlanet.scaledPos + planetPos / (int)(stellarSU / planetarySU);//this last term should==10000
		//if the stellar tracker/stellar pos is outside the threshold of the origin, recalculate the origin and shift everything back
		if(originNeedsUpdate(stellarPos, stellarFloatOrigin))
		{
			stellarFloatOrigin = calcOrigin(stellarPos);

			//shift everything in stellar space
			foreach(Planet plan in curSystem.planets)
				plan.scaledRep.transform.position = getStellarFloatingPos(plan.scaledPos);
			curSystem.star.scaledRep.transform.position = getStellarFloatingPos(curSystem.star.scaledPos);
		}
		//move the stellar tracker to its appropriate position
		stellarTracker.transform.position = getStellarFloatingPos(stellarPos);
	}

	bool originNeedsUpdate(LongPos pos, LongPos origin)
	{
		print(System.Math.Abs(pos.y-origin.y) + " " + floatThreshold);
		return System.Math.Abs(pos.x-origin.x) > floatThreshold || System.Math.Abs(pos.y-origin.y) > floatThreshold || System.Math.Abs(pos.z-origin.z) > floatThreshold;
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


	public static Vector3 SUtoUU(LongPos pos)
	{
		return pos.toVector3()/SUperUU;
	}

	//get the real position of the player
	public static Vector3 getPlayerPos()
	{
		return SUtoUU(planetPos);
	}

	/*public static Vector3 getRealPos(Vector3 floatPos)
	{
		return SUtoUU(planetPos);
	}*/

	//returns the chunk that the player is in for terrain generation
	public static WorldPos getChunk()
	{
		Vector3 uSpace = getPlayerPos();
		return new WorldPos(Mathf.FloorToInt(uSpace.x/Generator.chunkSize)*Generator.chunkSize,
			Mathf.FloorToInt(uSpace.y/Generator.chunkSize)*Generator.chunkSize, 
			Mathf.FloorToInt(uSpace.z/Generator.chunkSize)*Generator.chunkSize);
	}


	//calculates the floating origin of a space given the current position of the player/tracker in that space
	//it just rounds to the nearest 1000/whatever threshold is
	LongPos calcOrigin(LongPos pos)
	{
		
		return new LongPos(roundToNearest(pos.x, floatThreshold),
			roundToNearest(pos.y, floatThreshold),
			roundToNearest(pos.z, floatThreshold));
		
	}

	//rounds number "num" to the nearest "nearest"
	long roundToNearest(long num, long nearest)
	{
		bool negative = false;
		if(num < 0)
		{
			num *= -1;
			negative = true;
		}

		//first round down
		long rounded = num/nearest*nearest;

		//if the remaining portion is greater than half of nearest, round up
		if(num%nearest >= nearest/2)
			rounded += nearest;

		//if it was originally negative
		if(negative)
			rounded *= -1;

		return rounded;

	}


	void updateTrackerRotation()
	{
		stellarCam.transform.rotation = playerCam.transform.rotation;
	}


}


public enum spaces{Planetary = 9, Stellar = 10, Galactic=11, Universal = 12};
