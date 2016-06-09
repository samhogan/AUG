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

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
