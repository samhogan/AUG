using UnityEngine;
using System.Collections;

public class RockPrint : Blueprint
{
	private Sub substance;
	private ProbMeter sizeprob;

	public RockPrint(Sub sub, ProbMeter sp)
	{
		substance = sub;
		sizeprob = sp;
	}

	public override Mesh buildObject(System.Random rand)
	{
		MeshBuilder mb = new MeshBuilder();
		float size = (float)rand.NextDouble() * 4;
		ProcMesh.addCube(mb, Vector3.zero,  size, size, size, (Sub)rand.Next(30));
		return mb.getMesh();
			
	}
}
