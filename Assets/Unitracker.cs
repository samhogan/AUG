using UnityEngine;
using System.Collections;

//this tracker exists in unispace and tracks the position of the player in the universe
//it shall also eventually control floating origin (sorry dedicated FloatingOrigin script, it's better this way)
public class Unitracker : MonoBehaviour
{
	//reference to the player (or whatever is being tracked)
	public GameObject player;

	//is the player on a planet?(or asteroid or whatever gosh..)
	public static bool onPlanet = false;

	//the ratio of normal space to unispace, also used for number of units to exceed floating origin boundaries
	public static int uniscale = 10000;
	//half the uniscale 
	private static int halfus = uniscale/2;

	//how far the unitracker has to travel in unity units to be moved back by floating origin
	private static int unithreshold = 10000;
	private static int halfut = unithreshold/2;



	//the position of the player in THE UNIVERSE!!!!! (1:1 scale)
	//UniPos playerPos;
	//the player's reference point in the univers(what point its transform position is relative to)
	//when in space, relative to the universe origin, when on a planet, relative to planet center
	int pRefX;
	int pRefY;
	int pRefZ;

	//the player's reference point on the planet(same thing as pRef except rotated in space)
	//int planetRefX;
	//int planetRefY;
	//int planetRefZ;

	//the position of the player relative to the planet(if it is on one)
	//UniPos planetPos;

	//the position of the tracker in THE UNIVERSE!!!!! (1:10000 scale)
	//UniPos trackerPos;
	//the tracker's reference point(in increments of 10000)
	private static int tRefX;
	private static int tRefY;
	private static int tRefZ;


	// Use this for initialization
	void OnEnable()
	{
		pRefX = 0;
		pRefY = 0;
		pRefZ = 0;

		tRefX = 0;
		tRefY = 0;
		tRefZ = 0;
	}
	
	// Update is called once per frame
	void Update()
	{

		//calculate the position of the tracker based on the position of the player
		float scaledX = pRefX+(player.transform.position.x/uniscale)-tRefX;
		float scaledY = pRefY+(player.transform.position.y/uniscale)-tRefY;
		float scaledZ = pRefZ+(player.transform.position.z/uniscale)-tRefZ;

		transform.position = new Vector3(scaledX, scaledY, scaledZ);
		//print("pos is " + transform.position);

		print(UniverseSystem.curPlanet!=null);
		if(onPlanet)
			checkSpace();
		else
			checkPlanets();
	 
		checkTrackerPos();
		checkPlayerPos();
		//checkTrackerPos();
	
	}

	//checks if the player is in the jurisdiction of any planet
	void checkPlanets()
	{
		foreach(Planet plan in UniverseSystem.planets)
		{
			//if the player is inside the planets "atmosphere" the it is the curplanet
			if(Vector3.Distance(transform.position,plan.scaledRep.transform.position)<plan.scaledAtmosRadius)
			{
				setCurPlanet(plan);
				break;
			}

		}
	}

	void checkSpace()
	{

	}

	//sets up proper positioning and rotations of objects for a new planet
	void setCurPlanet(Planet plan)
	{
		onPlanet=true;
		UniverseSystem.curPlanet = plan;

		transform.parent = plan.scaledRep;
		pRefX = Mathf.RoundToInt(transform.localPosition.x);

		//calculate new position of the player
		player.transform.position = new Vector3((transform.localPosition.x-pRefX)*uniscale,
		                                        (transform.localPosition.y-pRefY)*uniscale,
		                                        (transform.localPosition.z-pRefZ)*uniscale);

		//rotate the player and other objects around it



	}



	//convers a unipos to an absolute world pos based on the current tRef
	public static Vector3 UniToAbs(UniPos uni)
	{
		Vector3 abs = new Vector3();
		abs.x = (uni.refX-tRefX)+uni.relPos.x;
		abs.y = (uni.refY-tRefY)+uni.relPos.y;
		abs.z = (uni.refZ-tRefZ)+uni.relPos.z;

		return abs;
	}



	//checks if the tracker is outside the precision threshold (5000 units from the origin) and moves it (and planets) back
	void checkTrackerPos()
	{

		bool newRef = false;//the ref has changed
		if(transform.position.x>halfut || transform.position.x<-halfut)
		{
			//the offset from the current tRefX to the new reference point
			int shift = Mathf.RoundToInt(transform.position.x/unithreshold)*unithreshold;
			tRefX+=shift;
			//the direction to shift in
			transform.position += new Vector3(-shift, 0, 0);
			newRef=true;
		//	print("offsetX is " + shift);
		}
		if(transform.position.y>halfut || transform.position.y<-halfut)
		{
			//the offset from the current tRefX to the new reference point
			int shift = Mathf.RoundToInt(transform.position.y/unithreshold)*unithreshold;
			tRefY+=shift;
			//the direction to shift in
			transform.position += new Vector3(0,-shift, 0);
			newRef=true;
			//print("offsetX is " + shift);
		}
		if(transform.position.z>halfut || transform.position.z<-halfut)
		{
			//the offset from the current tRefX to the new reference point
			int shift = Mathf.RoundToInt(transform.position.z/unithreshold)*unithreshold;
			tRefZ+=shift;
			//the direction to shift in
			transform.position += new Vector3(0,0,-shift);
			newRef=true;

		}

		//only reposition the planets if the tref has changed
		if(newRef)
			reposPlanets();

	}

	//recalculates all planet scaledRep positions 
	void reposPlanets()
	{
		foreach(Planet plan in UniverseSystem.planets)
		{
			plan.scaledRep.transform.position = UniToAbs(plan.scaledPos);
		}
	}




	//checks if the player is outside the precision threshold (5000 units from the origin) and moves it (and world objects) back
	//used with space and on planet
	void checkPlayerPos()
	{
		//pRef point relative to the local unity origin
		//if on planet, relative to the planet center
		//if in space, relative to the unispace origin
		Vector3 relRef;
		if(onPlanet)
			relRef=new Vector3(pRefX, pRefY, pRefZ);
		else
			relRef= new Vector3(pRefX-tRefX, pRefY-tRefY, pRefZ-tRefZ);
		print("the relRef is " + relRef);

		//if(player.transform.position.x>halfus)//the other way to do it using player pos rather than tracker pos
		//if the player has exceeded the play area(5000 units in any direction or .5 scaled units)
		if(transform.localPosition.x>relRef.x+0.5f || transform.localPosition.x<relRef.x-0.5f)
		{
			//the new reference point x coordinate
			int newRefX = Mathf.RoundToInt(transform.localPosition.x) + (onPlanet ? 0:tRefX);//only add the tRefX if in space
			//print("the new pRefX is " + pRefX);
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefX-newRefX)*uniscale;
			//print("the shift is " + shift);
			player.transform.position+=new Vector3(shift, 0, 0);
			
			//set the new ref
			pRefX = newRefX;
		}
		if(transform.localPosition.y>relRef.y+0.5f || transform.localPosition.y<relRef.y-0.5f)
		{
			//the new reference point y coordinate
			int newRefY = Mathf.RoundToInt(transform.localPosition.y)+(onPlanet ? 0:tRefY);
			
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefY-newRefY)*uniscale;
			player.transform.position+=new Vector3(0, shift, 0);
			
			//set the new ref
			pRefY = newRefY;
		}
		if(transform.localPosition.z>relRef.z+0.5f || transform.localPosition.z<relRef.z-0.5f)
		{
			//the new reference point z coordinate
			int newRefZ = Mathf.RoundToInt(transform.localPosition.z)+(onPlanet ? 0:tRefZ);
			
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefZ-newRefZ)*uniscale;
			player.transform.position+=new Vector3(0, 0, shift);
			
			//set the new ref
			pRefZ = newRefZ;
		}

	}

}
