using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//holds the info for everything to load and loads it!
//there is only one loader that loads everything - all planets, terrain, and objects
//there will also be a loadhelper monobehavior script that attaches to objects that load stuff around them(player and physics objects)

//NOTE: create means instantiate an object and load all of its properties. build means actually create the object's mesh in the unity world
public class Loader : MonoBehaviour
{
	private static List<WorldObject> renderList = new List<WorldObject>();//the list that will contain all objects to be rendered(meshes built/displayed), may need a better system later
	private static List<WorldObject> objList = new List<WorldObject>();//the list that will contain all objects already built
	//public List<Vector3> builtPos;//contains all the positions of objects already built so they will not be again (assumes two objects do not have the same position)
	//public List<Vector3> builtTer;//contains all the positions of objects already built so they will not be again (assumes two objects do not have the same position)

	void Update()
	{
		//renders max 1 object per frame and adds it to the object list
		if (renderList.Count > 0) 
		{
			renderList[0].Render();
			objList.Add(renderList[0]);
			renderList.RemoveAt(0);
		}
	}

	//adds an object to the render list so it will be rendered when ready
	public static void addToRender(WorldObject obj)
	{
		renderList.Add (obj);
	}


	//this method is flawed and needs to be changed
	public static void removeObj(Vector3 pos)
	{
		for(int i = objList.Count; i>=0; i--)
		{
			float distance = Vector3.Distance(objList[i].transform.position, pos);

			//if the object is 100 units away from the player, delete it
			if(distance>100)
			{
				Object.Destroy(objList[i].gameObject);
				return;
			}
		}
	}
}
