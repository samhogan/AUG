using UnityEngine;
using System.Collections;

//world objects that can be moved (everything except terrain objects probably, maybe planets later. the full project structure is unknown at this point)
[RequireComponent(typeof(MeshCollider))]
public class MobileObjects : WorldObject 
{
	protected MeshCollider meshCollider;
	protected Rigidbody rb;
 

    public override void setReferences()
	{
		base.setReferences();
		meshCollider = gameObject.GetComponent<MeshCollider>();
	}

	public void setMesh(Mesh m)
	{
		filter.mesh = m;
    }

    public void setMeshCol(Mesh m)
    {
        meshCollider.sharedMesh = m;
    }

    //creates a rigid body for the object
    public void addRB()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        meshCollider.convex = true;
    }

}
