using UnityEngine;
using System.Collections;

public class ProcMesh
{

	//adds a triangle given the verticies
	public static void addTri(MeshBuilder mb, Vector3 v0, Vector3 v1, Vector3 v2, Sub sub)
	{
		mb.addVertex(v0, sub);
		mb.addVertex(v1, sub);
		mb.addVertex(v2, sub);

		//the start index of the vertices of this triangle in the meshbuilder
		int si = mb.Verts.Count - 3;

		mb.addTriangle(si, si+1, si+2);

	}


	//adds a quad given four verticies
	public static void addQuad(MeshBuilder mb, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Sub sub)
	{
		addTri(mb, v0, v1, v2, sub);
		addTri(mb, v0, v2, v3, sub);
	}


	/// <summary>
	/// Adds a quad to the mesh
	/// </summary>
	/// <param name="mb">mesh builder.</param>
	/// <param name="origin">Origin.</param>
	/// <param name="widthDir">Width direction</param>
	/// <param name="lengthDir">Length direction</param>
	/// <param name="sub">Substance</param>
	public static void addQuad(MeshBuilder mb, Vector3 origin, Vector3 widthDir, Vector3 lengthDir, Sub sub)
	{
		//Vector3 origin = new Vector3(0, 0, 0);
		//Vector3 width = new Vector3(1, 0, 0);
		//Vector3 length = new Vector3(0, 0, 1);

		//divide each in two so the origin is at the center of this quad
		widthDir*=.5f;
		lengthDir*=.5f;

		addQuad(mb, origin-widthDir-lengthDir,
					origin-widthDir+lengthDir,
					origin+widthDir+lengthDir,
					origin+widthDir-lengthDir, sub);
		//static; mb.Equals(); mb.Verts; Vector2 14534635;

	}
		
	//builds a right rectangular prism
	public static void addCube(MeshBuilder mb, Vector3 origin, float width, float length, float height, Sub sub)//also add rotation quaternion
	{
		Vector3 widthDir = Vector3.right*width;
		Vector3 lengthDir = Vector3.forward*length;
		Vector3 heightDir = Vector3.up*height;
		addCuboid(mb, origin, widthDir, lengthDir, heightDir, sub);
	}

	//builds a polyhedron made of 6 quadrilaterals
	public static void addCuboid(MeshBuilder mb, Vector3 origin, Vector3 widthDir, Vector3 lengthDir, Vector3 heightDir, Sub sub)
	{
		addQuad(mb, origin+heightDir*.5f, widthDir, lengthDir, sub);
		addQuad(mb, origin-heightDir*.5f, -widthDir, lengthDir, sub);
		addQuad(mb, origin-lengthDir*.5f, widthDir, heightDir, sub);
		addQuad(mb, origin+lengthDir*.5f, -widthDir, heightDir, sub);
		addQuad(mb, origin+widthDir*.5f, lengthDir, heightDir, sub);
		addQuad(mb, origin-widthDir*.5f, -lengthDir, heightDir, sub);

	}

}
