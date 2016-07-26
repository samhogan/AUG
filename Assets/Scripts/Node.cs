using UnityEngine;
using System.Collections;

//a node is a connector on a functional object that can connect to another node and transfer electricity, data, materials, etc.
public class Node
{
    //the position of this node relative to its object
    public Vector3 position;

    //the rotation relative to its object
    public Quaternion rotation;

    public Node(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }

}
