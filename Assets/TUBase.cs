using UnityEngine;
using System.Collections;

//the base transport unit
public class TUBase : TransportUnit 
{
	//only base units can connect diagonally
	public bool conUpRight = false;//is the streetPoint connected to the street to the top right of it(forming a street) ?
	public bool conUpLeft = false;



	private int upRightLev = 0;
	private int upLeftLev = 0;

	public int UpRightLev
	{
		get{return upRightLev;} 
		set{
			if(upRightLev==0)
			{
				upRightLev=value;
			}
		}
	}

	public int UpLeftLev
	{
		get{return upLeftLev;} 
		set{
			if(upLeftLev==0)
			{
				upLeftLev=value;
			}
		}
	}

}
