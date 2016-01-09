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
	private static int uniscale = 10000;
	//half the uniscale 
	private static int halfus = uniscale/2;

	//how far the unitracker has to travel in unity units to be moved back by floating origin
	private static int unithreshold = 10000;



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
	int tRefX;
	int tRefY;
	int tRefZ;


	// Use this for initialization
	void Start()
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
		float scaledX = pRefX+(player.transform.position.x/uniscale)-tRefX*unithreshold;
		float scaledY = pRefY+(player.transform.position.y/uniscale)-tRefY*unithreshold;
		float scaledZ = pRefZ+(player.transform.position.z/uniscale)-tRefZ*unithreshold;

		transform.position = new Vector3(scaledX, scaledY, scaledZ);

		//if(player.transform.position.x>halfus)//the other way to do it using player pos rather than tracker pos
		//if the player has exceeded the play area(5000 units in any direction or .5 scaled units)
		if(transform.position.x>pRefX+0.5f || transform.position.x<pRefX-0.5f)
		{
			//the new reference point x coordinate
			int newRefX = Mathf.RoundToInt(transform.position.x)+tRefX;

			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefX-newRefX)*uniscale;
			player.transform.position+=new Vector3(shift, 0, 0);

			//set the new ref
			pRefX = newRefX;
		}
		if(transform.position.y>pRefY+0.5f || transform.position.y<pRefY-0.5f)
		{
			//the new reference point x coordinate
			int newRefY = Mathf.RoundToInt(transform.position.y)+tRefY;
			
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefY-newRefY)*uniscale;
			player.transform.position+=new Vector3(0, shift, 0);
			
			//set the new ref
			pRefY = newRefY;
		}
		if(transform.position.z>pRefZ+0.5f || transform.position.z<pRefZ-0.5f)
		{
			//the new reference point x coordinate
			int newRefZ = Mathf.RoundToInt(transform.position.z)+tRefZ;
			
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefZ-newRefZ)*uniscale;
			player.transform.position+=new Vector3(0, 0, shift);
			
			//set the new ref
			pRefZ = newRefZ;
		}
	
	}
}
