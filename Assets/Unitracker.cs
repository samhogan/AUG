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
	int pRefX;
	int pRefY;
	int pRefZ;

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

		checkTrackerPos();
		
		checkPlayerPos();
		//checkTrackerPos();
	
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
	void checkPlayerPos()
	{
		//pRef point relative to the tRef point
		Vector3 relRef = new Vector3(pRefX-tRefX, pRefY-tRefY, pRefZ-tRefZ);
		print("the relRef is " + relRef);
		//if(player.transform.position.x>halfus)//the other way to do it using player pos rather than tracker pos
		//if the player has exceeded the play area(5000 units in any direction or .5 scaled units)
		if(transform.position.x>relRef.x+0.5f || transform.position.x<relRef.x-0.5f)
		{
			//the new reference point x coordinate
			int newRefX = Mathf.RoundToInt(transform.position.x)+tRefX;
			//print("the new pRefX is " + pRefX);
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefX-newRefX)*uniscale;
			//print("the shift is " + shift);
			player.transform.position+=new Vector3(shift, 0, 0);
			
			//set the new ref
			pRefX = newRefX;
		}
		if(transform.position.y>relRef.y+0.5f || transform.position.y<relRef.y-0.5f)
		{
			//the new reference point y coordinate
			int newRefY = Mathf.RoundToInt(transform.position.y)+tRefY;
			
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefY-newRefY)*uniscale;
			player.transform.position+=new Vector3(0, shift, 0);
			
			//set the new ref
			pRefY = newRefY;
		}
		if(transform.position.z>relRef.z+0.5f || transform.position.z<relRef.z-0.5f)
		{
			//the new reference point z coordinate
			int newRefZ = Mathf.RoundToInt(transform.position.z)+tRefZ;
			
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefZ-newRefZ)*uniscale;
			player.transform.position+=new Vector3(0, 0, shift);
			
			//set the new ref
			pRefZ = newRefZ;
		}

	}

}
