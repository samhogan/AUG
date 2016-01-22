using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private static int pRefX;
	private static int pRefY;
	private static int pRefZ;

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
	void Start()
	{
		print(Quaternion.Inverse(Quaternion.Euler(0,-45,0)).eulerAngles);
		
		pRefX = 0;
		pRefY = 0;
		pRefZ = 0;

		tRefX = 0;
		tRefY = 0;
		tRefZ = 0;

		initialPositioning();
	}

	//properly positions the player and unitracker and sets all ref points
	//this initial setup has to be done differently because the tracker cannot reference the unset player position
	void initialPositioning()
	{

		bool startOnPlanet = false;

		if(startOnPlanet)
		{
			//the starting point of the player in relation to the starting planet
			//this will eventually be handled by a double precision vector3
			Vector3 startPoint = new Vector3(0,250020,0);
			//Vector3 startPoint = new Vector3(0,250020,0);
			Planet startPlanet = UniverseSystem.planets[0];
			UniverseSystem.curPlanet = startPlanet;

			onPlanet = true;

			//parent tracker and set up position
			transform.SetParent(startPlanet.scaledRep.transform);
			transform.localPosition = startPoint/uniscale;

			//calculate the initial player ref points and relative position
			reposPlayer();
		}
		else
		{
			Vector3 startPoint = new Vector3(0,0,0);
			transform.localPosition = startPoint/uniscale;
		
			onPlanet = false;

			reposPlayer();
		}

	}
	
	// Update is called once per frame
	void Update()
	{

		//calculate the position of the tracker based on the position of the player
		float scaledX = pRefX+(player.transform.localPosition.x/uniscale)-tRefX;
		float scaledY = pRefY+(player.transform.localPosition.y/uniscale)-tRefY;
		float scaledZ = pRefZ+(player.transform.localPosition.z/uniscale)-tRefZ;

		transform.localPosition = new Vector3(scaledX, scaledY, scaledZ);
		//print("pos is " + transform.position);

		//print(UniverseSystem.curPlanet!=null);
		if(onPlanet)
			checkSpace();
		else
			checkPlanets();
	 
		checkTrackerPos();
		checkPlayerPos();
		//checkTrackerPos();
	
	}


	//returns the real position relative to the planet's center
	//ONLY use this for planet positioning, also make double precision later
	public static Vector3 getRealPos(Vector3 floatPos)
	{
		floatPos.x+=pRefX*uniscale;
		floatPos.y+=pRefY*uniscale;
		floatPos.z+=pRefZ*uniscale;
		return floatPos;
	}
	
	//returns the floating position relative to the unity world origin given a real position relative to the planet origin 
	//used for positioning world objects on the planet
	public static Vector3 getFloatingPos(Vector3 realPos)
	{
		realPos.x-=pRefX*uniscale;
		realPos.y-=pRefY*uniscale;
		realPos.z-=pRefZ*uniscale;
		return realPos;
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
	

	//sets up proper positioning and rotations of objects for a new planet
	void setCurPlanet(Planet plan)
	{
		onPlanet=true;
		UniverseSystem.curPlanet = plan;

		print("p player initial rot is " + player.transform.rotation.eulerAngles);
		

		//parent the unitracker to the planet and set the new ref points relative to the planet center
		transform.SetParent(plan.scaledRep.transform, true);
		reposPlayer();


		print("p tracker parented rot is " + transform.rotation.eulerAngles);
		
		

		

		//rotate the player and other objects around it
		//player.transform.localRotation = Quaternion.Inverse(transform.localRotation)*player.transform.localRotation;
		//player.transform.localRotation = player.transform.localRotation*transform.localRotation;//Quaternion.Euler(0,-45,0);

		//player.transform.rotation = player.transform.rotation*transform.localRotation;
		//player.transform.rotation*=transform.localRotation;
		player.transform.rotation=transform.localRotation*player.transform.rotation;
		

		print("p new player rot is " + player.transform.rotation.eulerAngles);
		

		//print(transform.localRotation.eulerAngles);
		transform.localRotation = Quaternion.identity;

	}


	//repositions the player and sets pRefs based on the position of the tracker
	//this is only done with initial setup and leaving and entering a planet
	private void reposPlayer()
	{

		//calculate the new reference points
		pRefX = Mathf.RoundToInt(transform.localPosition.x);
		pRefY = Mathf.RoundToInt(transform.localPosition.y);
		pRefZ = Mathf.RoundToInt(transform.localPosition.z);
		
		
		//calculate new position of the player
		player.transform.position = new Vector3((transform.localPosition.x-pRefX)*uniscale,
		                                        (transform.localPosition.y-pRefY)*uniscale,
		                                        (transform.localPosition.z-pRefZ)*uniscale);

		//rotate the player to account for a rotated planet/leaving a rotated planet
		//player.transform.localRotation = player.transform.localRotation*transform.localRotation;
		
	}

	//checks if the player is far enough away from the current planet to be in space!!!!!!
	void checkSpace()
	{
		//if outside the atmosphere distance of the current planet, stop being part of that planet
		if(Vector3.Distance(transform.position,UniverseSystem.curPlanet.scaledRep.transform.position)>UniverseSystem.curPlanet.scaledAtmosRadius)
		{
			onPlanet = false;
			UniverseSystem.curPlanet = null;
			transform.parent = null;

			print("player initial rot is " + player.transform.rotation.eulerAngles);

			reposPlayer();

			//player.transform.rotation = player.transform.rotation*Quaternion.Inverse(transform.localRotation);
			//player.transform.rotation = player.transform.rotation*transform.localRotation;

			//
			print("tracker parented rot is " + transform.rotation.eulerAngles);
			
			//rotate the player rotation by the negative rotation of the planet
			//player.transform.rotation*=transform.localRotation;
			player.transform.rotation=transform.localRotation*player.transform.rotation;
			//player.transform.rotation = Quaternion.


			print("new player rot is " + player.transform.rotation.eulerAngles);
			

			//print(transform.rotation.eulerAngles);
			transform.localRotation = Quaternion.identity;


			//clear up all the request system arrays
			//yes sam you need to implement this asap
			//but i don't wanna
			//too bad, DO IT!!!!!
			//no
			//RequestSystem.builtObjects.Clear();
			//TerrainSystem.chunks.Clear();
			//SurfaceSystem.surfList.Clear();
		}
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
		//print("the relRef is " + relRef);

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
			//player.transform.position+=new Vector3(shift, 0, 0);
			moveWorldObjects(new Vector3(shift,0,0));
			//set the new ref
			pRefX = newRefX;
		}
		if(transform.localPosition.y>relRef.y+0.5f || transform.localPosition.y<relRef.y-0.5f)
		{
			//the new reference point y coordinate
			int newRefY = Mathf.RoundToInt(transform.localPosition.y)+(onPlanet ? 0:tRefY);
			
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefY-newRefY)*uniscale;
			moveWorldObjects(new Vector3(0, shift, 0));
			//set the new ref
			pRefY = newRefY;
		}
		if(transform.localPosition.z>relRef.z+0.5f || transform.localPosition.z<relRef.z-0.5f)
		{
			//the new reference point z coordinate
			int newRefZ = Mathf.RoundToInt(transform.localPosition.z)+(onPlanet ? 0:tRefZ);
			
			//how much to shift the player(and other objects) by in normal space units
			int shift = (pRefZ-newRefZ)*uniscale;

			moveWorldObjects(new Vector3(0, 0, shift));
			//set the new ref
			pRefZ = newRefZ;
		}

	}

	//moves all worldobjects in the world a certain amount
	private void moveWorldObjects(Vector3 shift)
	{
		/*foreach(KeyValuePair<WorldPos, List<WorldObject>> objectList in RequestSystem.builtObjects)
		{
			foreach(WorldObject wo in objectList.Value)
			{
				wo.transform.position+=shift;
			}
		}*/
		
		//also shift the player(should eventually not have to do this)
		player.transform.position+=shift;
	}

}
