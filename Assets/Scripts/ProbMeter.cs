using UnityEngine;
using System.Collections;

//the probmeter is my master creation and if i understood it, i would explain it to you
public class ProbMeter : Probability
{
	public ProbMeter(double[] vals, double[] weights) : base(vals, weights)
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
			{
				//good luck
				double lowerWeightBound = i>0 ? cumulativeWeights[i-1] : 0;
				double perBet = (frac-lowerWeightBound)/(cumulativeWeights[i]-lowerWeightBound);
				double valDist = values[i+1]-values[i];
				return values[i]+valDist*perBet;
			}
		}

		//idunno why not?
		return double.PositiveInfinity;
	}
}
