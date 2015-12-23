using UnityEngine;
using System.Collections;

public class TransportUnit 
{
	public Vector2 conPoint; //the point in the unit where the segments connect
	public Vector3 conPointWorld;//the connecting point in its world position

	public bool conRight = false;//is the streetPoint connected to the street above it(forming a street) ?
	public bool conUp = false;//is the streetPoint connected to the street to the right of it
	public bool conUpRight = false;//is the streetPoint connected to the street to the top right of it(forming a street) ?
	public bool conUpLeft = false;

	public bool populated = false;//for mid and large tus,has teh transport unit been populated with smaller transport units?

}
