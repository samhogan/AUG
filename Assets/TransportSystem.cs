using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransportSystem
{
	private int globalTUWidth = 4;//how many large transport units are on one(of 6) side of the planet
	private int largeTUWidth = 8;//how many mid transport units are on one side of a large street
	private int midTUWidth = 8;//how many base transport units are on one side of a mid street unit

	public int sideLength;//how many base units are on one side of a global transport unit
	//public int sideLengthMid;//how many mid units are on one side of a global transport unit

	//NOTE: transport units and surface units are the same thing but on different scales(usually)
	//stores all base t units that have been generated
	private Dictionary<SurfaceUnit, TransportUnit> baseTUs = new Dictionary<SurfaceUnit, TransportUnit>();

	//stores all mid t units that have been generated
	private Dictionary<SurfaceUnit, TransportUnit> midTUs = new Dictionary<SurfaceUnit, TransportUnit>();

	//stores all large t units that have been generated
	private Dictionary<SurfaceUnit, TransportUnit> largeTUs = new Dictionary<SurfaceUnit, TransportUnit>();



	public TransportSystem(int gtu, int ltu, int mtu)
	{
		globalTUWidth = gtu;
		largeTUWidth = ltu;
		midTUWidth = mtu;

		sideLength = gtu * ltu * mtu;
		//sideLengthMid = gtu*ltu;
	}

	//will return a TransportUnit object from the requested a base unit
	public TransportUnit getBase(PSide side, int upos, int vpos)
	{
		/*pseudo code
		 *if base unit exists in baseTUs, return it
		 *else generate it or discover it will never exist
		 *
        */
		//the base unit to be returned eventually
		TransportUnit bu = null;

		//if the base unit exists in the list, return it
		if(baseTUs.TryGetValue(new SurfaceUnit(side, upos, vpos), out bu))
		{
			return bu;
		}

		//if the base unit is not in the list, check if a mid unit is 
		TransportUnit mu = null;
		if(baseTUs.TryGetValue(new SurfaceUnit(side, upos, vpos), out mu))
		{
			return null;
		}
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

	//returns the coordinates of a mid unit that contain the base unit whose coordinates are given
	private SurfaceUnit getMid(SurfaceUnit bsu)
	{
		return new SurfaceUnit(bsu.side, 
		                       Mathf.FloorToInt((float)bsu.u/midTUWidth),
		                       Mathf.FloorToInt((float)bsu.v/midTUWidth));
	}

}
