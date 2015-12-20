using UnityEngine;
using System.Collections;

public class StreetUnitBase
{

	public bool blSet;//if the bl point has already been set

	//only base units can connect to base units diagonally
	public bool conUpRight;//is the streetPoint connected to the street to the top right of it(forming a street) ?
	public bool conUpLeft;


	public StreetUnitBase(Vector2 bl)
	{
		//streetPointBL = bl;//the bl point
		blSet = true;
		//streetPointPer = bl - (int)(bl);//street point percent within this base unit
	}
	public StreetUnitBase()
	{

	}

}
