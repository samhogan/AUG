using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//used to generation things on the surface of a planet
//an instance of this will belong to a planet instance
public class SurfaceSystem
{

	private float radius;//the planet radius
	public int sideLength;//how many surface units are on one side of a planet face
	private float halfSide;//half the side length used more than once
	
	private static List<SurfaceUnit> surfList = new List<SurfaceUnit>();//surface units that have already been loaded
	public GameObject tree;//used for instantiation testing


	public SurfaceSystem(float r, int side, GameObject testobj)
	{
		radius = r;
		sideLength = side;
		halfSide = sideLength/2;
		tree = testobj;//test object
	}


	//a working name
	//builds all the objects in a certain surface unit
	public void CreateSurfaceObjects(SurfaceUnit su)
	{
		//only make the objects in this unit if it has not already been generated
		//and add it to the list so it is not generated again
		if(!surfList.Contains(su))
		{
			surfList.Add(su);
			for(int i = 0; i<10; i++)
			{
				for(int j = 0; j<10; j++)
				{
					//surfacepos of the tree (middle of unit)
					SurfacePos treeSurf = new SurfacePos(su.side, su.u + i/10f, su.v + j/10f);
					//convert to world unit
					Vector3 treeWorld = UnitConverter.getWP(treeSurf, radius, sideLength);
					//GameObject.Instantiate(tree, treeWorld, Quaternion.identity);
					//build the tree object(adds it to builtobjects list and maybe eventually add it to the render list
					WorldHelper.buildObject<TestTree>(treeWorld);
					//WorldHelper.buildObject<TestTree>(new Vector3(5,5,210));
				}
			}
		}

	}
}
