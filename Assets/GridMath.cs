using UnityEngine;
using System.Collections;

//contains math functions used in transport system calculations
public class GridMath 
{

	//finds the base unit that a certain point falls in given the current mid unit
	public static void findBaseIndexfromPoint(Vector2 conPoint, out int indexX, out int indexY)
	{
		//rounds down to an integer to find the index
		indexX = Mathf.FloorToInt(conPoint.x);
		indexY = Mathf.FloorToInt(conPoint.y);
		//indexX = Mathf.FloorToInt(conPoint.x - midTUWidth * midU.indexI);
		//indexY = Mathf.FloorToInt(conPoint.y - midTUWidth * midU.indexJ);
		
	}

	//finds the mid unit that a point falls in 
	public static void findMidIndexfromPoint(Vector2 conPoint, int midTUWidth, out int indexX, out int indexY)
	{
		indexX = Mathf.FloorToInt(conPoint.x/midTUWidth);
		indexY = Mathf.FloorToInt(conPoint.y/midTUWidth);
	}
	
	public static float findSlope(Vector2 v1, Vector2 v2)
	{
		if(v1.x == v2.x)
		{
			Debug.Log("infinite slope");
			return 100;//if infinite, just return a big number, 300 seems to not cause any problems
			//return 10000000f;
			//return Mathf.Infinity;//i don't know what this will do but hopefully it will work
		}
		return (v1.y - v2.y) / (v1.x - v2.x);
		
	}
	
	//returns the slope perpindicular to the given slope
	public static float perp(float slope)
	{
		
		if(slope == 0f || slope == 0)
		{
			Debug.Log("infinite slope from perp function");
			return 100;//if infinite, just return a big number, 300 seems to not cause any problems
			//return 1000009f;
			//return Mathf.Infinity;//again i don't know what this will do but hopefully it will work
		}
		else if(slope == Mathf.Infinity)
			return 0;
		return -1 / slope;
	}
	
	//finds a point given 2 points and 2 slopes (uses a modifies point slope form)
	public static Vector2 findPoint(float x1, float y1, float s1, float x2, float y2, float s2)
	{
		float x3 = (s1 * x1 - s2 * x2 - y1 + y2) / (s1 - s2);//two equations in point slope form solved for y set equal to each other and solved for x
		
		float y3 = s1 * (x3 - x1) + y1; //point slope of y1 set equal to y
		
		return new Vector2(x3, y3);
	}

	//returns the x value of a cooresponding y value on a line given another point and the slope
	public static float findX(float yval, Vector2 point, float slope)
	{
		return (yval - point.y + slope * point.x) / slope;//uses modified point slope form
	}

	public static float findY(float xval, Vector2 point, float slope)
	{
		return slope * (xval - point.x) + point.y;//uses modified point slope form
	}
}
