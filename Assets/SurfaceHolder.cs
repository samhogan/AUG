using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//holds all items in a certain surface unit and how many generated world units are in it
public class SurfaceHolder 
{
	public List<WorldObject> objects;
	public int wuCount;//the number of world units that are within the surface unit's domain (used for deletion)

	public SurfaceHolder()
	{
		objects = new List<WorldObject>();
		wuCount = 0;
	}
}
