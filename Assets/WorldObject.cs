using UnityEngine;
using System.Collections;

//the parent class for all objects that the player could ever interact with and are visible in the world

//just so i don't have to add them manually, but i don't think this will be inherited so who knows... these attributes are new to me
//turns out they are inherited, but not every child will use a mesh collider so hmmmmmm....
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]

public class WorldObject : MonoBehaviour
{
	protected MeshFilter filter;//the mesh filter
	protected MeshRenderer renderer;//the mesh renderer\\

	public WorldPos holdingChunk;//the chunk that the object is inside

	//sets the references to the filter and renderer
	public void setReferences()
	{
		filter = gameObject.GetComponent<MeshFilter>();
		renderer = gameObject.GetComponent<MeshRenderer>();
		
		Material mat = Resources.Load("TestMaterial") as Material;//loads the default material, will remove this
		renderer.material = mat;
	}

	public virtual void Render(){}//renders the object(builds the meshes and such)

	//public virtual void init(){};//this is basically the constructor for worldObjects, it will set the properties of the object

	//public virtual void organize(){}
}
