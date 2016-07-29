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
    private Quaternion relRot;

    //is the curObject a functional object
    //private bool isFunctional;

    //the functional component of the cur object if it has one
    private FunctionalObject fo;

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
                pickupItem();
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
            reposObject();
        }
        //lastRot = transform.rotation.eulerAngles;
    }

    //calculates the new rotation and position of the curObject
    void reposObject()
    {
        //calculate the target position and rotation of the curbody
        Vector3 targetPos = transform.position + transform.forward * 2;
        //curBody.AddForce((destination - curBody.position)*5, ForceMode.Force);
        Quaternion targetRot = transform.rotation * relRot;

        if(isFunctional())
        {
            print("yes...");

            foreach(Node node in fo.nodes)
            {
                //the position of where the node should be in unity space
                Vector3 nodePos = targetPos + targetRot * node.position;

                foreach(Node otherNode in Node.curNodes)
                {

                    if(otherNode.go != curObject)//and the nodes are compatible
                    {
                        //the position of the other node in unity space (object pos + 
                        Vector3 otherNodePos = otherNode.go.transform.position + otherNode.go.transform.rotation * otherNode.position;

                        if(Vector3.Distance(nodePos, otherNodePos) < 1)
                        {
                            targetPos += new Vector3(0, 1, 0);
                        }

                    }
                }
                //if(node.position*targetRot)
            }

        }

        //interpolate to the target pos/rot
        curBody.velocity = (targetPos - curBody.position) * 10;
        //add the camera rot and target rot
        curBody.rotation = targetRot;
    }

    //attempts to pick up an item straigth ahead
    void pickupItem()
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
        relRot = curObject.transform.localRotation;
        curObject.transform.parent = null;

        fo = curObject.GetComponent<FunctionalObject>();

        //curBody.drag = 100;
       
    }

    //drops the current item
    void dropItem()
    {
        curBody.useGravity = true;
        curBody = null;
        curObject = null;
        fo = null;
    }

    bool isFunctional()
    {
        return fo != null;
    }
}
