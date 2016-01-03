﻿using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour
{

	public float speed = 20f;

	private Rigidbody rb;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate()
	{
		
		float forward = Input.GetAxisRaw("Vertical");
		float right = Input.GetAxisRaw("Horizontal");
		float throttle = Input.GetButton("Jump") ? speed : 0.0f;



		rb.AddRelativeTorque(0, right*10, forward*10, ForceMode.Acceleration);
		rb.AddForce(transform.right*throttle, ForceMode.Force);

		if(forward==0&&right==0)
			rb.angularVelocity = Vector3.zero;
		if(throttle==0)
			rb.velocity=Vector3.zero;


	}
}
