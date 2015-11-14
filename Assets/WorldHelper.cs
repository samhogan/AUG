using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Just a misc class that world related things that don't go anywhere else will be added
public class WorldHelper
{
	//builds a specific object and adds it to the appropriate lists(rendered later)
	//NOTE: type T MUST derive from WorldObject, nvmd added in constraint
	public static void buildObject<T>(Vector3 pos) where T : WorldObject
	{
		GameObject go = new GameObject();
		go.name = "figure out this naming thing sam";

		//set the objects position
		go.transform.position = pos;
		//add the component that is the type passed in the parameter(the type of object)
		WorldObject wo = go.AddComponent<T>() as WorldObject;
		Debug.Log("Tree built at " + pos);

		WorldPos wp = UnitConverter.toWorldPos(pos);
		Debug.Log("Tree is in worldpos " + wp);

		//add the object to the request system object dictionary
		//NOTE: this can easily be condensed but I am afraid it will lose readabilty
		List<WorldObject> refList = null;//reference to the worldobjcet list in the dictionary
		if(RequestSystem.builtObjects.TryGetValue(wp, out refList))
		{
			//add this world object to the returned list
			refList.Add(wo);
		} 
		else//there is no worldpos key(first object in its cube), add one
		{
			refList = new List<WorldObject>();//create a new list
			RequestSystem.builtObjects.Add(wp, refList);//add it to the dictionary at the new worldpos key
			Debug.Log("WorldPos added to builtobjects " + wp);
			refList.Add(wo);
			Debug.Log("Tree added to builtObjects " + wp);
		}

		/*Debug.Log(wp + " " + RequestSystem.requestedChunks.Contains(wp));
		for(int i = 0; i<RequestSystem.requestedChunks.Count; i++)
		{
			Debug.Log("Worldpos " + i + ": " + RequestSystem.requestedChunks[i]);
		}*/
		//check if the object is ready to be rendered
		if(RequestSystem.requestedChunks.Contains(wp))
		{
			Debug.Log("An object was added to the render list");
			RequestSystem.objectsToRender.Add(wo);
		}
		//wo.Render();

	}

}
