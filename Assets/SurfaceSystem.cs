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
	private float suLength;//the length of one side of a surface unit in world units

	//the transport system that holds road data and stuff like that
	//will eventually be a list of potentially multiple or no transport systems
	private TransportSystem transport;

	//private static List<SurfaceUnit> surfList = new List<SurfaceUnit>();//surface units that have already been loaded

	//dictionary of all 
	private static Dictionary<SurfaceUnit, SurfaceHolder> surfList = new Dictionary<SurfaceUnit, SurfaceHolder>();
	//public GameObject tree;//used for instantiation testing


	public SurfaceSystem(float r, int side)
	{
		radius = r;
		sideLength = side;
		halfSide = sideLength/2;
		suLength = (2 * r * Mathf.PI) / (sideLength * 4);//circumference divided by number of sus around the sphere cross section

		transport = new TransportSystem(4,8,8);
	}


	//a working name
	//builds all the objects in a certain surface unit
	public void CreateSurfaceObjects(SurfaceUnit su)
	{

		//creates an empty surface holder
		SurfaceHolder sh = null;

		//only make the objects in this unit if it has not already been generated
		//and add it to the list so it is not generated again
		if(!surfList.TryGetValue(su, out sh))
		{
			sh = new SurfaceHolder();

			surfList.Add(su, sh);
			//instance a random number generator with seed based on the su position sent through a hash function
			//NOTE: the last 1 parameter is used as a kind of planet identifier, but this may not be needed
			System.Random rand = new System.Random((int)WorldManager.hash.GetHash(su.u, su.v, (int)su.side, 1));

			//NOTE: all objects that are in the rect list are always in the radial list
			//create a list of radial collisions
			//(x,y,radius)
			List<RadialCol> radCols = new List<RadialCol>();
			//create a list of rectangular collisions
			//List<Vector4> rectCols = new List<Vector4>();
			//first add all roads, then buildings, then natural things

			//find transport segments(roads) to generate
			//SurfaceUnit startTU = UnitConverter.SPtoSP(
			//surface unit to start the loop in

			//start and ends in the loop
			//NOTE: make this into a funtion later
			int tuStartu = Mathf.FloorToInt((float)su.u/sideLength*transport.sideLength);
			int tuStartv = Mathf.FloorToInt((float)su.v/sideLength*transport.sideLength);
			int tuEndu = Mathf.FloorToInt(((float)su.u+1)/sideLength*transport.sideLength);
			int tuEndv = Mathf.FloorToInt(((float)su.v+1)/sideLength*transport.sideLength);

			Debug.Log(tuStartu + " " + tuStartv + " " + tuEndu + " " + tuEndv);

			for(int i = tuStartu; i<tuEndu; i++)
			{
				for(int j = tuStartv; j<tuEndv; j++)
				{
					//build the road units
					//Debug.Log("looping");
					//the trasport unit to examine
					TransportUnit tu = transport.getBase(su.side, i, j);
					if(tu.conRight)
					{
						//connecting point of the transport unit
						SurfacePos conPoint1 = new SurfacePos(su.side,i+tu.conPoint.x,j+tu.conPoint.y);//this will be shortened later by making the conpoint global instead of local
						//the transport unit to the right of this one
						TransportUnit tu2 = transport.getBase(su.side, i+1, j);
						// connecting point of the other transport unit
						SurfacePos conPoint2 = new SurfacePos(su.side,i+1+tu2.conPoint.x,j+tu2.conPoint.y);
						//world position of the transport units' connecting points
						Vector3 worldConPoint1 = UnitConverter.getWP(conPoint1, radius, transport.sideLength);
						Vector3 worldConPoint2 = UnitConverter.getWP(conPoint2, radius, transport.sideLength);

						//Debug.Log(worldConPoint1 + " " + worldConPoint2);

						Debug.DrawLine(worldConPoint1, worldConPoint2, Color.blue, Mathf.Infinity);
						//Debug.Log("drawing................");
					}
					if(tu.conUp)
					{

					}
				}
			}

			int count = rand.Next(30);

			for(int i = 0; i<count; i++)
			{
				//Vector3 pos = new Vector3(
				//choose random x and y position within the su
				float u = (float)rand.NextDouble();
				float v = (float)rand.NextDouble();

				//temp radius of tree used for testing
				float wuRadius = 2;
				//radius in world units/length of a surface unit = radius in surface units(less than 1)
				float suRadius = wuRadius/suLength;
				//Debug.Log("suRadius is " + suRadius);

			    bool isColliding = false;
				foreach(RadialCol oth in radCols)
				{
					//distance formula(move to struct later)
					//if the distance between the two centers - their radii is less than zero, they are colliding
					if(Mathf.Sqrt((oth.u-u)*(oth.u-u)+(oth.v-v)*(oth.v-v))-suRadius-oth.radius<0)
					{
						isColliding = true;
						//Debug.Log("samwell");
						break;
					}
				}
				//for the time being, if something is colliding, just discard it
				//later it may be moved slightly or completely repositioned
				if(isColliding)
				{
					continue;
				}

				//add this obect to the radial collision list
				//later, create the RadialCol object initially(replace x y and suRadius)
				radCols.Add(new RadialCol(u,v,suRadius));

				//surfacepos of the tree
				SurfacePos treeSurf = new SurfacePos(su.side, su.u + u, su.v + v);
				//SurfacePos treeSurf = new SurfacePos(su.side, su.u + (float)rand.NextDouble(), su.v + (float)rand.NextDouble());
				//convert to world unit
				Vector3 treeWorld = UnitConverter.getWP(treeSurf, radius, sideLength);
				//GameObject.Instantiate(tree, treeWorld, Quaternion.identity);
				//build the tree object(adds it to builtobjects list and maybe eventually add it to the render list
				buildObject<TestTree>(treeWorld, sh).init();
				//WorldHelper.buildObject<TestTree>(new Vector3(5,5,210));
			
			}
		}


		//increase the worldunit count of the surface holder
		sh.wuCount++;

	}



	//a wrapper function for the Build class build object that also adds the objects generated by the surface system to the surfList
	private WorldObject buildObject<T>(Vector3 pos, SurfaceHolder sh) where T : WorldObject
	{
		WorldObject wo = Build.buildObject<T>(pos);

		sh.objects.Add(wo);//add it to the surface holder list


		return wo;
	}

	//deletes all obects in a surface unit if no more world units are in it
	public void deleteSurface(SurfaceUnit su)
	{
		SurfaceHolder sh = null;
		surfList.TryGetValue(su, out sh);
		sh.wuCount--;

		//if there are no more world units that exist in the surface unit, delete the surface unit and all objects in it
		if(sh.wuCount == 0)
		{
			foreach(WorldObject wo in sh.objects)
			{
				Build.destroyObject(wo);
			}
			surfList.Remove(su);
		}
	}

}
