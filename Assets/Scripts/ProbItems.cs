using UnityEngine;
using System.Collections;

public class ProbItems : Probability 
{

	public ProbItems(double[] its, double[] weights) : base(its, weights)
	{

	}

	//you can omit the items to have whole numbers returned(0,1,2...)
	public ProbItems(double[] weights): base(null, weights)
	{
		
	}

	//percent must be between 0 and 1(inclusive)
	public override double getValue(double percent)
	{
		Debug.Assert(percent>=0 && percent<=1);

		//percent of total
		double frac = percent*total;

		for(int i = 0; i<cumulativeWeights.Length; i++)
		{
			if(cumulativeWeights[i]>=frac)
			if(items!=null)
				return items[i];
			else
				return i;
		}

		//idunno why not?
		return double.PositiveInfinity;
	}
}
