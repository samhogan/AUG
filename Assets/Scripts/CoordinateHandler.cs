using UnityEngine;
using System.Collections;

public class CoordinateHandler : MonoBehaviour
{
    public GameObject player, stellarTracker;
    public GameObject playerCam, stellarCam;
    public GameObject ship;


    public static PlanetaryCoordinates planetSpace;
    public static StellarCoordinates stellarSpace;

    

	void Start ()
    {
        planetSpace = new PlanetaryCoordinates(player, playerCam, ship);
        stellarSpace = new StellarCoordinates(stellarTracker, stellarCam, planetSpace);

        initialPositioning();

	}

    //sets up the initial state of every coordinate system
    void initialPositioning()
    {

        CoordinateSystem.curPlanet = new Planet(300000, new LongPos(0,0,0), 3241);
        UniverseSystem.curPlanet = CoordinateSystem.curPlanet;

        Vector3 startPoint = Random.onUnitSphere;
        startPoint *= CoordinateSystem.curPlanet.noise.getAltitude(startPoint) + 3;


        //calculate the player floating position 
        player.transform.position = planetSpace.getFloatingPos(startPoint);

        planetSpace.update();
        stellarSpace.update();

        player.GetComponent<GravityController>().gravity = CoordinateSystem.curPlanet.gravity;

        //position the ship
        Vector3 shipStart = startPoint + new Vector3(1, 1, 1) * 20;
        shipStart = shipStart.normalized * (CoordinateSystem.curPlanet.noise.getAltitude(shipStart) + 1);

        ship.transform.position = planetSpace.getFloatingPos(shipStart);

        ship.transform.rotation = Quaternion.LookRotation(shipStart);
        ship.transform.rotation *= Quaternion.Euler(90, 0, 0);


    }
	
	// Update is called once per frame
	void Update() {
        planetSpace.update();
        stellarSpace.update();
	}


   

    //returns the chunk that the player is in for terrain generation
    public static WorldPos getChunk()
    {
        Vector3 uSpace = planetSpace.getRealPos();
        return new WorldPos(Mathf.FloorToInt(uSpace.x / Generator.chunkSize) * Generator.chunkSize,
            Mathf.FloorToInt(uSpace.y / Generator.chunkSize) * Generator.chunkSize,
            Mathf.FloorToInt(uSpace.z / Generator.chunkSize) * Generator.chunkSize);
    }
}
