using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Substance
{

	public Vector2 colorPoint;//the location of the color on the atlas
	public const int atlasWidth = 64;

	public Substance()
	{
		
	}

	public Substance(float xcp, float ycp)
	{
		xcp /= atlasWidth;//texture points are in the range [0,1]
		ycp /= atlasWidth;
		colorPoint = new Vector2(xcp, ycp);
	}


	public static Dictionary<Sub, Substance> subs = new Dictionary<Sub, Substance>() 
	{
		{Sub.TEST, new Substance(16,16)},
		{Sub.ICE, new Substance(31,0)},
		{Sub.ROCK, new Substance(0,32)},
		{Sub.VEGITATION, new Substance(9,23)},
		{Sub.ROCK2, new Substance(4, 28)},
	};


}

//all the substances enumerated
public enum Sub
{
	TEST,
	ICE,
	ROCK,
	VEGITATION,
	ROCK2,
}
