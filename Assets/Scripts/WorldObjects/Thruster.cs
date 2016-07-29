using UnityEngine;
using System.Collections;

public class Thruster : FunctionalObject
{

    void Start()
    {
        Render();
        addRB();
    }

    void Update()
    {
        if(nodes[0].Active)
            rb.AddForce(transform.up*-100, ForceMode.Acceleration);

       // print(nodes[0] != null);
        //print(nodes[0].connected != null);
        //print(nodes[0].connected.Active);

    }

    public override void Render()
    {
        MeshBuilder mb = new MeshBuilder();
        ProcMesh.addCube(mb, Vector3.zero, .5f, .5f, 1, Sub.Hawaiite);
        setMeshCol(mb.getMesh());

        addNode<PowerInNode>(mb, new Vector3(0, -.5f, 0), Quaternion.Euler(180, 0, 0));
        setMesh(mb.getMesh());
    }
}
