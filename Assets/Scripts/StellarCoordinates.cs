using UnityEngine;
using System.Collections;

public class StellarCoordinates : CoordinateSystem {

    public StellarCoordinates(GameObject track, GameObject cam, CoordinateSystem ch) : base(1, track, cam, ch)
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
        if(LongPos.Distance(pos, curPlanet.scaledPos) > curPlanet.atmosRadius)
        {
            leaveBody();
        }
    }
}
