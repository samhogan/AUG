using UnityEngine;
using System.Collections;

public class Thruster : FunctionalObject
{

    void Start()
    {
        Render();
        addRB();
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
