using UnityEngine;
using System.Collections;

public class Seat : FunctionalObject
{
    public Rigidbody player;

    void Start()
    {
        Render();
        addRB();
    }

    void Update()
    {
        if(Vector3.Distance(player.position, transform.position) < 2 && Input.GetKey("j"))
        {
            player.gameObject.transform.position = transform.position + transform.up * 1.1f;

            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = player;
            /*HingeJoint joint = gameObject.AddComponent<HingeJoint>();

            joint.axis = transform.up;
            joint.connectedBody = player;*/
            PlayerMove1.sitting = true;

            player.constraints = RigidbodyConstraints.None;
        }
    }


    public override void Render()
    {
        MeshBuilder mb = new MeshBuilder();
        ProcMesh.addCube(mb, Vector3.zero, 1f, 1f, .1f, Sub.TEST);
        setMeshCol(mb.getMesh());

        addNode<NeutralNode>(mb, new Vector3(0, -.05f, 0), Quaternion.Euler(180, 0, 0));
        setMesh(mb.getMesh());
    }
}
