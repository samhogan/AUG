using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//a series of funcions for modifying meshes
public class ModMesh 
{

	//randomly moves around the verts
	public static void displace(MeshBuilder mb, float amount)
	{
		//create a vertex dictionary enumerating redundant verts so they will be moved together
		Dictionary<Vector3, List<int>> verts = new Dictionary<Vector3, List<int>>();
		for(int i=0; i<mb.Verts.Count; i++)
		{

			List <int> iList;
			//if the key is already in the dictionary(vert duplicate), add the vert index to that list
			if(verts.ContainsKey(mb.Verts[i]))
				iList = verts[mb.Verts[i]];
			else
			{
				iList = new List<int>();
				verts.Add(mb.Verts[i], iList);
			}

			iList.Add(i);
		}


		foreach(KeyValuePair<Vector3, List<int>> entry in verts)
		{
			//displace the vert position
			Vector3 disp = new Vector3(Random.value*amount, Random.value*amount, Random.value*amount) + entry.Key;

			//update all the verts
			foreach(int index in entry.Value)
			{
				mb.Verts[index] = disp;
			}

		}

	}




}
