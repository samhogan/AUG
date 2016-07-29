using UnityEngine;
using System.Collections;

public class PlayerGrab : MonoBehaviour
{
    //the object being held
    private GameObject curObject;
    private Rigidbody curBody;

    //the rotation of the camera in the previous fixed update
    //private Vector3 lastRot;

    //the rotation of the curObject at pickup relative to the camera
    private Quaternion targetRot;

    // Use this for initialization
    void Start ()
    {
        //lastRot = transform.rotation.eulerAngles;
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

                //targetRot = Quaternion.Euler(transform.rotation.eulerAngles - curObject.transform.rotation.eulerAngles);//curBody.rotation * Quaternion.Inverse(transform.rotation);
                //WHAT A HACK!!
                //I am completely confused why my attempted mathematical implementation of this failed
                //TODO: fix this, although not top priority
                //my guess is that it will never get fixed ever, today's date is 7/28/16
                curObject.transform.parent = transform;
                targetRot = curObject.transform.localRotation;
                curObject.transform.parent = null;




            }
            else
            {
                dropItem();
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

            //this does not work very well
            //TODO: fix it
            //curBody.angularVelocity = transform.rotation.eulerAngles - lastRot;
            // curBody.angularVelocity = Vector3.zero;

            //add the camera rot and target rot
            curBody.rotation = transform.rotation * targetRot;
            //Vector3 destRot = 
        }
        //lastRot = transform.rotation.eulerAngles;
    }

    //drops the current item
    void dropItem()
    {
        curBody.useGravity = true;
        curBody = null;
        curObject = null;
    }
}
