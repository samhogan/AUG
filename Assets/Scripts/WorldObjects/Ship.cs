using UnityEngine;
using System.Collections;

public class Ship : MobileObjects
{
	public GameObject player;
	public static bool playerOn = false;
	Rigidbody rb;
//	FixedJoint fj;

	public float speed = 20f;
	public float hyperSpeed = 300000000f;

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
				player.transform.parent = null;
				player.transform.position = transform.position + transform.right*3 + transform.up;
				//fj.connectedBody = null;
				playerOn = false;
				player.GetComponent<Rigidbody>().isKinematic = false;
				player.GetComponent<Rigidbody>().detectCollisions = true;
			}
		}
		else if((player.transform.position - transform.position).magnitude < 2 || Input.GetKeyDown("j")) 
		{
			player.transform.parent = transform;
			player.GetComponent<Rigidbody>().isKinematic = true;
			player.GetComponent<Rigidbody>().detectCollisions = false;
			//r.enabled = false;
			//player

			//fj.connectedBody = player.GetComponent<Rigidbody>();
			player.transform.localPosition = new Vector3(0,1,1);
			player.transform.rotation = transform.rotation;
			playerOn = true;
			//player.GetComponent<Rigidbody>().
		}

		if(Input.GetKeyDown("h"))
		{
			if(!playerOn)
				GameUI.ui.displayMessage("must be on ship to activate super-c drive");
			else if(!isHyper && CoordinateSystem.curPlanet==null)
			{
				isHyper = true;
				GameUI.ui.displayMessage("super-c drive activated");
			}
			else if(isHyper)
			{
				isHyper = false;
				GameUI.ui.displayMessage("super-c drive deactivated");
			}
			else if(CoordinateSystem.curPlanet!=null)
				GameUI.ui.displayMessage("too close to planet to activate super-c drive");

		}

		//keep hyper speed off when on a planet 
		if(CoordinateSystem.curPlanet!=null)
		{
			if(isHyper)
			{
				isHyper = false;
				GameUI.ui.displayMessage("super-c drive deactivated because hitting a planet at faster-than-light speed is not beneficial to one's health");
			}
			Vector3 realPos = CoordinateHandler.planetSpace.getRealPos();//TODO: idk something
			speed = Mathf.Abs(realPos.magnitude - UniverseSystem.curPlanet.noise.getAltitude(realPos)) * 9 + 3500;
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
			float pitch = Input.GetAxisRaw("Vertical");
			float yaw = Input.GetAxisRaw("Horizontal");
			float throttle = Input.GetButton("Jump") ? (isHyper ? hyperSpeed : speed) : 0.0f;

			float roll = Input.GetKey("q") ? 1 : Input.GetKey("e") ? -1 : 0;

			rb.angularVelocity = Vector3.zero;
			rb.velocity = Vector3.zero;

			rb.AddRelativeTorque(pitch * -70, yaw * 70, roll * 70, ForceMode.Acceleration);
			//transform.Rotate(0, right, forward);
			rb.AddForce(transform.forward * throttle, ForceMode.Force);


			if(pitch == 0 && yaw == 0 && roll == 0)
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
		ProcMesh.addCube(mb, Vector3.zero, 4, 5, .5f, Sub.Gold);
		meshCollider.sharedMesh = mb.getMesh();
		ProcMesh.addCube(mb, new Vector3(1.75f, 2, 2.25f), .5f, .5f, 4, Sub.Gold);
		ProcMesh.addCube(mb, new Vector3(-1.75f, 2, 2.25f), .5f, .5f, 4, Sub.Gold);
		ProcMesh.addCube(mb, new Vector3(0, 4, 2.25f), 4f, .5f, .5f, Sub.Gold);
		filter.mesh = mb.getMesh();
		//setMesh(mb.getMesh());
	}
}
