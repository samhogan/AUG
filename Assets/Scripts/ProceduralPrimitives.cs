using UnityEngine;
using System.Collections;

public class ProceduralPrimitives
{

	public static void Quad(MeshBuilder mb)
	{
		Vector3 origin = new Vector3(0, 0, 0);
		Vector3 width = new Vector3(1, 0, 0);
		Vector3 length = new Vector3(0, 0, 1);

		mb.Verts.Add(new Vector3(-1, 0, -1));//0
		mb.Verts.Add(new Vector3(-1, 0, 1));//1
		mb.Verts.Add(new Vector3(1, 0, 1));//2
		mb.Verts.Add(new Vector3(1, 0, -1));//3

		mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, 1f));

		mb.addTriangle(0, 1, 2);
		mb.addTriangle(0, 2, 3);

		//static; mb.Equals(); mb.Verts; Vector2 14534635;

	}

}
