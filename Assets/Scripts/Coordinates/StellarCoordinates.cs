using UnityEngine;
using System.Collections;

public class StellarCoordinates : CoordinateSystem {

    public StellarCoordinates(double su, GameObject track, GameObject cam, CoordinateSystem ch) : base(su, track, cam, ch)
    {

    }

    protected override void shiftItems()
    {
        //shift everything in stellar space
        foreach(Planet plan in curSystem.planets)
            plan.scaledRep.transform.position = getFloatingPos(plan.scaledPos);
        curSystem.star.scaledRep.transform.position = getFloatingPos(curSystem.star.scaledPos);
    }

    //returns the current star system if it exists
    protected override AstroObject getBodyReference()
    {
        return curSystem;// curSystem;
    }

    protected override void checkVoid()
    {
       // Debug.Log(LongPos.Distance(pos, curPlanet.scaledPos) + " " + curPlanet.atmosRadius);
        if(LongPos.Distance(pos, curPlanet.scaledPos) > curPlanet.atmosRadius)
        {
            curPlanet = null;
            updateChildRef();
        }
    }

    protected override void checkBodies()
    {
        foreach(Planet plan in curSystem.planets)
        {
            if(LongPos.Distance(pos, plan.scaledPos) < plan.atmosRadius)
            {
                curPlanet = plan;
                UniverseSystem.curPlanet = plan;//TODO: c'mon sam
                enterBody();
                break;
            }
        }
    }
}
