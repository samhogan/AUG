using UnityEngine;
using System.Collections;

public class SurfaceLoader : MonoBehaviour {

	//sqrt(2)/2 the coordinate value of the corner of a square normalized(mapped onto a unit circle), 
	//used to find what face of the planet the player is on
	private static readonly float sCorner = Mathf.Sqrt (2) / 2;//0.57735026919f;

	public int sideLength = 8; //this is temporary probably, later dependent on current planet

	//normalized 2d vectors to simplify math
	Vector2 normXY;
	Vector2 normZY;
	Vector2 normXZ;


	// Use this for initialization
	void Start () {

		for (int i = 0; i<=8; i++) {
			for (int j = 0; j<=8; j++) {
				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				sphere.transform.position = new Vector3 (-4+i, 4, -4+j).normalized * 20;
			}
		}
		/*for (int i = 0; i<=8; i++) {
			for (int j = 0; j<=8; j++) {
				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				//Vector3 pos = new Vector3(-1,1,-1).normalized*20;
				//pos = Quaternion.Euler(0,0,-90/8*i) * pos;//rotate vector x direction
				//pos = Quaternion.Euler(-90/8*j,0,0) * pos;//rotate vector x direction

				//sphere.transform.position = pos;
				sphere.transform.position =  Quaternion.Euler(-90/8*j,0,-90/8*i) * new Vector3(-1,1,-1) * 20;
			}
		}*/

	}
	
	// Update is called once per frame
	void Update () {
		//print for testing
		//print (getSP (transform.position));
		getSP (transform.position);
	}

	//recalculates the 2d normalized vectors 
	private void calculate2DNormalized(Vector3 pos)
	{
		normXY = new Vector2 (pos.x, pos.y).normalized;
		normZY = new Vector2 (pos.z, pos.y).normalized;
		normXZ = new Vector2 (pos.x, pos.z).normalized;
	}


	//returns the surfacePosition given a world xyz position
	private SurfacePos getSP(Vector3 pos)
	{
		//will later be returned after infoed(new word i just made up)
		SurfacePos sp = new SurfacePos ();

		//recalculates 2d vectors
		calculate2DNormalized(pos);

		PSide side = getSide ();//the current side of the current position
		sp.side = side;//this will be part of the output

		if (side == PSide.TOP) 
		{
			sp.x = CPtoSP(normXY.x);
			sp.y = CPtoSP(normZY.x);
		}
		if (side == PSide.BOTTOM) 
		{
			sp.x = CPtoSP(normXY.x);
			sp.y = CPtoSP(normZY.x);
		}

		if (Mathf.Abs (pos.y) > Mathf.Abs (pos.x) && Mathf.Abs (pos.y) > Mathf.Abs (pos.z))
			print (pos / pos.y);

		return sp;
	}

	//this function takes in a coordinate and side length and returns the 1d position on a flat surface(one x or y coordinate on the side of a planet
	//other words, takes a coordinate on a circle and returns the cooresponding coordinate on a square of a given length
	//also called Sam's Function named after myself for discovering it
	//cp to sp means circle point to square point
	private float CPtoSP(float v)//v is an x or y coordinate on a unit circle (-1<=x<=1)
	{
		//int negative = v < 0 ? -1 : 1;//is the coordinate negative?

		float v2 = v * v;//value squared
		return Mathf.Sqrt ((v2 * (sideLength/2) * (sideLength/2)) / (1 - v2));// * negative + sideLength/2;
	}

	//returns the side given a world position
	private PSide getSide()//add param!!!!!!!!!!
	{
		//the normalized vector of the players position (point on a unit sphere)
		//Vector3 norm = transform.position.normalized;//put this in parameter later

		//finds side by checking if one of the values "bulges" out from a corner
		//basically, all y values that coorespond to the top side will be greater than sqrt3/3 and so on
		//ok, changed to simplified 2d using sqrt2/2
		//if (norm.y>0 && -sCorner<norm.x && norm.x<sCorner && -sCorner<norm.z && norm.z<sCorner)//(norm.y > sCorner)
		if(normXY.y>sCorner && normZY.y>sCorner)
			return PSide.TOP;
		else if (normXY.y < -sCorner && normZY.y<-sCorner)
			return PSide.BOTTOM;
		else if (normXY.x > sCorner && normXZ.x > sCorner)
			return PSide.RIGHT;
		else if (normXY.x < -sCorner && normXZ.x < -sCorner)
			return PSide.LEFT;
		else if (normXZ.y > sCorner && normZY.x > sCorner)//.x and .y refer to the z direction in this case, kind of confusing
			return PSide.FRONT;
		else if (normXZ.y < -sCorner && normZY.x < -sCorner)
			return PSide.BACK;
		else
			return PSide.NONE;//catch, should rarely happen


	}
}

//the 6 sides of a planetary body, each side has its own grid system
public enum oldPSide{TOP, BOTTOM, RIGHT, LEFT, FRONT, BACK, NONE};

//the surface coordinate system point
public struct oldSurfacePos
{
	public PSide side;//side of the planet
	public float x, y;//grid point on that side

	/*public SurfacePos()
	{
		side = PSide.NONE;
		x = 0f;
		y = 0f;
	}*/

	public oldSurfacePos(PSide s, float xpos, float ypos)
	{
		side = s;
		x = xpos;
		y = ypos;
	}

	public override string ToString()
	{
		return side + " " + x + " " + y;
	}

}

//struct of 3 vector 2's
public struct Vector2_3
{



}