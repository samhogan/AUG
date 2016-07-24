using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetaryCoordinates : CoordinateSystem
{

    //temporary
    GameObject ship;

    public PlanetaryCoordinates(double su, GameObject player, GameObject cam, GameObject sh):base(su, player, cam)
    {
        ship = sh;
    }

    public override void update()
    {
        //calculate the pos based on the player's position in unityspace
        pos.x = floatingOrigin.x + (long)(tracker.transform.position.x * SUperUU);
        pos.y = floatingOrigin.y + (long)(tracker.transform.position.y * SUperUU);
        pos.z = floatingOrigin.z + (long)(tracker.transform.position.z * SUperUU);

        //Debug.Log(pos + " " + floatingOrigin);

        //if the origin needs updating, update it and shift everything
        if(originNeedsUpdate(pos, floatingOrigin))
        {
            //Debug.Log("origin updating.");
            //calculate where the origin should now be
            LongPos newOrigin = calcOrigin(pos);

            //shift all items if on a planet
            if(curPlanet != null)
                shiftItems(newOrigin);


            //ship.transform.position += shift;
            //also shift the player(should eventually not have to do this)
            //if(!Ship.playerOn)
           // tracker.transform.position = getFloatingPos(pos);

            //now update the origin
            floatingOrigin = newOrigin;

            if(!Ship.playerOn)
                tracker.transform.position = getFloatingPos(pos);
            else
                ship.transform.position = getFloatingPos(pos);
        }

    }

    void shiftItems(LongPos newOrigin)
    {
        //calcualte how much to shift the worldobjects
        Vector3 shift = ((floatingOrigin - newOrigin) / SUperUU).toVector3();
        foreach(KeyValuePair<WorldPos, List<WorldObject>> objectList in RequestSystem.builtObjects)
        {
            foreach(WorldObject wo in objectList.Value)
            {
                wo.transform.position += shift;
            }
        }

        //move all terrain objects that are in planetary space (if the player is on a planet
        foreach(KeyValuePair<LODPos, TerrainObject> chunk in UniverseSystem.curPlanet.lod.chunks)
            if(chunk.Key.level <= LODSystem.uniCutoff)
                chunk.Value.gameObject.transform.position += shift;



        //Debug.Log(getFloatingPos(pos) + " " + (tracker.transform.position + shift));

        if(!Ship.playerOn)
            ship.transform.position += shift;

       

       // if(!Ship.playerOn)
         //   tracker.transform.position = getFloatingPos(pos);*/

    }


    //updates the poition of player based on the pos
    //only called when exit/leaving planet and shifting the full coordinate system
    protected override void updateTracker()
    {
        //if the pos is outside the threshold of the origin, recalculate the origin and shift everything back
        if(originNeedsUpdate(pos, floatingOrigin))
             floatingOrigin = calcOrigin(pos);

        //move the player tracker to its appropriate position
        //tracker.transform.position = getFloatingPos(pos);
        ship.transform.position = getFloatingPos(pos);
    }


    protected override AstroObject getBodyReference()
    {
        return curPlanet;
    }

}
