using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarchingSquares 
{

	public static float surface = 1f;


	public static MeshBuilder buildMesh(float[,] voxVals, Sub[,] subs)
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
				//if the voxel is not the rightmost one and the edge will be used(different states), calculate its right edge
				if(x < l - 1 && (voxels[x,y].state^voxels[x+1,y].state))
				{
					float percent = .5f;
					//if they are not the same, use linear interpolation to find the percent
					if(voxVals[x, y] != voxVals[x+1, y])
						percent = (surface - voxVals[x, y]) / (voxVals[x+1, y] - voxVals[x, y]);

					voxels[x, y].xEdge = voxels[x, y].pos + new Vector3(percent, 0, 0);
				}

				if(y < w - 1 && (voxels[x,y].state^voxels[x,y+1].state))
				{
					float percent = .5f;
					//if they are not the same, linear interpolation
					if(voxVals[x, y] != voxVals[x, y+1])
						percent = (surface - voxVals[x, y]) / (voxVals[x, y+1] - voxVals[x, y]);
					voxels[x, y].yEdge = voxels[x, y].pos + new Vector3(0, percent, 0);
				}
			}
		}


		//finally, march the squares
		//there are 1 fewer squares than verts/voxels in each dimension
		for(int x = 0; x < l-1; x++)
		{
			for(int y = 0; y < w-1; y++)
			{
				marchSquare(mb, voxels[x,y], voxels[x+1, y], voxels[x, y+1], voxels[x+1, y+1], subs[x,y]);
			}
		}

		return mb;

	}

	//build the mesh of the square made up of these four voxels
	private static void marchSquare(MeshBuilder mb, SVox v1, SVox v2, SVox v3, SVox v4, Sub sub)
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

		//Sub sub = Sub.Foyaite;
		//now build the mesh based on the cell type
		switch(cellType)
		{
		case 0:
			return;
		case 1:
			ProcMesh.addTri(mb, v1.pos, v1.yEdge, v1.xEdge, sub);
			return;
		case 2:
			ProcMesh.addTri(mb, v2.pos, v1.xEdge, v2.yEdge, sub);
			return;
		case 3:
			ProcMesh.addQuad(mb, v1.pos, v1.yEdge, v2.yEdge, v2.pos, sub);
			return;
		case 4:
			ProcMesh.addTri(mb, v3.pos, v3.xEdge, v1.yEdge, sub);
			return;
		case 5:
			ProcMesh.addQuad(mb, v1.pos, v3.pos, v3.xEdge, v1.xEdge, sub);

			return;
		case 6:
			ProcMesh.addTri(mb, v3.pos, v3.xEdge, v1.yEdge, sub);
			ProcMesh.addTri(mb, v2.pos, v1.xEdge, v2.yEdge, sub);
			//ProcMesh.addQuad(mb, v1.yEdge, v3.xEdge, v2.yEdge, v1.xEdge, Sub.Limestone);
			return;
		case 7:
			ProcMesh.addTri(mb, v1.pos, v3.pos, v2.pos, sub);
			ProcMesh.addQuad(mb, v3.pos, v3.xEdge, v2.yEdge, v2.pos, sub);
			return;
		case 8:
			ProcMesh.addTri(mb, v4.pos, v2.yEdge, v3.xEdge, sub);
			return;
		case 9:
			ProcMesh.addTri(mb, v1.pos, v1.yEdge, v1.xEdge, sub);
			ProcMesh.addTri(mb, v4.pos, v2.yEdge, v3.xEdge, sub);
			//ProcMesh.addQuad(mb, v1.yEdge, v3.xEdge, v2.yEdge, v1.xEdge, Sub.Limestone);
			return;
		case 10:
			ProcMesh.addQuad(mb, v2.pos, v1.xEdge, v3.xEdge, v4.pos, sub);
			return;
		case 11:
			ProcMesh.addTri(mb, v1.pos, v4.pos, v2.pos, sub);
			ProcMesh.addQuad(mb, v1.pos, v1.yEdge, v3.xEdge, v4.pos, sub);
			return;
		case 12:
			ProcMesh.addQuad(mb, v3.pos, v4.pos, v2.yEdge, v1.yEdge, sub);
			return;
		case 13:
			ProcMesh.addTri(mb, v1.pos, v3.pos, v4.pos, sub);
			ProcMesh.addQuad(mb, v1.pos, v4.pos, v2.yEdge, v1.xEdge, sub);
			return;
		case 14:
			ProcMesh.addTri(mb, v3.pos, v4.pos, v2.pos, sub);
			ProcMesh.addQuad(mb, v3.pos, v2.pos, v1.xEdge, v1.yEdge, sub);
			return;
		case 15:
			//ProcMesh.addQuad(mb, v1.pos, v3.pos, v4.pos, v2.pos, sub);//Sub.TEST);
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


