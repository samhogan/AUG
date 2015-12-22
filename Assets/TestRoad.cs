using UnityEngine;
using System.Collections;
//used for testing road generation, may be used full time eventually
public class TestRoad : MobileObjects
{
	private float length;

	public void init(float l)
	{
		length = l;
	}
	
	/*void OnEnable()//onenable makes these references immediately after being created instead of in the next frame
	{
		setReferences();
		
		//coll = gameObject.GetComponent<MeshCollider>();
		
	}*/
	
	public override void Render()
	{
		GameObject goInst = GameObject.CreatePrimitive(PrimitiveType.Cube);
		goInst.transform.localScale = new Vector3(0.7f, 0.5f, length);
		//GameObject goInst = GameObject.Instantiate(go, transform.position, Quaternion.identity) as GameObject;
		goInst.transform.parent = transform;
		goInst.transform.localRotation = Quaternion.identity;//make sure this child obj has 0 rotation relative to the parent
		goInst.transform.localPosition = Vector3.zero;
	}
	

}
