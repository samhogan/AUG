using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {
	
	public float speed = 2; //velocity when walking
	public float jump = 2; //force added when jumping

	private Rigidbody rb;
	private CapsuleCollider capsule;
	private float groundCheck = 0.01f; //distance below player to check for ground
	private bool wasMoving = false; //if the player was moving the last frame (used to stop the player)


	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody>();
		capsule = GetComponent<CapsuleCollider>();

	}
	
	// Update is called once per frame
	void Update ()
	{
		//if(Input.GetButton("Jump"))

	}


	void FixedUpdate()
	{

		float forward = Input.GetAxis("Vertical");
		float right = Input.GetAxis("Horizontal");
		float up = Input.GetButton("Jump") ? jump : 0.0f;
		//Vector3 force = new Vector3 (right, 0.0f, forward);
		//force = transform.TransformDirection(force) * speed;\
		//o
		//finds the global direction and force to move
		/*Vector3 force = (transform.forward * forward + transform.right * right) * speed;
		force.y = rb.velocity.y;*/


		Vector3 newVel = transform.InverseTransformDirection(rb.velocity); //converts current velocity to local space
		newVel = new Vector3 (right * speed, newVel.y, forward * speed);//overrides right and forward speed, but preserves y speed
		newVel = transform.TransformDirection(newVel);//converts back to global space
		//moves the player
		//rb.AddForce(force*speed, ForceMode.Impulse); 
		bool buttonPush = forward != 0 || right != 0;

		//*****************
		//apply force but stop with velocity 


		//if a button was pushed this update
		if(buttonPush)
			wasMoving = true;
		//if a button was pushed this update or last
		if(wasMoving)
			rb.velocity = newVel;

		//keeps wasmoving true for an update after it stops moving
		if(!buttonPush)
			wasMoving=false;

		//apply velocity when a button is pushed or a button was pushed the last frame

		//jumps if touching the ground
		if( isGrounded() )
			//rb.AddForce(0.0f, up, 0.0f, ForceMode.VelocityChange);
			rb.AddForce(transform.up*up, ForceMode.VelocityChange);

		//planet.Attract(transform);

		//points player away from planet
		rb.rotation = Quaternion.FromToRotation (transform.up, rb.position) * rb.rotation;
		//print(Quaternion.FromToRotation (transform.up, transform.position) * transform.rotation);
		/*Vector3 gravityUp = (transform.position).normalized;
		Vector3 localUp = transform.up;
		
		// Apply downwards gravity to body
		rb.AddForce(gravityUp * 10);
		// Allign bodies up axis with the centre of planet
		rb.rotation = Quaternion.FromToRotation(localUp,gravityUp) * transform.rotation;*/
	}


	//finds if the player is touching the ground
	private bool isGrounded()
	{
		RaycastHit hitInfo;
		return Physics.SphereCast(transform.position, capsule.radius, -transform.up, out hitInfo, (capsule.height / 2f) - capsule.radius + groundCheck);
	}


}
