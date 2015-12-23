using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransportSystem
{
	private int globalTUWidth = 4;//how many large transport units are on one(of 6) side of the planet
	private int largeTUWidth = 8;//how many mid transport units are on one side of a large street
	private int midTUWidth = 8;//how many base transport units are on one side of a mid street unit

	public int sideLength;//how many base units are on one side of a global transport unit
	public int sideLengthLarge;//how many base units are on one side of a large transport unit(for conversions)

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
		sideLengthLarge = ltu*mtu;
	}

	//will return a TransportUnit object from the requested a base unit
	//OH YES THIS USES RECURSION OH MAN!!!!!!!!!!!!!!!! I'M SO PROUD OF MYSELF!!!!!
	public TransportUnit getBase(SurfaceUnit su)
	{
		/*pseudo code
		 *if base unit exists in baseTUs, return it
		 *else generate it or discover it will never exist
		 *
        */

		//the parameters made into a surface unit of the base unit, will probably just make the paramter a surfaceunit later
		//SurfaceUnit su = new SurfaceUnit(side, upos, vpos);

		//the base unit to be returned eventually
		TransportUnit bu = null;

		//if the base unit exists in the list, return it
		if(baseTUs.TryGetValue(su, out bu))
		{
			return bu;
		}


		//if the base unit is not in the list, check if a mid unit is 
		TransportUnit mu = null;

		//the coordinates of the mid unit
		SurfaceUnit mus = getMid(su);

		if(midTUs.TryGetValue(mus, out mu))
		{
			//if the mu has already been populated, the bu will never exist
			if(mu.populated)
				return null;
			else
			{
				//populate it
				int startu = mus.u*midTUWidth;
				int startv = mus.v*midTUWidth;
				for(int i = startu; i<startu+midTUWidth; i++)
				{
					for(int j = startv; j<startv+midTUWidth; j++)
					{
						//create a new base unit, set its properties, and add it to the base list
						TransportUnit newTU = new TransportUnit();
						newTU.conUp = true;
						newTU.conRight = false;
						newTU.conPoint = new Vector2(i + 0.5f, j + 0.5f);
						newTU.conPointWorld = UnitConverter.getWP(new SurfacePos(su.side, newTU.conPoint.x, newTU.conPoint.y), 
						                                       WorldManager.curPlanet.radius, sideLength);

						baseTUs.Add(new SurfaceUnit(su.side, i, j), newTU);
					}
				}
				mu.populated = true;

				//use recursion to return to the top of the function
				//NOTE: need to optimize later and probably not use recursion :(
				return getBase(su);
			}
		}

		//if the mid unit is not in the list, check if a large unit is
		TransportUnit lu = null;
		//the coordinates of the proposed large unit
		SurfaceUnit lus = getLarge(su);
		if(largeTUs.TryGetValue(lus, out lu))
		{
			//if the lu has already been populated, the mu will never exist, so the bu will never exist
			if(lu.populated)
				return null;
			else
			{
				//populate it with mid units
				int startu = lus.u*largeTUWidth;
				int startv = lus.v*largeTUWidth;
				//loop through all mid units in the large unit and create them
				for(int i = startu; i<startu+largeTUWidth; i++)
				{
					for(int j = startv; j<startv+largeTUWidth; j++)
					{
						//create a new base unit, set its properties, and add it to the base list
						TransportUnit newTU = new TransportUnit();
						newTU.conUp = true;
						newTU.conRight = false;
						newTU.conPoint = new Vector2(i + 0.5f, j + 0.5f);
						midTUs.Add(new SurfaceUnit(su.side, i, j), newTU);
					}
				}
				lu.populated = true;
				//use recursion to return to the top of the function
				return getBase(su);
			}
		}

		//if no large unit exists, build one (every possible large unit will exist)
		largeTUs.Add(lus, new TransportUnit());
		return getBase(su);

		/*TransportUnit tu = new TransportUnit();
		tu.conPoint = new Vector2(su.u + 0.5f, su.v + 0.5f);
		tu.conUp = true;
		tu.conRight = true;

		//set this unit's con point world position 
		//radius will later take things like elevation into account using a terrainsystem method
		tu.conPointWorld = UnitConverter.getWP(new SurfacePos(su.side, tu.conPoint.x, tu.conPoint.y), 
		                                       WorldManager.curPlanet.radius, sideLength);

		return tu;*/
	}

	//returns the coordinates of a mid unit that contain the base unit whose coordinates are given
	private SurfaceUnit getMid(SurfaceUnit bsu)
	{
		return new SurfaceUnit(bsu.side, 
		                       Mathf.FloorToInt((float)bsu.u/midTUWidth),
		                       Mathf.FloorToInt((float)bsu.v/midTUWidth));
	}

	//returns the coordinates of a large unit that contain the base unit whose coordinates are given
	private SurfaceUnit getLarge(SurfaceUnit bsu)
	{
		return new SurfaceUnit(bsu.side, 
		                       Mathf.FloorToInt((float)bsu.u/sideLengthLarge),
		                       Mathf.FloorToInt((float)bsu.v/sideLengthLarge));
	}
}
