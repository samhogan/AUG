using UnityEngine;
using System.Collections;

public class Gyroscope : FunctionalObject
{

    void Start()
    {
        Render();
        addRB();
        //rb.angularDrag = 100000000;
    }

    // Update is called once per frame
    void Update ()
    {
        //rb.angularVelocity = Vector3.zero;
        if(nodes[0].Active)
        {
            //rb.constraints = RigidbodyConstraints.FreezeRotation;
            //rb.angularDrag = 
        }
        if(Input.GetKey("a"))
        {
            //rb.rotation *= Quaternion.Euler(transform.right*20);
            
        }
	}


    public override void Render()
    {
        MeshBuilder mb = new MeshBuilder();
        ProcMesh.addCube(mb, Vector3.zero, .5f, .5f, .5f, Sub.Vegitation2);
        setMeshCol(mb.getMesh());
        addNode<PowerInNode>(mb, new Vector3(.25f, 0, 0), Quaternion.Euler(0, 0, -90));
        setMesh(mb.getMesh());


    }
}
