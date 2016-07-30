using UnityEngine;
using System.Collections;

public class VehicleBase : FunctionalObject
{


    void Start()
    {
        Render();
        addRB();
    }


    void Update()
    {
        if(PlayerMove1.sitting)
        {
            nodes[0].Active = Input.GetKey("space");
            nodes[1].Active = Input.GetKey("g");

        }
    }

    void FixedUpdate()
    {
        if(PlayerMove1.sitting)
        {
            float pitch = Input.GetAxisRaw("Vertical");
            float yaw = Input.GetAxisRaw("Horizontal");
            // float throttle = Input.GetButton("Jump") ? (isHyper ? hyperSpeed : speed) : 0.0f;

            float roll = Input.GetKey("q") ? 1 : Input.GetKey("e") ? -1 : 0;

            rb.angularVelocity = Vector3.zero;
            //rb.velocity = Vector3.zero;

            rb.AddRelativeTorque(pitch * -70, yaw * 70, roll * 70, ForceMode.Acceleration);
            //transform.Rotate(0, right, forward);
            //rb.AddForce(transform.forward * throttle, ForceMode.Force);


            if(pitch == 0 && yaw == 0 && roll == 0)
                rb.angularVelocity = Vector3.zero;
        }
    }




    //don't ask why I capitalized this method but don't follow convention for anything else
    //okay fine I'll tell you, you might want to get some popcorn because it's story time
    //Many believe the Render method to be a fluke, an accident, the result of an experiment gone horribly wrong.
    //None of these are in fact the truth at all. The mysterious Render method is simply a remnant of a class long lost in the git archive
    //from many projects ago...
    //It started many years in the past when a young boy was on a quest to unlock the secrets of cyberspace and the world of game development
    //He had an idea, a starting point to a game he would enjoy: an infinite world
    //this would give him a map to let his imagination run free with creation
    //There was just one problem: He did not know how to do it.
    //The boy decided to consult an online tutorial explaining how to create a voxel based world represented by cubes
    //This mighty tutorial offered the boy something that few souls have encountered and many believe to be a myth
    //They call it the Chunk class
    //lengend has it that the Chunk class contained infinite knowledge of world generation in game development
    //It's countless methods were more beautiful, more elegant, and more efficient than the greatest minds in computer science could ever imagine
    //It is believed that one of these methods was simply named Render
    //The great class was there for the boy's taking, but he knew he would have much work ahead of him in understanding its holy contents
    //He spent days and weeks exploring the unknown, learning more than he thought possible
    //But one day, something changed.
    //to be continued i need to get back to work

    public override void Render()
    {
        MeshBuilder mb = new MeshBuilder();
        ProcMesh.addCube(mb, Vector3.zero, 3, 5, .5f, Sub.Gold);
        setMeshCol(mb.getMesh());
        addNode<PowerOutNode>(mb, new Vector3(0, 0, -2.5f), Quaternion.Euler(-90, 0, 0));
        addNode<PowerOutNode>(mb, new Vector3(0, 0, 2.5f), Quaternion.Euler(90, 0, 0));
        addNode<NeutralNode>(mb, new Vector3(0, .25f, 0), Quaternion.identity);

        setMesh(mb.getMesh());

    }


}
