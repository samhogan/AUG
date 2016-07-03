using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		ProbMeter p = new ProbMeter(new double[]{0,2,4,8}, new double[]{2,1,1} );
		Debug.Log(p.getValue(.25));
		Debug.Log(p.getValue(.5));
		Debug.Log(p.getValue(1));

		int[] count = { 0, 0, 0, 0 };

		for(int i = 0; i<100000000; i++)
		{
			float num = PlanetBuilder.eDist(.5, 10000, Random.value);
			if(num<10)
				count[0]++;
			else if(num<100)
				count[1]++;
			else if(num<1000)
				count[2]++;
			else if(num<10000)
				count[3]++;
			 
		}

		print(count[0] + " " + count[1]+" "+count[2]+" "+count[3]);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
