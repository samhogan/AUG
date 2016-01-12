using UnityEngine;
using System.Collections;

//the camera that captures unispace from the player's perspective
public class UniCam : MonoBehaviour 
{

	//the player's camera to track
	public GameObject playerCam;

	// Use this for initialization
	void Start () 
	{

	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.rotation = playerCam.transform.rotation;
	}
}
