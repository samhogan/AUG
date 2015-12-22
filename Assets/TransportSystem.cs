using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransportSystem
{
	private int globalTUWith = 4;//how many large transport units are on one(of 6) side of the planet
	private int largeTUWith = 8;//how many mid transport units are on one side of a large street
	private int midTUWith = 8;//how many base transport units are on one side of a mid street unit

	public int sideLength;//how many base units are on one side of a global transport unit


	//stores all base t units that have been generated
	private Dictionary<SurfaceUnit, TransportUnit> baseTUs = new Dictionary<SurfaceUnit, TransportUnit>();


	//the mid t units that have been loaded, used to assure that empty base units do not need to be loaded
	//NOTE: transport units and surface units are the same thing but on different scales(usually)
	//private List<SurfaceUnit> loadedMids = new List<SurfaceUnit>();

	//the large t units that have been loaded, used to assure that empty mid units do not need to be loaded
	//private List<SurfaceUnit> loadedLgs = new List<SurfaceUnit>();

	public TransportSystem(int gtu, int ltu, int mtu)
	{
		globalTUWith = gtu;
		largeTUWith = ltu;
		midTUWith = mtu;

		sideLength = gtu * ltu * mtu;
	}

	//will return a TransportUnit object from the requested a base unit
	public TransportUnit getBase(PSide side, int upos, int vpos)
	{
		/*pseudo code
		 *if base unit exists in baseTUs, return it
		 *else generate it or discover it will never exist
		 *
        */

		TransportUnit tu = new TransportUnit();
		tu.conPoint = new Vector2(upos + 0.5f, vpos + 0.5f);
		tu.conUp = true;
		tu.conRight = true;

		//set this unit's con point world position 
		//radius will later take things like elevation into account using a terrainsystem method
		tu.conPointWorld = UnitConverter.getWP(new SurfacePos(side, tu.conPoint.x, tu.conPoint.y), 
		                                       WorldManager.curPlanet.radius, sideLength);

		return tu;
	}

}
