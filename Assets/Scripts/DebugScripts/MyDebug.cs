using UnityEngine;
using System.Collections;


//my debug class
public class MyDebug : MonoBehaviour {

	public static void placeMarker(Vector3 pos, float scale)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.transform.position = pos;
		go.transform.localScale = new Vector3(scale, scale, scale);
	}
	public static void placeMarker(Vector3 pos)
	{
		placeMarker(pos, 1);
	}
}
