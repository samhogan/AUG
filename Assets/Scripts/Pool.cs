using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Pool
{

	private static List<GameObject> pooledTerrain = new List<GameObject>();


	public static GameObject getTerrain()
	{
		//if no terrain pooled, make some
		if(pooledTerrain.Count == 0)
		{
			GameObject go = new GameObject();
			go.AddComponent<TerrainObject>();
			return go;
		}
		else
		{
			
			GameObject go = pooledTerrain[0];
			go.SetActive(true);
			pooledTerrain.RemoveAt(0);
			return go;
		}
		//later do a clever look to see if this terrain is the same or something like that maybe not that would get complicated with multiplayer and stuff
	}


	public static void deleteTerrain(TerrainObject obj)
	{
		obj.reset();
		obj.gameObject.SetActive(false);
		obj.gameObject.name = "pooled terrain";
		pooledTerrain.Add(obj.gameObject);
		//GameObject.Destroy(obj.gameObject);
	}


}
