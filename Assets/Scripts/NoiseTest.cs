using UnityEngine;
using System.Collections;
using LibNoise.Generator;
public class NoiseTest : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		//Billow b = new Billow(
		Perlin p = new Perlin(1, 2, .5, 1, 456254, LibNoise.QualityMode.High);
		//ImprovedPerlin p = new ImprovedPerlin();
		//Billow p = new Billow();
		System.DateTime stime = System.DateTime.Now;
		for (int i = 0; i < 10000000; i++)
			p.GetValue (34, 2345, 346);
		print (stime - System.DateTime.Now);

	}

	// Update is called once per frame
	void Update () {

	}
}
