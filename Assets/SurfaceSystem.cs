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

		//transport = new TransportSystem(8,16,8);
		transport = new TransportSystem(2,3,2);

		GameObject go = Resources.Load("Test things/rottest") as GameObject;
		Vector3 pos = UnitConverter.getWP(new SurfacePos(PSide.TOP, 0, 0), radius, sideLength);
		Quaternion rot = getWorldRot(pos, Quaternion.identity, PSide.TOP);
		GameObject.Instantiate(go, pos, rot);
	}


	//a working name
	//builds all the objects in a certain surface unit
	//or if it already exists, increase its wuCount
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

			//buildTransport(su);

			int count = rand.Next(30);

			MyDebug.placeMarker(UnitConverter.getWP(new SurfacePos(su.side, su.u, su.v), radius, sideLength));
		/*	for(int i = 0; i<count; i++)
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
			
			}*/

			GameObject go = Resources.Load("Test things/rottest") as GameObject;
			Vector3 pos = UnitConverter.getWP(new SurfacePos(su.side, su.u+0.5f, su.v+0.5f), radius, sideLength);
			Quaternion rot = getWorldRot(pos, Quaternion.identity, su.side);
		
			GameObject.Instantiate(go, pos, rot);

		}


		//increase the worldunit count of the surface holder
		sh.wuCount++;

	}

	//builds all the appropriate transportation segments in the surface unit
	private void buildTransport(SurfaceUnit su)
	{
		//find transport segments(roads) to generate
		//SurfaceUnit startTU = UnitConverter.SPtoSP(
		//surface unit to start the loop in
		
		//start and ends in the loop
		//convert the top right and bottom right corners of the su to the transport units they lie in
		int tuStartu = SUtoTU(su.u);
		int tuStartv = SUtoTU(su.v);
		int tuEndu = SUtoTU(su.u+1);
		int tuEndv = SUtoTU(su.v+1);
		
		Debug.Log(tuStartu + " " + tuStartv + " " + tuEndu + " " + tuEndv);
		
		for(int i = tuStartu; i<=tuEndu; i++)
		{
			for(int j = tuStartv; j<=tuEndv; j++)
			{
				//build the road units
				//Debug.Log("looping");
				//the trasport unit to examine
				TransportUnit tu = transport.getBase(su.side, i, j);
				if(tu.conRight)
				{
					//the transport unit to the right of this one
					TransportUnit tu2 = transport.getBase(su.side, i+1, j);
					buildTransportSegment(tu,tu2);
				}
				if(tu.conUp)
				{
					//the transport unit above this one
					TransportUnit tu2 = transport.getBase(su.side, i, j+1);
					buildTransportSegment(tu,tu2);
				}
				//if(tu.conUpRight)//add this in later
			}
		}
	}

	//builds a transport(road) segment between two transport units
	private void buildTransportSegment(TransportUnit t1, TransportUnit t2 )
	{
		//Debug.DrawLine(t1.conPointWorld, t2.conPointWorld, Color.blue, Mathf.Infinity);
		GameObject road = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//position of the segment is halfway between each point(makes sense huh?)
		road.transform.position = (t1.conPointWorld+t2.conPointWorld)/2;

		//z scale is distance between the two points
		road.transform.localScale = new Vector3(3f, 0.5f, Vector3.Distance(t1.conPointWorld, t2.conPointWorld));
		//road.transform.LookAt(t1.conPointWorld);
	}

	//converts between surface units(from surfacesystem to transportsystem) for a single value 
	//to optimize for looping in buildTransport()
	private int SUtoTU(float pos)
	{
		return Mathf.FloorToInt(pos/sideLength*transport.sideLength);
	}

	//returns the proper world rotation for an object
	private Quaternion getWorldRot(Vector3 worldPos, Quaternion surfRot, PSide side)
	{
		//initial rotation to match side(as if placing it on the side of a cube)
		Quaternion startRot;

		//the axis that the start rot will be rotated from to align with the vector from the origin to the object's position
		//top is standard side so y rotation in surfrot cooresponds to spinning left or right
		Vector3 fromRot;

		//NOTE: Euler rotation order is important, and unity does x, y, then z rotation
		switch(side)
		{
		case PSide.TOP: 
			fromRot = Vector3.up; 
			startRot = Quaternion.Euler(0,0,0);//final!
			break;
		case PSide.BOTTOM: 
			fromRot = Vector3.down; 
			startRot = Quaternion.Euler(180,0,0);
			break;
		case PSide.RIGHT:
			fromRot = Vector3.right;
			startRot = Quaternion.Euler(-90,-90,0);//could be any other combination of 90 idk
			break;
		case PSide.LEFT: 
			fromRot = Vector3.left;
			startRot = Quaternion.Euler(-90,90,0);//could be any other combination of 90 idk
			break;
		case PSide.FRONT: 
			fromRot = Vector3.forward;
			startRot = Quaternion.Euler(-90,180,0);//WHAT ORDER DO THE COMPONENTS ROTATE IN?, oh got it(refer to comment above)
			break;
		case PSide.BACK: 
			fromRot = Vector3.back;
			startRot = Quaternion.Euler(-90,0,0);
			break;
		default: //will never happen hopefully.......,......wwww
			fromRot = Vector3.zero; 
			startRot = Quaternion.Euler(0,0,0);
			break;
		}

		//Quaternion rot = Quaternion.FromToRotation(Vector3.up, worldPos);
		Quaternion rot = Quaternion.FromToRotation(fromRot, worldPos);

		return rot * startRot;

	}

	//a wrapper function for the Build class build object that also adds the objects generated by the surface system to the surfList
	//probably remove later
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
