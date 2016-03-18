using UnityEngine;
using System.Collections;

//the surface coordinate system point
public struct SurfacePos
{
	public PSide side;//side of the planet
	public float x, y;//grid point on that side, deprecated because it gets confusing(use u and v)
	public float u, v;//grid point on that side
	
	public SurfacePos(PSide s, float upos, float vpos)
	{
		side = s;
		u = upos;
		v = vpos;
		x = y = 0;//remove later
	}

	//returns the surface unit that this surface point is in (rounds down)
	public SurfaceUnit toUnit()
	{
		return new SurfaceUnit(side, Mathf.FloorToInt(u), Mathf.FloorToInt(v));
	}

	public override string ToString()
	{
		return side + " " + u + " " + v;
	}
	
}

//an integer version of the SurfacePos used for refering to specific surface units rather than just a point
//for an analogy: Vector3 is to WorldPos as SurfacePos is to SurfaceUnit
public struct SurfaceUnit
{
	public PSide side;//side of the planet
	public int u, v;//grid point on that side
	
	public SurfaceUnit(PSide s, int upos, int vpos)
	{
		side = s;
		u = upos;
		v = vpos;
	}
	
	public override string ToString()
	{
		return side + " " + u + " " + v;
	}
	
}

//the 6 sides of a planetary body, each side has its own grid system
public enum PSide
{
	TOP,
	BOTTOM,
	RIGHT,
	LEFT,
	FRONT,
	BACK,
	NONE
}
