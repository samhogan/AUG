using UnityEngine;
using System.Collections;
//using System;

//[Serializable]

//this is a voxel position in 3d space
//basically an integer version of a vector3
public struct WorldPos
{
	public int x, y, z;//the only variables that matter in this struct
	
	public WorldPos(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3 toVector3()
	{
		return new Vector3(x, y, z);
	}


	//this was in the tutorial so i guess it will live in here
	public override bool Equals(object obj)
	{
		if (GetHashCode() == obj.GetHashCode())
			return true;
		return false;
	}
	
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 47;
			
			hash = hash * 227 + x.GetHashCode();
			hash = hash * 227 + y.GetHashCode();
			hash = hash * 227 + z.GetHashCode();
			
			return hash;
		}
	}
}