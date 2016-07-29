using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//functional objects are 'man made' and contain nodes that give them some functionality
public class FunctionalObject : MobileObjects
{
    //a list of the nodes on this object
    public List<Node> nodes = new List<Node>();

    //adds a node to the mesh and to the node list
    protected void addNode<T>(MeshBuilder mb, Vector3 pos, Quaternion rot) where T : Node
    {
        //move the mesh out just a bit to prevent z fighting
        //maybe add the node to the list first then change this
        pos += rot*Vector3.up*.01f;


        //set colors of nodes
        //TODO: change this 
        Sub sub = Sub.Foyaite;
        if(typeof(T) == typeof(PowerInNode))
            sub = Sub.Vegitation1;
        

        // ProcMesh.addCube(mb, pos, .4f, .4f, .1f, Sub.Foyaite, rot);
        ProcMesh.addQuad(mb, pos, rot * Vector3.right *.4f, rot * Vector3.forward*.4f, sub);

        //create the node
        Node node;
        if(typeof(T) == typeof(PowerInNode))
            node = new PowerInNode(pos, rot) { go = this.gameObject };
        else
            node = new PowerOutNode(pos, rot) { go = this.gameObject };

        nodes.Add(node);
        Node.curNodes.Add(node);

    }

   
}
