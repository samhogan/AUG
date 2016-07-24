using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RockPrint : Blueprint
{
	private Sub substance;
	private ProbMeter sizeprob;

	//can the rock only be found near its substance in the terrain
	private bool subDependent;

	//the amount of rocks in a surface unit
	private ProbMeter amountProb;

	//the amount of displacement to make the mesh less platonic
	//value [0,1] multiplied by the rock size
	private ProbMeter dispProb;

	public RockPrint(Sub sub, ProbMeter sp, bool sd, ProbMeter ap, ProbMeter dp)
	{
		substance = sub;
		sizeprob = sp;
		subDependent = sd;
		amountProb = ap;
		dispProb = dp;
	}

	public override Mesh buildObject(int seed)
	{
		System.Random rand = new System.Random(seed);

		MeshBuilder mb = new MeshBuilder();
		float size = (float)sizeprob.getValue(rand.NextDouble());
		ProcMesh.addCube(mb, Vector3.zero,  size, size, size, substance);

		ModMesh.displace(mb, size * (float)dispProb.getValue(rand.NextDouble()));

		return mb.getMesh();
			
	}

	//returns the amount of rocks in a surface unit given the subs it contains and its position
	public override int getAmount(List<Sub> subs, WorldPos pos, double random)
	{
		//if the rock is substance dependent and the substance is not present, don't build any
		if(subDependent && !subs.Contains(substance))
			return 0;

		//TODO: possibly dependent on a noise function
		return (int)amountProb.getValue(random);
	}





	/////static method that generates blueprints//
	//TODO: implement static probmeters that generate the probmeters


	public static RockPrint buildBlueprint(int seed, Sub sub)
	{
		System.Random rand = new System.Random(seed);

		//create the size probmeter(will be more complex later)
		double smallSize = rand.NextDouble() * 4;
		ProbMeter sizeprob = new ProbMeter(new double[]{smallSize,smallSize+rand.NextDouble()*4}, new double[]{1});

		//randomly choose if these rocks will be dependent on the substance(most likely)
		bool subDep = rand.NextDouble() < .5;

		//create the amount probmeter
		double smallAm = rand.NextDouble() * 4;
		ProbMeter amount = new ProbMeter(new double[]{smallAm, smallAm+rand.NextDouble()*4}, new double[]{1});


		double smallDisp = rand.NextDouble();//*.5f;
		ProbMeter disp = new ProbMeter(new double[]{ smallDisp, smallDisp }, new double[]{ 1 });


		RockPrint rp = new RockPrint(sub, sizeprob, subDep, amount, disp);
		return rp;

	}
}
