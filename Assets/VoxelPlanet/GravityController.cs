﻿using UnityEngine;
using System.Collections;

public class GravityController : MonoBehaviour {

	public float gravity;
	// Use this for initialization
	void Start () 
	{
		//for testing the ship and stuff... ya know
		Physics.gravity = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		//attracts player to 0,0 by changing gravity

		Vector3 realPos = FloatingOrigin.getRealPos(transform.position);
		//Physics.gravity = Vector3.Scale((realPos).normalized, new Vector3(-10,-10,-10));
		
		//rb.rotation = 
	}
}
