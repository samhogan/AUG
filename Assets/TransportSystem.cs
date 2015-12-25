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
	private Dictionary<SurfaceUnit, TUBase> baseTUs = new Dictionary<SurfaceUnit, TUBase>();

	//stores all mid t units that have been generated
	private Dictionary<SurfaceUnit, TransportUnit> midTUs = new Dictionary<SurfaceUnit, TransportUnit>();

	//stores all large t units that have been generated
	private Dictionary<SurfaceUnit, TransportUnit> largeTUs = new Dictionary<SurfaceUnit, TransportUnit>();

	//used to fill mid units with base units(too much code, had to break it off into its own class)
	private TUMidFiller midfill;

	public TransportSystem(int gtu, int ltu, int mtu)
	{
		globalTUWidth = gtu;
		largeTUWidth = ltu;
		midTUWidth = mtu;

		sideLength = gtu * ltu * mtu;
		sideLengthLarge = ltu*mtu;

		midfill = new TUMidFiller(this, mtu);
	}

	//will return a TransportUnit object from the requested a base unit
	//OH YES THIS USES RECURSION OH MAN!!!!!!!!!!!!!!!! I'M SO PROUD OF MYSELF!!!!!
	public TUBase getBase(SurfaceUnit su)
	{
		//the base unit to be returned eventually
		TUBase bu = null;

		//if the base unit exists in the list, return it
		if(baseTUs.TryGetValue(su, out bu))
		{
			return bu;
		}

		//if the base unit is not in the list, check if a mid unit is

		//the coordinates of the mid unit
		SurfaceUnit mus = getMidSU(su);

		//retrieve the actual mid unit (or not if it will never exist)
		TransportUnit mu = getMid(mus);

		//if the mid unit will never exist, the base unit will never exist
		if(mu==null)
			return null;

		//if the mu has already been populated, the base unit will never exist
		if(mu.populated)
			return null;
		else
		{
			//populate it
			/*int startu = mus.u*midTUWidth;
			int startv = mus.v*midTUWidth;
			for(int i = startu; i<startu+midTUWidth; i++)
			{
				for(int j = startv; j<startv+midTUWidth; j++)
				{
					//create a new base unit, set its properties, and add it to the base list
					TUBase newTU = new TUBase();
					newTU.conUp = Random.value>0.5f;
					newTU.conRight = Random.value>0.5f;
					newTU.conPoint = new Vector2(i + Random.value, j + Random.value);
					newTU.conPointWorld = UnitConverter.getWP(new SurfacePos(su.side, newTU.conPoint.x, newTU.conPoint.y), 
					                                       WorldManager.curPlanet.radius, sideLength);

					baseTUs.Add(new SurfaceUnit(su.side, i, j), newTU);
				}
			}*/
			Dictionary<SurfaceUnit, TUBase> bases = midfill.populate(mu, mus);
			foreach(KeyValuePair<SurfaceUnit, TUBase> pair in bases)
			{
				pair.Value.conPointWorld = UnitConverter.getWP(new SurfacePos(su.side, pair.Value.conPoint.x, pair.Value.conPoint.y), 
				                                    WorldManager.curPlanet.radius, sideLength);
				baseTUs.Add(new SurfaceUnit(su.side, pair.Key.u, pair.Key.v), pair.Value);
				//Debug.Log(pair.Key);
			}

			mu.populated = true;

			//use recursion to return to the top of the function and get the base unit from the list(or not if it was not generated)
			return getBase(su);
		}

	}

	//returns the mid unit at the specified su or null if one does not exist
	public TransportUnit getMid(SurfaceUnit su)
	{
		TransportUnit mu = null;

		//if it is in the mid list, return it
		if(midTUs.TryGetValue(su, out mu))
		{
			return mu;
		}
		//if it's not in the mid list, check if a large unit will contain it

		//the coordinates of the proposed large unit
		SurfaceUnit lus = getLargeSU(su);
		//retrieve the large unit
		TransportUnit lu = getLarge(lus);
	
		//if the lu has already been populated, the mu will never exist, so return null
		if(lu.populated)
			return null;
		else
		{
			//populate it with mid units(later will be moved to a separate function)
			int startu = lus.u*largeTUWidth;
			int startv = lus.v*largeTUWidth;
			//loop through all mid units in the large unit and create them
			for(int i = startu; i<startu+largeTUWidth; i++)
			{
				for(int j = startv; j<startv+largeTUWidth; j++)
				{
					//create a new base unit, set its properties, and add it to the base list
					TransportUnit newTU = new TransportUnit();
					newTU.conUp = true;//Random.value>0.5f;
					newTU.conRight = true;//Random.value>0.5f;
					newTU.indexI = i;
					newTU.indexJ = j;
					//newTU.conPoint = new Vector2((i+0.5f)*midTUWidth + Random.value*4-2,(j+0.5f)*midTUWidth + Random.value*4-2);
					newTU.conPoint = new Vector2((i+0.5f)*midTUWidth,(j+0.5f)*midTUWidth);
					//newTU.conPoint = new Vector2((i+Random.value)*midTUWidth,(j+Random.value)*midTUWidth);
					midTUs.Add(new SurfaceUnit(su.side, i, j), newTU);
				}
			}
			lu.populated = true;
			//use recursion to return to the top of the function and return the newly created(or not) mid unit
			return getMid(su);
		}

	}

	//returns the large unit at a specified su, creates one if it does not exist(every large unit will exist)
	public TransportUnit getLarge(SurfaceUnit su)
	{
		TransportUnit lu = null;

		//if it is in the list, return it
		if(largeTUs.TryGetValue(su, out lu))
		{
			return lu;
		}

		//if not, build one and return it
		TransportUnit tu = new TransportUnit();
		largeTUs.Add(su, tu);
		return tu;
	}

	//returns the coordinates of a mid unit that contain the base unit whose coordinates are given
	private SurfaceUnit getMidSU(SurfaceUnit bsu)
	{
		return new SurfaceUnit(bsu.side, 
		                       Mathf.FloorToInt((float)bsu.u/midTUWidth),
		                       Mathf.FloorToInt((float)bsu.v/midTUWidth));
	}

	//returns the coordinates of a large unit that contain the mid unit whose coordinates are given
	private SurfaceUnit getLargeSU(SurfaceUnit bsu)
	{
		return new SurfaceUnit(bsu.side, 
		                       Mathf.FloorToInt((float)bsu.u/largeTUWidth),
		                       Mathf.FloorToInt((float)bsu.v/largeTUWidth));
	}
}
