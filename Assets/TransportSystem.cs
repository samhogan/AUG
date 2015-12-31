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

	public int midperglob;//how many mid units are on one side of a global unit

	private int halfmidperglob;
	private int halfSideLength;
	private int halfgtuw;

	//NOTE: transport units and surface units are the same thing but on different scales(usually)
	//stores all base t units that have been generated
	public Dictionary<SurfaceUnit, TUBase> baseTUs = new Dictionary<SurfaceUnit, TUBase>();

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
		midperglob = gtu*ltu;

		//i don't feel like writing a helpful comment on this one so hopefully i will remember what these are used for.....
		halfmidperglob = midperglob/2;//*0.5f;
		halfSideLength = sideLength/2;
		halfgtuw = globalTUWidth/2;


		midfill = new TUMidFiller(this, mtu);
	}

	//will return a TransportUnit object from the requested a base unit
	//OH YES THIS USES RECURSION OH MAN!!!!!!!!!!!!!!!! I'M SO PROUD OF MYSELF!!!!!
	public TUBase getBase(SurfaceUnit su)
	{

		//quick test check, see getMid\
		if(su.u<-halfSideLength || su.u>=halfSideLength || su.v<-halfSideLength || su.v>=halfSideLength)
			return null;


		//if(su.side==PSide.NONE)
		//	return null;


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
				//Debug.Log(su.side+" " + pair.Key.u + " " + pair.Key.v);
				SurfaceUnit newKey = new SurfaceUnit(su.side, pair.Key.u, pair.Key.v);
				//if the key is not already in the list, add it
				if(!baseTUs.ContainsKey(newKey))
				{
					//Debug.Log(su.side+" " + pair.Key.u + " " + pair.Key.v + " ");
					
					baseTUs.Add(newKey, pair.Value);
				}
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

		//quick check to see if this mid is out of range on the side
		//later it will be transformed into a unit of other side in order to connect sides
		if(su.u<-halfmidperglob || su.u>=halfmidperglob || su.v<-halfmidperglob || su.v>=halfmidperglob)
			return null;

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
		//Debug.Log("proposed " + lus);
		//if the lu has already been populated, the mu will never exist, so return null
		if(lu.populated)
			return null;
		else
		{
			//populateLarge(lu, lus);
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
					newTU.conPoint = new Vector2((i+0.5f)*midTUWidth + Random.value-0.5f,(j+0.5f)*midTUWidth + Random.value-0.5f);
					//newTU.conPoint = new Vector2((i+0.5f)*midTUWidth,(j+0.5f)*midTUWidth);
					//newTU.conPoint = new Vector2((i+Random.value)*midTUWidth,(j+Random.value)*midTUWidth);
					midTUs.Add(new SurfaceUnit(su.side, i, j), newTU);
				}
			}
			lu.populated = true;
			//use recursion to return to the top of the function and return the newly created(or not) mid unit
			return getMid(su);
		}

	}

	//returns the adjusted version of a mid unit accounting for side rotation
	//returns the unit in the dir direction relative to the given su
	public TransportUnit getAdjustedMid(SurfaceUnit su)
	{
	

		//half the side length
		int halfside = midperglob/2;
		//if it is withing the bounds of a side(usually the case), it does not have to be modified
		if(su.u>=-halfside && su.u<halfside && su.v>=-halfside && su.v<halfside)
			return getMid(su);


		//all cases for back side
		if(su.side==PSide.BACK)
		{
			//if the u value is to far to the right, it is on the very left of the right side
			if(su.u>=halfside)
			{
				return getMid(new SurfaceUnit(PSide.RIGHT, -halfside, su.v));
			}
		}
		else if(su.side==PSide.RIGHT)
		{
			//if the u value is to far to the right, it is on the very left of the right side
			if(su.u<halfside)
			{
				return getMid(new SurfaceUnit(PSide.BACK, halfside-1, su.v));
			}
		}

		//if no other conditions are met, just return null
		return null;
	}

	//returns the large unit at a specified su, creates one if it does not exist(every large unit will exist)
	public TransportUnit getLarge(SurfaceUnit su)
	{
		//quick hopefully temporary check, see getMid for details
		if(su.u<-halfgtuw || su.u>=halfgtuw || su.v<-halfgtuw || su.v>=halfgtuw)
			return null;

		TransportUnit lu = null;

		//if it is in the list, return it
		if(largeTUs.TryGetValue(su, out lu))
		{
			return lu;
		}

		//if not, build one and return it
		TransportUnit tu = new TransportUnit();
		tu.conPoint = new Vector2((su.u+Random.value)*sideLengthLarge,(su.v+Random.value)*sideLengthLarge);
		tu.conUp = true;
		tu.conRight = true;
		tu.indexI = su.u;
		tu.indexJ = su.v;
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
	private SurfaceUnit getLargeSU(SurfaceUnit msu)
	{
		return new SurfaceUnit(msu.side, 
		                       Mathf.FloorToInt((float)msu.u/largeTUWidth),
		                       Mathf.FloorToInt((float)msu.v/largeTUWidth));
	}


	//list of all the mid transport units that are in a single large unit, connected in some way for building new level 2 roads, cleared for every new large unit
	private List<TransportUnit> indexList = new List<TransportUnit>();

	//returns the mid unit from midTUs at index u,v and creates one if necessary
	private TransportUnit buildMid(int i, int j, PSide side)
	{

		TransportUnit mu = null;
		SurfaceUnit su = new SurfaceUnit(side, i, j);
		//Debug.Log(su);
		if(!midTUs.TryGetValue(su, out mu))
		{
			mu = new TransportUnit();
			mu.indexI = i;
			mu.indexJ = j;
			midTUs.Add(su, mu);
			indexList.Add(mu);
		}
		return mu;
	}

	//populates a large unit with mid units
	private void populateLarge(TransportUnit lu, SurfaceUnit lus)
	{
		//Debug.Log("built " + lus);

		//clear the index list
		indexList.Clear();

		//find the mid unit that the large unit's conpoint falls in 
		int powIndexX, powIndexY;
		GridMath.findMidIndexfromPoint(lu.conPoint, midTUWidth, out powIndexX, out powIndexY);
		TransportUnit powMid = buildMid(powIndexX, powIndexY, lus.side);
		powMid.conPoint = lu.conPoint;//same conpoint, or could change to not be, doesn't really  matter
		powMid.conSet=true;

		TransportUnit rightLU = getLarge(new SurfaceUnit(lus.side,lus.u + 1, lus.v));
		TransportUnit leftLU = getLarge(new SurfaceUnit(lus.side,lus.u - 1, lus.v));
		TransportUnit upLU = getLarge(new SurfaceUnit(lus.side,lus.u, lus.v+1));
		TransportUnit downLU = getLarge(new SurfaceUnit(lus.side,lus.u, lus.v-1));
		//Vector2 conPointRight = rightLU.conPoint;

		//determine in which direction the streets should be built
		bool conRight = lu.conRight;
		bool conLeft = leftLU.conRight;
		bool conUp = lu.conUp;
		bool conDown = downLU.conUp;


		//list of the index of all the mid transport units that are connected in some way
		if(conRight)
			buildMidCurve(lu, rightLU.conPoint, Dir.RIGHT, powMid, lus.side);
		if(conLeft)
			buildMidCurve(lu, leftLU.conPoint, Dir.LEFT, powMid, lus.side);
		if(conUp)
			buildMidCurve(lu, upLU.conPoint, Dir.UP, powMid, lus.side);
		if(conDown)
			buildMidCurve(lu, downLU.conPoint, Dir.DOWN, powMid, lus.side);

		//build some level 2 streets coming off the level 1 streets or other level 2 streets
		int numStreets = 20;//(int)(Random.value*5);
		for(int i=0; i<numStreets; i++)
		{
			int startNum = Random.Range(0, indexList.Count);
			TransportUnit startMid = indexList[startNum];
			buildLev2(lu, startMid, Random.Range(2,20), lus.side);
		}


		Vector3 topright = UnitConverter.getWP(new SurfacePos(lus.side, (lu.indexI+1)*sideLengthLarge, (lu.indexJ+1)*sideLengthLarge), 10000, 1024);
		Vector3 topleft = UnitConverter.getWP(new SurfacePos(lus.side, (lu.indexI)*sideLengthLarge, (lu.indexJ+1)*sideLengthLarge), 10000, 1024);
		Vector3 botright = UnitConverter.getWP(new SurfacePos(lus.side, (lu.indexI+1)*sideLengthLarge, (lu.indexJ)*sideLengthLarge), 10000, 1024);
		Vector3 botleft = UnitConverter.getWP(new SurfacePos(lus.side, (lu.indexI)*sideLengthLarge, (lu.indexJ)*sideLengthLarge), 10000, 1024);
		Debug.DrawLine(topleft, botleft, Color.red, Mathf.Infinity);
		Debug.DrawLine(topleft, topright, Color.red, Mathf.Infinity);
		Debug.DrawLine(topright, botright, Color.red, Mathf.Infinity);
		Debug.DrawLine(botright, botleft, Color.red, Mathf.Infinity);


		lu.populated = true;

	}

	//builds a level 2 street of length from startMid within curLarge transport unit
	private void buildLev2(TransportUnit curLarge, TransportUnit startMid, int length, PSide side)
	{
		//the max and min indexes that the street can be built to(stay within the large unit)
		//move out of this function so it is not recalculated every time
		int maxI = (curLarge.indexI+1)*largeTUWidth-1;
		int minI = (curLarge.indexI)*largeTUWidth;
		int maxJ = (curLarge.indexJ+1)*largeTUWidth-1;
		int minJ = (curLarge.indexJ)*largeTUWidth;

		//the last mid unit to build away from
		TransportUnit lastMid = startMid;
		Dir lastDir;
		for(int i=0; i<length; i++)
		{
			//the direction to build
			//NOTE: modify later to not build back from the same direction
			Dir dir = (Dir)(Random.Range(1,5));

			//the index of the new mid unit to modify
			int curix = lastMid.indexI;
			int curiy = lastMid.indexJ;

			if(dir==Dir.RIGHT)
			{
				curix++;
				if(curix>maxI)
				{
					lastMid.conRight=true;
					lastMid.RightLev=2;
					break;
				}
			}
			if(dir==Dir.LEFT)
			{
				curix--;
				if(curix<minI)
					break;
			}
			if(dir==Dir.UP)
			{
				curiy++;
				if(curiy>maxJ)
				{
					lastMid.conUp=true;
					lastMid.UpLev=2;
					break;
				}
			}
			if(dir==Dir.DOWN)
			{
				curiy--;
				if(curiy<minJ)
					break;
			}

			//build (or just retrieve) the mid unit at the new indexes
			TransportUnit curMid = buildMid(curix, curiy, side);

			//set its conpoint if it has not already been set
			curMid.setConPoint(new Vector2((curix+Random.value)*midTUWidth,(curiy+Random.value)*midTUWidth));

			//MyDebug.placeMarker(UnitConverter.getWP(new SurfacePos(PSide.TOP, curMid.conPoint.x, curMid.conPoint.y), 10000, 1024), 10);

			//connect on level 2
			connectUnits(curMid, lastMid, 2);

			lastMid = curMid;
		}
	}

	//builds a street from the center of a large unit to the edge in the given direction
	private void buildMidCurve(TransportUnit lu, Vector2 outsideConPoint, Dir dir, TransportUnit powMid, PSide side)//powmid is the mid unit that contains the large's conpoint
	{


		int gix = 0, giy = 0;//goal index x and y of the goal mid unit

		float outSlope = GridMath.findSlope(lu.conPoint, outsideConPoint);//find the average slope from this conPoint to the outsideConPoint

		Debug.DrawLine(UnitConverter.getWP(new SurfacePos(PSide.TOP, lu.conPoint.x, lu.conPoint.y), 10000, 1024), 
		               UnitConverter.getWP(new SurfacePos(PSide.TOP, outsideConPoint.x, outsideConPoint.y), 10000, 1024), Color.blue, Mathf.Infinity);
		//determine the goal mid unit
		if(dir==Dir.RIGHT)
		{
			//find the index of the goal mid unit
			float goalPosX = (lu.indexI+1)*sideLengthLarge;//the x value of the very right side of the large unit in mid units
			float goalPosY = GridMath.findY(goalPosX, lu.conPoint, outSlope);//the y value on the line between teh two conpoints
			GridMath.findMidIndexfromPoint(new Vector2(goalPosX-0.5f, goalPosY), midTUWidth, out gix, out giy);

			TransportUnit goalMid = buildMid(gix, giy, side);
			goalMid.conRight=true;
			goalMid.RightLev = 1;
			//Debug.DrawLine(UnitConverter.getWP(new SurfacePos(PSide.TOP, lu.conPoint.x, lu.conPoint.y), 10000, 1024), 
			  //             UnitConverter.getWP(new SurfacePos(PSide.TOP, goalPosX, goalPosY), 10000, 1024), Color.blue, Mathf.Infinity);
		}
		else if(dir==Dir.LEFT)
		{
			//find the index of the goal mid unit
			float goalPosX = (lu.indexI)*sideLengthLarge;//the x value of the very right side of the large unit in mid units
			float goalPosY = GridMath.findY(goalPosX, lu.conPoint, outSlope);//the y value on the line between teh two conpoints
			GridMath.findMidIndexfromPoint(new Vector2(goalPosX+0.5f, goalPosY), midTUWidth, out gix, out giy);
			//buildMid(gix, giy, side).conLeft=true;
			
		}
		else if(dir==Dir.UP)
		{
			//find the index of the goal mid unit
			float goalPosY = (lu.indexJ+1)*sideLengthLarge;//the x value of the very right side of the large unit in mid units
			float goalPosX = GridMath.findX(goalPosY, lu.conPoint, outSlope);//the y value on the line between teh two conpoints
			GridMath.findMidIndexfromPoint(new Vector2(goalPosX, goalPosY-0.5f), midTUWidth, out gix, out giy);
			TransportUnit goalMid = buildMid(gix, giy, side);
			goalMid.conUp=true;
			goalMid.UpLev = 1;

			
		}
		else if(dir==Dir.DOWN)
		{
			//find the index of the goal mid unit
			float goalPosY = (lu.indexJ)*sideLengthLarge;//the x value of the very right side of the large unit in mid units
			float goalPosX = GridMath.findX(goalPosY, lu.conPoint, outSlope);//the y value on the line between teh two conpoints
			GridMath.findMidIndexfromPoint(new Vector2(goalPosX, goalPosY+0.5f), midTUWidth, out gix, out giy);
		}
		//the x and y index of the mid unit last created in the loop(starts as if pows were last created)
		//int lastix = powIndexX, lastiy = powIndexY;
		TransportUnit lastMid = powMid;
		
		while(true)
		{
			
			//the difference in indexes between the current and goal mid units
			int xdif = gix - lastMid.indexI;
			int ydif = giy - lastMid.indexJ;
			
			//the direction to move in this loop iteration(1=x 2=y)
			int movedir;
			
			//if they are both not on the goal index, pick a random one to change
			if(xdif!=0 && ydif!=0)
				movedir = Random.value>0.5f ? 1:2;
			else if(xdif!=0)
				movedir = 1;
			else if(ydif!=0)
				movedir = 2;
			else//if they are both zero we are done maybe?
				break;//?
			
			//the index of the mid unit to be created
			int curix = lastMid.indexI, curiy = lastMid.indexJ;
			
			//if moving in the x direction
			if(movedir==1)
			{
				if(xdif>0)
					curix++;
				else
					curix--;
			}
			else//movedir==2
			{
				if(ydif>0)
					curiy++;
				else
					curiy--;
			}
			
			//create or retrieve the new mid unit
			TransportUnit curMid = buildMid(curix, curiy, side);

			//set its conpoint if it has not already been set
			curMid.setConPoint(new Vector2((curix+Random.value)*midTUWidth,(curiy+Random.value)*midTUWidth));

			//MyDebug.placeMarker(UnitConverter.getWP(new SurfacePos(PSide.TOP, curMid.conPoint.x, curMid.conPoint.y), 10000, 1024), 5);
			
			connectUnits(curMid, lastMid, 1);
			
			lastMid = curMid;
		}
		


	}




	//connects two transport units based on their relation to each other
	public void connectUnits(TransportUnit u1, TransportUnit u2, int lev)//the two units to set connectivity, lev is basically street size(1 is largest)
	{
		if(u1.indexI + 1 == u2.indexI && u1.indexJ == u2.indexJ)//if the second unit is directly to the right of the first one
		{
			u1.conRight = true;//connect the first u to the right because the second u is on the right
			u1.RightLev = lev;
		} else if(u1.indexI - 1 == u2.indexI && u1.indexJ == u2.indexJ)//if the second u unit is directly to the left of the first one
		{
			u2.conRight = true;
			u2.RightLev = lev;
		} else if(u1.indexI == u2.indexI && u1.indexJ + 1 == u2.indexJ)//if the second u unit is directly to the top of the first one
		{
			u1.conUp = true;
			u1.UpLev = lev;
		} else if(u1.indexI == u2.indexI && u1.indexJ - 1 == u2.indexJ)//if the second u unit is directly to the bottom of the first one
		{
			u2.conUp = true;
			u2.UpLev = lev;
		}
	}
	
}
