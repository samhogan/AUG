using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//a node is a connector on a functional object that can connect to another node and transfer electricity, data, materials, etc.
public class Node
{
    //a (probably) temporary reference to all nodes for checking connections
    public static List<Node> curNodes = new List<Node>();


    //the position of this node relative to its object
    public Vector3 position;

    //the rotation relative to its object
    public Quaternion rotation;

    //a reference to the node that is connected to this one
    public Node connected;

    //is the node transporting anything (electricity, material, data)
    private bool isActive;
    public virtual bool Active
    {
        get { return isActive; }
        set { isActive = value; }
    }

    //a reference to the gameobject that this is attached to
    public GameObject go;

    public Node(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }

    public static void connectNodes(Node n1, Node n2)
    {
        n1.connected = n2;
        n2.connected = n1;

        FixedJoint joint = n1.go.AddComponent<FixedJoint>();
        joint.connectedBody = n2.go.GetComponent<Rigidbody>();
    }

}
