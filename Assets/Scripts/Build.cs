using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Just a misc class that world related things that don't go anywhere else will be added
//Build was formerly WorldHelper
public class Build
{
	//builds a specific object and adds it to the appropriate lists(rendered later)
	//NOTE: type T MUST derive from WorldObject, nvmd added in constraint
	//NOTE: pos and rot might be moved to the init function of each object later possibly
	public static T buildObject<T>(Vector3 pos, Quaternion rot) where T : WorldObject
	{
		//can instantiate multiple empty gameobjects per frame because i read they have little performance impact
		//this creates an empty gameobject and intantiates it into the unity game world
		GameObject go = new GameObject();

		//go.SetActive(false);
		//Make its name in the inspector its type name
		go.name = typeof(T).ToString();

		//calculate the floating position(around the world origin)
		Vector3 floatPos = PositionController.getPlanetFloatingPos(pos);

		//set the objects position
		go.transform.position = floatPos;
		go.transform.rotation = rot;

		//add the component that is the type passed in the parameter(the type of object)
		WorldObject wo = go.AddComponent<T>() as WorldObject;

		//Debug.Log("Tree built at " + pos);

		//gets the chunk that the world object is in
		WorldPos wp = UnitConverter.getChunk(pos);
		wo.holdingChunk = wp;
		//Debug.Log("Tree is in worldpos " + wp);

		//add to all appropriate lists
		organizeObject(wo, wp);
		//wo.Render();

		return wo as T;//this is so the objects can be initialized like this: WorldHelper.buildObject<TestTree>().init(5000.5f);
	}

	//this will add the object to the appropriate position in the builtObjects list and possibly to the render list
	private static void organizeObject(WorldObject wo, WorldPos wp)
	{
		//add the object to the request system object dictionary
		//NOTE: this can easily be condensed but I am afraid it will lose readabilty
		List<WorldObject> refList = null;//reference to the worldobjcet list in the dictionary
		if(RequestSystem.builtObjects.TryGetValue(wp, out refList))
		{
			//add this world object to the returned list
			//refList.Add(wo);
		} 
		else//there is no worldpos key(first object in its cube), add one
		{
			refList = new List<WorldObject>();//create a new list
			RequestSystem.builtObjects.Add(wp, refList);//add it to the dictionary at the new worldpos key
			//Debug.Log("WorldPos added to builtobjects " + wp);
			//refList.Add(wo);
			//Debug.Log("Tree added to builtObjects " + wp);
		}
	
		//if the object being added is a terrain object, add it to the front of the list so it is rendered first
		//NOTE: may change this later to move every terrain to front of render list rather than front of buildObject list
		if(wo is TerrainObject)
			refList.Insert(0, wo);
		else
			refList.Add(wo);
		//check if the object is ready to be rendered
		if(RequestSystem.requestedChunks.Contains(wp))
		{
			//Debug.Log("An object was added to the render list");
			RequestSystem.objectsToRender.Add(wo);
		}


	}

	public static void destroyObject(WorldObject wo)
	{
		//if its in the render list, remove it
		if(RequestSystem.objectsToRender.Contains(wo))
			RequestSystem.objectsToRender.Remove(wo);

		List<WorldObject> refList = null;
		RequestSystem.builtObjects.TryGetValue(wo.holdingChunk, out refList);//this list certainly exists if there is an object in it
		refList.Remove(wo);//remove it from builtObjects

		//if that was the last object in the list, remove the list from builtObjects
		if(refList.Count == 0)
			RequestSystem.builtObjects.Remove(wo.holdingChunk);

		//finally, destroy the gameobject
		//NOTE: eventually the object will be pooled and reused
		Object.Destroy(wo.gameObject);
	}

}
