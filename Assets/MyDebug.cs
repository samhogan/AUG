using UnityEngine;
using System.Collections;


//my debug class
public class MyDebug : MonoBehaviour {

	public static void placeMarker(Vector3 pos)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.transform.position = pos;
	}
}
