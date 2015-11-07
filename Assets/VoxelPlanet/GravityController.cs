using UnityEngine;
using System.Collections;

public class GravityController : MonoBehaviour {

	public float gravity;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		//attracts player to 0,0 by changing gravity
		Physics.gravity = Vector3.Scale((transform.position).normalized, new Vector3(-10,-10,-10));
		
		//rb.rotation = 
	}
}
