using UnityEngine;
using System.Collections;

public class Ship : MobileObjects
{
	public GameObject player;
	public static bool playerOn = false;
	Rigidbody rb;
//	FixedJoint fj;

	public float speed = 20f;
	public float hyperSpeed = 200000f;

	private bool isHyper = false;

	void Start()
	{

		Render();
		rb = gameObject.AddComponent<Rigidbody>();
		rb.useGravity = false;
		gameObject.GetComponent<MeshCollider>().convex = true;
		//fj = gameObject.AddComponent<FixedJoint>();
		//fj.connectedBody = player.GetComponent<Rigidbody>();
		//rb.isKinematic = true;
	}

	void Update()
	{
		//print((player.transform.position - transform.position).magnitude <2);
		if(playerOn)
		{
			if(Input.GetKeyDown("u"))
			{
				player.transform.position = transform.position + new Vector3(-3, 0, 0);
				//fj.connectedBody = null;
				//player.transform.parent = null;
				playerOn = false;
			}
		}
		else if((player.transform.position - transform.position).magnitude < 2) 
		{
			player.transform.parent = transform;
			player.GetComponent<Rigidbody>().isKinematic = true;
			player.GetComponent<Rigidbody>().detectCollisions = false;
			//r.enabled = false;
			//player

			//fj.connectedBody = player.GetComponent<Rigidbody>();
			player.transform.localPosition = transform.up;
			player.transform.rotation = transform.rotation;
			playerOn = true;
			//player.GetComponent<Rigidbody>().
		}

		if(Input.GetKeyDown("h"))
		{
			if(!isHyper && !Unitracker.onPlanet)
				isHyper = true;
			else if(isHyper)
				isHyper = false;
		}

		//keep hyper speed off when on a planet 
		if(Unitracker.onPlanet)
		{
			isHyper = false;
		
			Vector3 realPos = Unitracker.getRealPos(transform.position);
			speed = Mathf.Abs(realPos.magnitude - UniverseSystem.curPlanet.noise.getAltitude(realPos)) * 10 + 3500;
			//print(Mathf.Abs(Unitracker.getRealPos(transform.position).magnitude - UniverseSystem.curPlanet.radius));
		}
		else
		{
			//speed = 2000000;
		}
	}

	void FixedUpdate()
	{
		if(playerOn)
		{
			float forward = Input.GetAxisRaw("Vertical");
			float right = Input.GetAxisRaw("Horizontal");
			float throttle = Input.GetButton("Jump") ? (isHyper ? hyperSpeed : speed) : 0.0f;


			rb.angularVelocity = Vector3.zero;
			rb.velocity = Vector3.zero;

			rb.AddRelativeTorque(0, right * 70, forward * 70, ForceMode.Acceleration);
			//transform.Rotate(0, right, forward);
			rb.AddForce(transform.right * throttle, ForceMode.Force);


			if(forward == 0 && right == 0)
				rb.angularVelocity = Vector3.zero;
			if(throttle == 0)
				rb.velocity = Vector3.zero;
		}
		else
		{
			rb.angularVelocity = Vector3.zero;
			rb.velocity = Vector3.zero;

		}

	}

	public override void Render()
	{
		MeshBuilder mb = new MeshBuilder();
		ProcMesh.addCube(mb, Vector3.zero, 5, 4, 1, Sub.Gold);
		setMesh(mb.getMesh());
	}
}
