using UnityEngine;
using System.Collections;
using LibNoise.Generator;
using System.Collections.Generic;
public class NoiseTest : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{

		Dictionary<SurfacePos, float> dict = new Dictionary<SurfacePos, float>();
		//Billow b = new Billow(
		Perlin p = new Perlin(1, 2, .5, 1, 456254, LibNoise.QualityMode.High);
		//ImprovedPerlin p = new ImprovedPerlin();
		//Billow p = new Billow();
		System.DateTime stime = System.DateTime.Now;
		//for (int i = 0; i < 10000000; i++)
			p.GetValue (34, 2345, 346);
		for(int i = 0; i < 30000; i++)
			dict.ContainsKey(new SurfacePos());
		print (stime - System.DateTime.Now);

	}

	// Update is called once per frame
	void Update () {

	}
}
