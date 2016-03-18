using UnityEngine;
using System.Collections;

//handles very large positions by using a reference point and a vector3
public struct UniPos 
{
	//precise position relative to the reference point
	public Vector3 relPos;

	//the reference point, will probably eventually use long instead of int
	public int refX;
	public int refY;
	public int refZ;

	public UniPos(Vector3 rp, int rx, int ry, int rz)
	{
		relPos = rp;
		refX = rx;
		refY = ry;
		refZ = rz;
	}

}
