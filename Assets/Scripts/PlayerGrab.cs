using UnityEngine;
using System.Collections;

public class PlayerGrab : MonoBehaviour
{
    //the object being held
    private GameObject curObject;
    private Rigidbody curBody;


	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(curObject == null)
            {
                RaycastHit hitInfo;
                Physics.Raycast(transform.position, transform.forward, out hitInfo, 5f);
                curBody = hitInfo.rigidbody;
                curBody.useGravity = false;
                curObject = hitInfo.rigidbody.gameObject;

            }
        }
	}

    void FixedUpdate()
    {
        if(curObject != null)
        {
            //curBody.position = transform.position + transform.forward * 2;
            Vector3 destination = transform.position + transform.forward * 2;
            //curBody.AddForce((destination - curBody.position)*5, ForceMode.Force);
            curBody.velocity = (destination - curBody.position) * 10;
        }
    }
}
