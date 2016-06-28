using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarchingSquares 
{

	public static float surface = .5f;


	public static MeshBuilder buildMesh(float[,] voxVals)
	{
		MeshBuilder mb = new MeshBuilder();

		int l = voxVals.GetLength(0), w = voxVals.GetLength(1);


		//array of voxels
		SVox[,] voxels = new SVox[l,w];


		//create all the voxel objects
		for(int x = 0; x < l; x++)
		{
			for(int y = 0; y < w; y++)
			{
				voxels[x, y] = new SVox(x, y, voxVals[x, y] < surface);
			}
		}

		//fill in all voxel edge data
		for(int x = 0; x < l; x++)
		{
			for(int y = 0; y < w; y++)
			{
				//if the voxel is not the rightmost one, calculate its right edge
				if(x < l - 1)
				{
					voxels[x, y].xEdge = voxels[x, y].pos + new Vector3(.5f, 0, 0);
				}

				if(y < w - 1)
				{
					voxels[x, y].yEdge = voxels[x, y].pos + new Vector3(0, .5f, 0);
				}
			}
		}


		//finally, march the squares
		//there are 1 fewer squares than verts/voxels in each dimension
		for(int x = 0; x < l-1; x++)
		{
			for(int y = 0; y < w-1; y++)
			{
				marchSquare(mb, voxels[x,y], voxels[x+1, y], voxels[x, y+1], voxels[x+1, y+1]);
			}
		}

		return mb;

	}

	//build the mesh of the square made up of these four voxels
	private static void marchSquare(MeshBuilder mb, SVox v1, SVox v2, SVox v3, SVox v4)
	{
		//calculate the cell type
		int cellType = 0;
		if(v1.state)
			cellType |= 1;
		if(v2.state)
			cellType |= 2;
		if(v3.state)
			cellType |= 4;
		if(v4.state)
			cellType |= 8;

		//now build the mesh based on the cell type
		switch(cellType)
		{
		case 0:
			return;
		case 1:
			ProcMesh.addTri(mb, v1.pos, v1.yEdge, v1.xEdge, Sub.Foyaite);
			return;
		case 15:
			ProcMesh.addQuad(mb, v1.pos, v3.pos, v4.pos, v2.pos, Sub.Mud);
			return;
		default:
			return;
		}

	}


	//the square voxel class
	public class SVox 
	{
		public bool state;
		public Vector3 pos;
		public Vector3 xEdge, yEdge;

		public SVox(int x, int y, bool s)
		{
			pos = new Vector3(x, y, 0);
			state = s;
		}
	}


}


