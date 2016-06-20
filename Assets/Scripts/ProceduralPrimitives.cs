using UnityEngine;
using System.Collections;

public class ProceduralPrimitives
{

	public static void addQuad(MeshBuilder mb, Vector3 origin, Vector3 widthDir, Vector3 lengthDir, Sub sub)
	{
		//Vector3 origin = new Vector3(0, 0, 0);
		//Vector3 width = new Vector3(1, 0, 0);
		//Vector3 length = new Vector3(0, 0, 1);

		//divide each in two so the origin is at the center of this quad
		widthDir/=2;
		lengthDir/=2;

		mb.addVertex(origin-widthDir-lengthDir, sub);//0
		mb.addVertex(origin-widthDir+lengthDir, sub);//1
		mb.addVertex(origin+widthDir+lengthDir, sub);//2
		mb.addVertex(origin+widthDir-lengthDir, sub);//3

	/*	mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, .6f));
		mb.UVs.Add(new Vector2(.5f, 1f));*/

		//the start index of the vertices of this quad in the meshbuilder
		int si = mb.Verts.Count - 4;

		mb.addTriangle(si, si+1, si+2);
		mb.addTriangle(si, si+2, si+3);

		//static; mb.Equals(); mb.Verts; Vector2 14534635;

	}
		

	public static void addCube(MeshBuilder mb, Vector3 origin, Vector3 widthDir, Vector3 lengthDir, Vector3 heightDir, Sub sub)
	{
		addQuad(mb, origin+heightDir*.5f, widthDir, lengthDir, sub);
		addQuad(mb, origin-heightDir*.5f, -widthDir, lengthDir, sub);
		addQuad(mb, origin-lengthDir*.5f, widthDir, heightDir, sub);
		addQuad(mb, origin+lengthDir*.5f, -widthDir, heightDir, sub);
		addQuad(mb, origin+widthDir*.5f, lengthDir, heightDir, sub);
		addQuad(mb, origin-widthDir*.5f, -lengthDir, heightDir, sub);

	}

}
