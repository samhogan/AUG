using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//attaches to a game object and loads items on the surface
public class SurfaceLoader3 : MonoBehaviour
{
	
	public int sideLength = 8; //this is temporary probably, later dependent on current planet
	private float halfSide;//half the side length used more than once

	private List<SurfacePos> surfList = new List<SurfacePos>();//surface poses that have already been loaded
	public GameObject tree;//used for instantiation testing

	// Use this for initialization
	void Start()
	{
		halfSide = sideLength/2;

		/*for(int i = 0; i<=20; i++)
		{
			Vector3 randpos = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));
			//Quaternion rot = Quaternion.LookAt(Vector3.down, randpos);
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, randpos);
			GameObject tree1 = GameObject.Instantiate(tree, randpos, rot) as GameObject;
			//tree1.transform.LookAt(Vector3.zero, Vector3.down);
		}*/
		/*for(int i = 0; i<=8; i++)
		{
			for(int j = 0; j<=8; j++)
			{
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = new Vector3(-4 + i, 4, -4 + j).normalized * 20;
			}
		}*/

		//surface position test
		/*for(int i = -4; i<=4; i++)
		{
			for(int j = -4; j<=4; j++)
			{

				SurfacePos sp = new SurfacePos(PSide.TOP, i, j);
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = getWP(sp, 20);
				
				
			}
		}*/
	}
	
	// Update is called once per frame
	void Update()
	{
		//print for testing
		//print (getSP (transform.position));
		//getSP(transform.position);

		/*SurfacePos sp = getSP(transform.position);
		sp.u = Mathf.Floor(sp.u);//turn into units rather than 5.82376982759 or something
		sp.v = Mathf.Floor(sp.v);
		//print(sp);
		if(!surfList.Contains(sp))
		{
			surfList.Add(sp);
			Vector3 wp = getWP(sp, 200);//world position of surface unit
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, wp);//points away from center
			GameObject.Instantiate(tree, wp, rot);
		}
		//print(surfList.Count);*/
	}

	//converts a surface position to a world position given a surfacepos and radius
	//this converts the surfacepos to a point on a unit cube, which is then converted to a point on a unit sphere
	public Vector3 getWP(SurfacePos sp, float radius)
	{
		//point on a unit cube
		Vector3 cubePos;

		//converts surface position to a world position on a cube(not on a sphere yet)
		switch(sp.side)
		{
		case PSide.TOP:
			cubePos = new Vector3(sp.u, halfSide, sp.v);//halfside in the y component means this point is on the top of the cube
			break;
		case PSide.BOTTOM:
			cubePos = new Vector3(sp.u, -halfSide, sp.v);
			break;
		case PSide.RIGHT:
			cubePos = new Vector3(halfSide, sp.v, sp.u);
			break;
		case PSide.LEFT:
			cubePos = new Vector3(-halfSide, sp.v, sp.u);
			break;
		case PSide.FRONT:
			cubePos = new Vector3(sp.u, sp.v, halfSide);
			break;
		case PSide.BACK:
			cubePos = new Vector3(sp.u, sp.v, -halfSide);
			break;
		default://won't ever happen
			cubePos = new Vector3();
			break;
		}

		//shrinks the cube down to having a side length of 2
		cubePos /= halfSide;

		//point on a unit sphere
		Vector3 spherePos = new Vector3();

		//this formula maps the coordinates on a cube to coordinates on a sphere
		//found here: http://mathproofs.blogspot.co.uk/2005/07/mapping-cube-to-sphere.html

		//for use in the formula
		float xsq = cubePos.x * cubePos.x;
		float ysq = cubePos.y * cubePos.y;
		float zsq = cubePos.z * cubePos.z;


		spherePos.x = cubePos.x * Mathf.Sqrt(1.0f - ysq * 0.5f - zsq * 0.5f + ysq * zsq / 3.0f);
		spherePos.y = cubePos.y * Mathf.Sqrt(1.0f - xsq * 0.5f - zsq * 0.5f + xsq * zsq / 3.0f);
		spherePos.z = cubePos.z * Mathf.Sqrt(1.0f - ysq * 0.5f - xsq * 0.5f + ysq * xsq / 3.0f);

		//resulting vector is 1 unit long, so multiply by some specified radius
		return spherePos * radius;
	}

	//returns the surface Position (side, x, and y) given a world xyz position
	//inverse of getWP
	private SurfacePos getSP(Vector3 pos)
	{
		pos.Normalize();//normalize the pos to get a point on the unit sphere
		//will later be returned after infoed(new word i just made up)
		SurfacePos sp = new SurfacePos();
		
		//absolute values of each component
		float absx = Mathf.Abs(pos.x);
		float absy = Mathf.Abs(pos.y);
		float absz = Mathf.Abs(pos.z);
		
		//the position of a point on a cube with side length 2 in world space
		//one of the values will always be 1 or -1 
		Vector3 cubePos = new Vector3();
		
		//determines the side and assigns x and y values
		//basically, the direction that is farthes from the center determines the side you are on
		if(absy > absx && absy > absz) //if the y value is the farthest it will be the top or bottom
		{
			//pos.y == -1 or 1 (bottom or top)
			//calculate the remaining cube components
			cubify(pos.x, pos.z, out sp.u, out sp.v);

			if(pos.y >= 0)
				sp.side = PSide.TOP;//if positive y value, it is the top
			else
				sp.side = PSide.BOTTOM;
		}
		else if(absx > absy && absx > absz) //if the x value is the farthest it will be the Right or left
		{
			//calculate the remaining cube components
			cubify(pos.z, pos.y, out sp.u, out sp.v);
			
			if(pos.x >= 0)
				sp.side = PSide.RIGHT;//if positive x value, it is the right
			else
				sp.side = PSide.LEFT;
		}
		else if(absz > absy && absz > absx) //if the z value is the farthest it will be the front or back
		{
			//calculate the remaining cube components
			cubify(pos.x, pos.y, out sp.u, out sp.v);
			
			if(pos.z >= 0)
				sp.side = PSide.FRONT;//if positive z value, it is the front
			else
				sp.side = PSide.BACK;
		}
		else //shouldn't happen
		{
			sp.x = 0;
			sp.y = 0;
			sp.side = PSide.NONE;
		}
		
		
		//these values have a range of 2 [-1,1], so multiply them by half the side length
		sp.u *= halfSide;
		sp.v *= halfSide;
		
		return sp;
	}


	const float iSqrt2 = 0.70710676908493042f;//this function needs to divide by the square root of 2
	//takes some coordinates from one face of a unit sphere and turns them into coordinates on the side of a cube
	//NOTE: the side of the sphere is unknown, this function is used in getSP
	//a and b are the x, y, or z values on a sphere's surface
	public void cubify(float a, float b, out float u, out float v)//a and b will coorespond to a u and v component
	{
		//set u and v surface coordinates 
		//found this here: http://stackoverflow.com/questions/2656899/mapping-a-sphere-to-a-cube
		//a and b are the remaining sphere components (not the longest one)
		//normalize the pos to get a point on the unit sphere
		float aa2 = a*a*2.0f;
		float bb2 = b*b*2.0f;
		float inner = -aa2 + bb2 - 3;//piece of the eqation used multiple times
		float innersqrt = -Mathf.Sqrt((inner * inner) - 12.0f * aa2);
		u = Mathf.Sign(a)*Mathf.Sqrt(innersqrt + aa2 - bb2 + 3.0f) * iSqrt2;//multiply by the sign (+ or -) to get the correct final sign
		v = Mathf.Sign(b)*Mathf.Sqrt(innersqrt - aa2 + bb2 + 3.0f) * iSqrt2;

		//Note to self: FIGURE OUT HOW THIS THING WORKS!!!!!!!!
		//yes future code readers, i don't know how this actually works
		//i tried to figure out the inverse function myself but i couldn't
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
	
	public override string ToString()
	{
		return side + " " + u + " " + v;
	}
	
}
