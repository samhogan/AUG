using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoordinateSystem
{
    //the number of scaled units per unity unit in every coordinate system
    public const int SUperUU = 10000;

    //the length of a single scaled unit in meters
    protected readonly double SU;

    //the current position of the camera/player in this coordinate system
    protected LongPos pos;
    //the point in this coordinate system that marks the origin of the unity space, moves when the player is out of range
    protected LongPos floatingOrigin;

    //the number of unity units that must be exceeded in unity space for the objects to be shifted back
    protected const int UUThreshold = 10000;

    //the number of scaled units that must be exceeded in this coordinate system for the objects to be shifted back
    protected const int SUThreshold = UUThreshold * SUperUU;

    //the number of scaled units that must be exceeded for the entire coordinate system to shifted in its parent coordinate system
    protected readonly long referenceThreshold = 100000000;

    //a referece to the player/tracker and its camera for the base planetary system
    protected GameObject tracker, camera;

    //the coordinate system that is nested inside this one
    CoordinateSystem child;
    //the celestial object that 
    //Planet childBodyReferece;
    //the point in this coordinate system that serves as the origin of the child system
    LongPos childRef;



    //the ratio between this system's su and the child system's su
    //mulitply to convert su to child su
    int SUtoChildSU;

    //static references to the current bodies, null if not connected
    public static Planet curPlanet;
    public static StarSystem curSystem;

    public CoordinateSystem(double su, GameObject track, GameObject cam)
    {
        SU = su;
        tracker = track;
        camera = cam;
    }

    public CoordinateSystem(double su, GameObject track, GameObject cam, CoordinateSystem ch): this(su, track, cam)
    {
        child = ch;
        SUtoChildSU = (int)(SU / child.SU);
    }


    //updates the pos, floating origin, child system shift
    public virtual void update()
    {
       // Debug.Log("sup");
        //first calculate the new position and update the child system if needed
        if(!childBodyDependent())
        {
            pos = childRef + child.pos / SUtoChildSU;
      
            if(childOriginNeedsUpdate())
                updateChildRef();
            Debug.Log("idk");
        }
        else//we are on a planet
        {
            //the planet origin is the origin
            pos = child.getBodyReference().scaledPos + child.pos / SUtoChildSU;
            
        }

        //now update the tracker and floating origin
        updateTracker();
        updateTrackerRotation();    
    }

    //rotates the tracker camera based on the child camera
    void updateTrackerRotation()
    {
        camera.transform.rotation = child.camera.transform.rotation;
    }

    //updates the poition of tracker and shifts it with everything else if out of the floating threshold
    protected virtual void updateTracker()
    {
        //if the stellar tracker/stellar pos is outside the threshold of the origin, recalculate the origin and shift everything back
        if(originNeedsUpdate(pos, floatingOrigin))
        {
            floatingOrigin = calcOrigin(pos);

            //shift all gameobjects
            //if(getBodyReference()!=null)
                shiftItems();
        }
        //move the stellar tracker to its appropriate position
        tracker.transform.position = getFloatingPos(pos);
    }

    //shifts the child coordinate system
    void updateChildRef()
    {
        childRef = calcReferenceOrigin();
        child.pos = (pos - childRef) * SUtoChildSU;
        updateTracker();
    }

    //shifts all the gameObjects in this system when the floating origin changes
    protected virtual void shiftItems()
    {

    }

    //returns if the child coordinate system is centered on a body(planet, star system, galaxy)
    protected bool childBodyDependent()
    {
        return child.getBodyReference()!=null;
    }

    protected virtual Planet getBodyReference()
    {
        return null;
    }

    //checks if the current pos is far away enough from the floating origin that it needs shifting/updating
    //can probably make this non static
    public static bool originNeedsUpdate(LongPos pos, LongPos origin)
    {
        return System.Math.Abs(pos.x - origin.x) > SUThreshold || System.Math.Abs(pos.y - origin.y) > SUThreshold || System.Math.Abs(pos.z - origin.z) > SUThreshold;
    }

    //calculates the floating origin of a space given the current position of the player/tracker in that space
    //it just rounds to the nearest 1000/whatever threshold is
    public LongPos calcOrigin(LongPos pos)
    {

        return new LongPos(roundToNearest(pos.x, SUThreshold * 2),
            roundToNearest(pos.y, SUThreshold * 2),
            roundToNearest(pos.z, SUThreshold * 2));

    }


    //checks if the pos of the child system is large enough to shift the entire system
    public bool childOriginNeedsUpdate()
    {
        return System.Math.Abs(child.pos.x) > referenceThreshold ||
            System.Math.Abs(child.pos.y) > referenceThreshold ||
            System.Math.Abs(child.pos.z) > referenceThreshold;
    }

    //calculates the origin of the child system in this system
    public LongPos calcReferenceOrigin()
    {
        return new LongPos(roundToNearest(pos.x, referenceThreshold*2/SUtoChildSU),
            roundToNearest(pos.y, referenceThreshold * 2 / SUtoChildSU),
            roundToNearest(pos.z, referenceThreshold * 2 / SUtoChildSU));

    }


    //rounds number "num" to the nearest "nearest"
    long roundToNearest(long num, long nearest)
    {
        bool negative = false;
        if(num < 0)
        {
            num *= -1;
            negative = true;
        }

        //first round down
        long rounded = num / nearest * nearest;

        //if the remaining portion is greater than half of nearest, round up
        if(num % nearest >= nearest / 2)
            rounded += nearest;

        //if it was originally negative
        if(negative)
            rounded *= -1;

        return rounded;

    }

    //takes a longpos and returns the floating pos in unity space
    public Vector3 getFloatingPos(LongPos _pos)
    {
        //finds relative position in su, divides by 10000 to convert to uu, converts to v3
        return ((_pos - floatingOrigin).toVector3() / SUperUU);
    }

    //later either won't need this or will use a double precision vector
    //takes a vector3 in uu and returns a floating pos vector3 in uu 
    public Vector3 getFloatingPos(Vector3 _pos)
    {
        //finds relative position in su, divides by 10000 to convert to uu, converts to v3
        return _pos - (floatingOrigin.toVector3() / SUperUU);
    }


    //converts scaled units to unity units
    public static Vector3 SUtoUU(LongPos pos)
    {
        return pos.toVector3() / SUperUU;
    }

    //get the real position of the player in unity units, 
    public Vector3 getRealPos()
    {
        return SUtoUU(pos);
    }
}

public enum spaces { Planetary = 9, Stellar = 10, Galactic = 11, Universal = 12 };