using UnityEngine;
using System.Collections;

public class Speed : MonoBehaviour
{

	Vector3 lastPos;
	// Use this for initialization
	void Start () 
	{
		lastPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float dist = Vector3.Distance(transform.position, lastPos);
		float metps = dist / Time.deltaTime;
		float mph = metps * 2.23694185194f;
		print(mph);

		lastPos = transform.position;
	}
}
