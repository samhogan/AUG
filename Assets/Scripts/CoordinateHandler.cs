using UnityEngine;
using System.Collections;

public class CoordinateHandler : MonoBehaviour
{
    public GameObject player;


    public static CoordinateSystem planetSpace;


    public static Planet curPlanet;

	void Start ()
    {
        planetSpace = new CoordinateSystem(0.0001, player);

        initialPositioning();

	}

    //sets up the initial state of every coordinate system
    void initialPositioning()
    {

        curPlanet = new Planet(300000, new LongPos(0,0,0), 3241);
        UniverseSystem.curPlanet = curPlanet;

        Vector3 startPoint = Random.onUnitSphere;
        startPoint *= curPlanet.noise.getAltitude(startPoint) + 3;


        //calculate the player floating position 
        player.transform.position = planetSpace.getFloatingPos(startPoint);

        planetSpace.update();

        player.GetComponent<GravityController>().gravity = curPlanet.gravity;

    }
	
	// Update is called once per frame
	void Update() {
        planetSpace.update();
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
