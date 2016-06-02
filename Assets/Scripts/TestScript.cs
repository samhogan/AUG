using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		ProbItems p = new ProbItems(new double[]{2,1,1});
		Debug.Log(p.getValue(.5));
		Debug.Log(p.getValue(.6));
		Debug.Log(p.getValue(.2));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
