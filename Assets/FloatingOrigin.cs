using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//keeps the player and nearby objects centered around the origin to avoid floating point errors
public class FloatingOrigin : MonoBehaviour
{
	//the size in unity units of the offset chunk size
	static int step = 50;
	static int halfstep = step/2;

	//the number of offset chunks away from the origin of some width
	static int offsetX = 0;
	static int offsetY = 0;
	static int offsetZ = 0;

	// Use this for initialization
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update()
	{
		//if position is greater than step, move everything back
		if(transform.position.x>halfstep || transform.position.x<-halfstep)
		{
			//the amount of offsets
			int offset = Mathf.RoundToInt(transform.position.x/step);
			offsetX+=offset;
			//the direction to shift in
			int shiftx = offset*-step;
			moveWorldObjects(new Vector3(shiftx,0,0));
		}
		if(transform.position.y>halfstep || transform.position.y<-halfstep)
		{
			int offset = Mathf.RoundToInt(transform.position.y/step);
			offsetY+=offset;
			int shifty = offset*-step;
			moveWorldObjects(new Vector3(0,shifty,0));
		}
		if(transform.position.z>halfstep || transform.position.z<-halfstep)
		{
			int offset = Mathf.RoundToInt(transform.position.z/step);
			offsetZ+=offset;
			int shiftz = offset*-step;
			moveWorldObjects(new Vector3(0,0,shiftz));
		}

	}

	//moves all worldobjects in the world a certain amount
	private void moveWorldObjects(Vector3 shift)
	{
		foreach(KeyValuePair<WorldPos, List<WorldObject>> objectList in RequestSystem.builtObjects)
		{
			foreach(WorldObject wo in objectList.Value)
			{
				wo.transform.position+=shift;
			}
		}

		//also shift the player(should eventually not have to do this)
		transform.position+=shift;
	}
	//returns the real position relative to the planet's center 
	public static Vector3 getRealPos(Vector3 floatPos)
	{
		floatPos.x+=offsetX*step;
		floatPos.y+=offsetY*step;
		floatPos.z+=offsetZ*step;
		return floatPos;
	}

	//returns the floating position relative to the unity world origin given a real position relative to the planet origin 
	public static Vector3 getFloatingPos(Vector3 realPos)
	{
		realPos.x-=offsetX*step;
		realPos.y-=offsetY*step;
		realPos.z-=offsetZ*step;
		return realPos;
	}

}
