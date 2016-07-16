using UnityEngine;
using System.Collections;

public struct LongPos
{
	public long x, y, z;

	public LongPos(long xpos, long ypos, long zpos)
	{
		x = xpos;
		y = ypos;
		z = zpos;
	}

	public Vector3 toVector3()
	{
		return new Vector3((float)x, (float)y, (float)z);
	}

	public static LongPos operator +(LongPos p1, LongPos p2)
	{
		return new LongPos(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
	}

	public static LongPos operator -(LongPos p1, LongPos p2)
	{
		return new LongPos(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
	}

	public static LongPos operator /(LongPos p1, int num)
	{
		return new LongPos(p1.x/num, p1.y/num, p1.z/num);
	}

    public static LongPos operator *(LongPos p1, int num)
    {
        return new LongPos(p1.x * num, p1.y * num, p1.z * num);
    }



    //distance between two longpos using distance formula
    public static double Distance(LongPos p1, LongPos p2)
    {
        return System.Math.Sqrt(System.Math.Pow(p1.x - p2.x, 2) + System.Math.Pow(p1.y - p2.y, 2) + System.Math.Pow(p1.y - p2.y, 2));       
    }


}