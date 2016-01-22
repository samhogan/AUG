using UnityEngine;
using System.Collections;

public class RotationTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation=Quaternion.Euler(0,0.1f,0)*transform.rotation;
		//transform.Rotate(0,0.5f,0,Space.World);
	}
}
