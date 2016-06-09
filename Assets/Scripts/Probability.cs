using UnityEngine;
using System.Collections;

public abstract class Probability 
{
	protected double total;//total of weights
	protected double[] cumulativeWeights;
	protected double[] values;


	public double[] Values
	{
		set{values = value;}
	}


	public Probability(double[] vals, double[] weights)
	{
		//foreach(int weight in weights)
		//	total+=weight;

		values = vals;

		total = 0;
		cumulativeWeights = new double[weights.Length];
		for(int i=0; i<weights.Length; i++)
		{
			total += weights[i];
			cumulativeWeights[i] = total;
			//Debug.Log(cumulativeWeights[i]);
		}


	}


	public abstract double getValue(double percent);
}
