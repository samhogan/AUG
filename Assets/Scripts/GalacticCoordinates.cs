using UnityEngine;
using System.Collections;

public class GalacticCoordinates : CoordinateSystem
{

    public GalacticCoordinates(double su, GameObject track, GameObject cam, CoordinateSystem ch) : base(su, track, cam, ch)
    {

    }

    protected override void shiftItems()
    {
        //shift everything in stellar space
        foreach(StarSystem system in curGalaxy.systems)
            system.scaledRep.transform.position = getFloatingPos(system.scaledPos);
      
    }

    //returns the current star system if it exists
    protected override AstroObject getBodyReference()
    {
        return curGalaxy;
    }

    protected override void checkVoid()
    {
        // Debug.Log(LongPos.Distance(pos, curPlanet.scaledPos) + " " + curPlanet.atmosRadius);
       /* if(LongPos.Distance(pos, curPlanet.scaledPos) > curPlanet.atmosRadius)
        {
            curPlanet = null;
            updateChildRef();
        }*/
    }

    protected override void checkBodies()
    {
       /* foreach(Planet plan in curSystem.planets)
        {
            if(LongPos.Distance(pos, plan.scaledPos) < plan.atmosRadius)
            {
                curPlanet = plan;
                enterBody();
                break;
            }
        }*/
    }

}
