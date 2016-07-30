using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {


	public float lookSpeed = 10f; //speed camera moves & player spins
	public float smoothTime = 10f; //fraction to reach target rotations
	public bool smooth = true; //smooth camera movement with slerp

	//vertical range
	private float vRange = 90.0f;

	private Rigidbody rb;
	private GameObject cam;
	private Vector3 playerTargetRot; //
	private Quaternion camTargetRot; //target vertical rotation to be slerped to
	private float vertRot = 0f; //camera vertical rotation

	private float prot = 0f;

    public GameObject sitRotater;
	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody>();
		cam = GameObject.FindGameObjectWithTag("PlayerCam");
		playerTargetRot = transform.localEulerAngles;
		camTargetRot = cam.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () 
	{

		float xMove = Input.GetAxis("Mouse X") * lookSpeed;
		float yMove = Input.GetAxis("Mouse Y") * lookSpeed;

		//stores camera vertical rotation so it can be clamped
		vertRot += yMove;
		vertRot = Mathf.Clamp(vertRot, -vRange, vRange);

		playerTargetRot += new Vector3(0f, xMove, 0f);

		prot = xMove;

		//print (playerTargetRot);

		//camTargetRot *= Quaternion.Euler(-yMove, 0f, 0f);
		camTargetRot = Quaternion.Euler(-vertRot, 0f, 0f);
		//camTargetRot = Quaternion.Euler(camTargetRot.eulerAngles.y, 0f, 0f);
		
		//Debug.Log(camTargetRot);

		//transform.localEulerAngles += new Vector3 (0f, 1f, 0f);

		if(smooth) 
		{

            //transform.eulerAngles += Vector3.Slerp(new Vector3(0f,0f,0f), new Vector3(0f, prot, 0f), smoothTime * Time.deltaTime);
            //rb.rotation = 
            if(PlayerMove1.sitting)
                sitRotater.transform.Rotate(Vector3.up * xMove);
            else
                transform.Rotate(Vector3.up * xMove);//remember to change to use slerp and rigidbody
			//rb.MoveRotation(
			cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, camTargetRot, smoothTime * Time.deltaTime);
		}
		else 
		{
			//rb.rotation = playerTargetRot;
			cam.transform.localRotation = camTargetRot;
		}
	}
}
