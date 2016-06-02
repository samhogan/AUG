using UnityEngine;
using System.Collections;

public abstract class Probability 
{
	protected double total;
	protected double[] cumulativeWeights;
	protected double[] items;

	public Probability(double[] its, double[] weights)
	{
		//foreach(int weight in weights)
		//	total+=weight;

		items = its;

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
