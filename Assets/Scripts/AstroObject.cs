using UnityEngine;
using System.Collections;

//this encompasses all macrostuctures in the universe such as planets, stars, star, systems, and galaxies
public class AstroObject
{
    //the position of this astronomical object in its respective space
    public LongPos scaledPos;

    //the gameobject that represents this object in its space
    public GameObject scaledRep;

    public Quaternion Rotation
    {
        get{return scaledRep.transform.rotation; }
        
    }

    public AstroObject()
    {
    }
	
}
