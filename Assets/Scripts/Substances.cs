using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Substance
{

	public Vector2 colorPoint;//the location of the color on the atlas
	public const int atlasWidth = 64;

	//abundance of substance on the surface of a planet
	//0-100 scale (although can be higher(if you want a universe full of gold or something))
	public double surfAb;
	//public float crustAb;
	//public float

	public Substance()
	{
		
	}


	public Substance(int x, int y, double sa) : this(0, 0, 153, sa)
	{
	}

	public Substance(int r, int g, int b, double sa)
	{
		//calculate the color position in the atlas
		int bpos = Mathf.CeilToInt(b/17f);
		float xcp = (bpos%4)*16+Mathf.CeilToInt(g/17f);
		float ycp = (bpos/4)*16+Mathf.CeilToInt(r/17f);
			

		xcp /= atlasWidth;//texture points are in the range [0,1]
		ycp /= atlasWidth;
		colorPoint = new Vector2(xcp, ycp);
		//Debug.Log(xcp+" "+ycp);
		surfAb = sa;
	}


	//***Static stuff***///

	public static ProbItems surfProb;

	//set up probitems
	static Substance()
	{
		List<double> surfIts = new List<double>();
		List<double> surfWeights = new List<double>();

		foreach(KeyValuePair<Sub, Substance> sub in subs)
		{
			if (sub.Value.surfAb>0)
			{
				surfIts.Add((double)sub.Key);
				surfWeights.Add(sub.Value.surfAb);
			}
		}

		surfProb = new ProbItems(surfIts.ToArray(), surfWeights.ToArray());

	}



	//a dictionary of an instances of each substance
	public static Dictionary<Sub, Substance> subs = new Dictionary<Sub, Substance>() 
	{
		{Sub.TEST, new Substance(2,254,255,0)},
		{Sub.WaterIce, new Substance(255,255,255,1)},
		{Sub.Vegitation1, new Substance(47,155,0,1)},
		{Sub.Vegitation2, new Substance(0,84,37,1)},
		{Sub.Vegitation3, new Substance(123,141,0,1)},
		{Sub.Sand, new Substance(240,135,0, 20)},
		{Sub.Dirt, new Substance(98,55,0, 1)},
		{Sub.Mud, new Substance(56,21,0,1 )},
		{Sub.Basalt, new Substance(45,45,45, 34)},
		{Sub.Baslat2, new Substance(61,61,61, 33)},
		{Sub.Baslat3, new Substance(78,78,78, 33)},
		{Sub.IronDioxide, new Substance(169,63,0,10)}, 
		{Sub.IronDioxide2, new Substance(169,31,0, 0)}, 
		{Sub.Gold, new Substance(255,242,29,0.01)},
		{Sub.Granite, new Substance(217,163,132,33)},//igneous
		{Sub.Andesite, new Substance(180,183,183,33)},//igneous
		{Sub.Anorthosite, new Substance(172,162,140,33)},//igneous
	};



//	public static Sub rock = { Sub.ROCK, Sub.ROCK2, Sub.BASALT, Sub.BASALT2, Sub.BASALT3 };

}

//all the substances enumerated
public enum Sub
{
	TEST,
	WaterIce,
	Sand,
	Vegitation1,
	Vegitation2,//darker grass
	Vegitation3,//dying grass
	Dirt,
	Mud,
	Basalt,
	Baslat2,
	Baslat3,
	IronDioxide,
	IronDioxide2,
	Gold,
	Granite,
	Andesite,
	Anorthosite,
}
