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

	public Substance(float xcp, float ycp, double sa)
	{
		xcp /= atlasWidth;//texture points are in the range [0,1]
		ycp /= atlasWidth;
		colorPoint = new Vector2(xcp, ycp);

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
		{Sub.TEST, new Substance(16,16,0)},
		{Sub.WaterIce, new Substance(31,0,1)},
		{Sub.Vegitation1, new Substance(9,23,1)},
		{Sub.Vegitation2, new Substance(4,19,1)},
		{Sub.Vegitation3, new Substance(8,26,1)},
		{Sub.Sand, new Substance(15, 29, 20)},
		{Sub.Dirt, new Substance(5, 29, 1)},
		{Sub.Mud, new Substance(2, 30,1 )},
		{Sub.Basalt, new Substance(6,0, 34)},
		{Sub.Baslat2, new Substance(8,0, 33)},
		{Sub.Baslat3, new Substance(10,0, 33)},
		{Sub.IronDioxide, new Substance(10,30,10)}, 
		{Sub.IronDioxide2, new Substance(10,31, 0)}, 
		{Sub.Gold, new Substance(18,27,0.01)},
		{Sub.Granite, new Substance(27,31,33)},//igneous
		{Sub.Andesite, new Substance(15,0,33)},//igneous
		//{Sub.Anorthosite, new Substance(15,0,33)},//igneous
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
