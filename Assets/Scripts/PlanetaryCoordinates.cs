using UnityEngine;
using System.Collections;

public class PlanetaryCoordinates : CoordinateSystem
{

    public CoordinateSystem(double su, GameObject track):base(su, track)
    {
        SU = su;
        tracker = track;
    }

    public override void update()
    {
        //calculate the pos based on the player's position in unityspace
        pos.x = floatingOrigin.x + (long)(tracker.transform.position.x * SUperUU);
        pos.y = floatingOrigin.y + (long)(tracker.transform.position.y * SUperUU);
        pos.z = floatingOrigin.z + (long)(tracker.transform.position.z * SUperUU);

        //if the origin needs updating, update it and shift everything
        if(originNeedsUpdate(pos, floatingOrigin))
        {
            //calculate where the origin should now be
            LongPos newOrigin = calcOrigin(pos);

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
            // if(curPlanet != null)
            foreach(KeyValuePair<LODPos, TerrainObject> chunk in UniverseSystem.curPlanet.lod.chunks)
                if(chunk.Key.level <= LODSystem.uniCutoff)
                    chunk.Value.gameObject.transform.position += shift;


            //ship.transform.position += shift;
            //also shift the player(should eventually not have to do this)
            //if(!Ship.playerOn)
            tracker.transform.position += shift;

            //now update the origin
            floatingOrigin = newOrigin;
        }

    }

}
