using UnityEngine;
using System.Collections;

public class RotTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.rotation = Quaternion.FromToRotation (transform.up, transform.position) * transform.rotation;
		
	}
}
