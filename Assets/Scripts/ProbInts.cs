using UnityEngine;
using System.Collections;

public class ProbInts //: Probability
{
	
	public ProbInts(double[] weights)
	{

	}

	//percent must be between 0 and 1(inclusive)
	/*public override double getValue(double percent)
	{
		Debug.Assert(percent>=0 && percent<=1);

		//percent of total
		double frac = percent*total;

		for(int i = 0; i<cumulativeWeights.Length; i++)
		{
			if(cumulativeWeights[i]>=frac)
				return i;
		}

		//idunno why not?
		return double.PositiveInfinity;
	}*/

}
