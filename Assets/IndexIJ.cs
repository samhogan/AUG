using UnityEngine;
using System.Collections;

//basically a vector2 but with inegers, used to keep track of what transport units have roads through them
//Why i and j? I have no stinkin' idea...
public struct IndexIJ 
{
	int i;
	int j;

	public IndexIJ(int ival, int jval)
	{
		i = ival;
		j = jval;
	}

}
