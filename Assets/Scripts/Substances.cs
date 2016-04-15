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
		{Sub.VEGITATION2, new Substance(4,19)},
		{Sub.VEGITATION3, new Substance(8,26)},
		{Sub.ROCK2, new Substance(4, 28)},
		{Sub.SAND, new Substance(15, 29)},
		{Sub.DIRT, new Substance(5, 29)},
		{Sub.MUD, new Substance(2, 30)},
		{Sub.BASALT, new Substance(6,0)},
		{Sub.BASALT2, new Substance(8,0)},
		{Sub.BASALT3, new Substance(10,0)},
		{Sub.IRONOXIDE, new Substance(10,30)}, 
		{Sub.IRONOXIDE2, new Substance(10,31)}, 
	};


}

//all the substances enumerated
public enum Sub
{
	TEST,
	ICE,
	ROCK,
	ROCK2,
	SAND,
	VEGITATION,
	VEGITATION2,//darker grass
	VEGITATION3,//dying grass
	DIRT,
	MUD,
	BASALT,
	BASALT2,
	BASALT3,
	IRONOXIDE,
	IRONOXIDE2,
}
