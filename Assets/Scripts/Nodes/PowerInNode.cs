using UnityEngine;
using System.Collections;

public class PowerInNode : Node
{

    public PowerInNode(Vector3 pos, Quaternion rot) : base(pos, rot)
    {

    }
    //cannot be set, only active if the connected node is active
    public override bool Active
    {
        get
        {

            if(connected != null)
                return connected.Active;
            return false;
        }
    }

}
