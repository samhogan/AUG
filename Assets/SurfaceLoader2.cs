using UnityEngine;
using System.Collections;

//attaches to a game object and loads items on the surface
public class SurfaceLoader2 : MonoBehaviour
{
	
	public int sideLength = 8; //this is temporary probably, later dependent on current planet
	private float halfSide;//half the side length used more than once

	// Use this for initialization
	void Start()
	{
		halfSide = sideLength/2;
		
		/*for(int i = 0; i<=8; i++)
		{
			for(int j = 0; j<=8; j++)
			{
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = new Vector3(-4 + i, 4, -4 + j).normalized * 20;
			}
		}*/
		for(int i = -4; i<=4; i++)
		{
			for(int j = -4; j<=4; j++)
			{
				Vector3 vPosition = new Vector3(i/4f,1,j/4f);

				float x2 = vPosition.x * vPosition.x;
				float y2 = vPosition.y * vPosition.y;
				float z2 = vPosition.z * vPosition.z;



				vPosition.x = vPosition.x * Mathf.Sqrt(1.0f - (y2 * 0.5f) - (z2 * 0.5f) + ((y2 * z2) / 3.0f));
				vPosition.y = vPosition.y * Mathf.Sqrt(1.0f - (z2 * 0.5f) - (x2 * 0.5f) + ((z2 * x2) / 3.0f));
				vPosition.z = vPosition.z * Mathf.Sqrt(1.0f - (x2 * 0.5f) - (y2 * 0.5f) + ((x2 * y2) / 3.0f));

				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = vPosition * 20;


			}
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		//print for testing
		///print (getSP (transform.position));
		//getSP(transform.position);
	}
	
	//returns the surface Position (side, x, and y) given a world xyz position
	private SurfacePos getSP(Vector3 pos)
	{
		//will later be returned after infoed(new word i just made up)
		SurfacePos sp = new SurfacePos();

		//absolute values of each component
		float absx = Mathf.Abs(pos.x);
		float absy = Mathf.Abs(pos.y);
		float absz = Mathf.Abs(pos.z);

		//the position of a point on a cube with side length 2 in world space
		//one of the values will always be 1 or -1 
		Vector3 cubePos;

		//determines the side and assigns x and y values
		//basically, the direction that is farthes from the center determines the side you are on
		if(absy > absx && absy > absz) //if the y value is the farthest it will be the top or bottom
		{
			//set cubepos to find surface coordinates (divides pos by y value to get a cube point)
			cubePos = pos / absy;//might change to just pos.y instead of absy later

			//set x and y surface coordinates
			sp.x = cubePos.x;
			sp.y = cubePos.z;

			if(pos.y >= 0)
				sp.side = PSide.TOP;//if positive y value, it is the top
			else
				sp.side = PSide.BOTTOM;
		}
		else if(absx > absy && absx > absz) //if the x value is the farthest it will be the Right or left
		{
			//set cubepos to find surface coordinates (divides pos by x value to get a cube point)
			cubePos = pos / absx;
			
			//set x and y surface coordinates
			sp.x = cubePos.z;
			sp.y = cubePos.y;
			
			if(pos.x >= 0)
				sp.side = PSide.RIGHT;//if positive x value, it is the right
			else
				sp.side = PSide.LEFT;
		}
		else if(absz > absy && absz > absx) //if the z value is the farthest it will be the front or back
		{
			//set cubepos to find surface coordinates (divides pos by z value to get a cube point)
			cubePos = pos / absz;
			
			//set x and y surface coordinates
			sp.x = cubePos.x;
			sp.y = cubePos.y;
			
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
		//adding halfside again makes all values positive(just my preference of how things should work)
		sp.x = sp.x*halfSide+halfSide;
		sp.y = sp.y*halfSide+halfSide;

		
		return sp;
	}
	


}

//the 6 sides of a planetary body, each side has its own grid system
/*public enum PSide
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
		return side + " " + (int)x + " " + (int)y;
	}
	
}
*/