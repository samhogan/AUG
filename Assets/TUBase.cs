using UnityEngine;
using System.Collections;

//the base transport unit
public class TUBase : TransportUnit 
{
	//only base units can connect diagonally
	public bool conUpRight = false;//is the streetPoint connected to the street to the top right of it(forming a street) ?
	public bool conUpLeft = false;

	//has the conPoint been officially set yet?
	public bool conSet = false;

	public int upRightLev;
	public int upLeftLev;

}
