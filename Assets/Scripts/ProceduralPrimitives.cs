using UnityEngine;
using System.Collections;

public class ProceduralPrimitives
{

	public static void Quad(MeshBuilder mb, Vector3 origin, Vector3 widthDir, Vector3 lengthDir, Sub sub)
	{
		//Vector3 origin = new Vector3(0, 0, 0);
		//Vector3 width = new Vector3(1, 0, 0);
		//Vector3 length = new Vector3(0, 0, 1);

		//divide each in two so the origin is at the center of this quad
		widthDir/=2;
		lengthDir/=2;

		mb.addVertex(-widthDir-lengthDir, sub);//0
		mb.addVertex(-widthDir+lengthDir, sub);//1
		mb.addVertex(widthDir+lengthDir, sub);//2
		mb.addVertex(widthDir-lengthDir, sub);//3

	/*	mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, 1f));*/

		mb.addTriangle(0, 1, 2);
		mb.addTriangle(0, 2, 3);

		//static; mb.Equals(); mb.Verts; Vector2 14534635;

	}
		

}
