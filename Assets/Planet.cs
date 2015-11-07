using UnityEngine;
using System.Collections;

//holds all the information for the planet including terrain and civilization
public class Planet : MonoBehaviour {

	//private RoadSystem roads;//builds the roads/civilization organization
	public TerrainSystem terrain;//builds the terrain with voxels and marching cubes
	public SurfaceSystem surface;//builds the objects that are on the planet surface

	private float radius = 200f;//radius of the planet
	private int globalSUWith = 4;//how many large street units are on one(of 6) side of the planet
	private int largeSUWith = 8;//how many mid street units are on one side of a large street
	private int midSUWith = 8;//how many base street units are on one side of a mid street unit

	private float genScale = 5f;//the perlin scale for general elevation

	public GameObject testobj;//used for position testing
	// Use this for initialization
	void OnEnable() 
	{

		MarchingCubes2.SetWindingOrder (2, 1, 0);//this will be moved later 
		MarchingCubes2.SetTarget(1);//and this

		terrain = new TerrainSystem (radius);
		surface = new SurfaceSystem (radius, 8, testobj);
		//terrain.CreateChunk(new Vector3(0f,0f,0f));

		/*for (int i = -3; i<3; i++) 
		{
			for (int j = -3; j<3; j++) 
			{
				for (int k = -3; k<3; k++) 
				{
					terrain.CreateChunk(new Vector3(i*16,j*16,k*16));
				}
			}
		}*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
