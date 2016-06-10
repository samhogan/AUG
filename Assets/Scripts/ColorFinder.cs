using UnityEngine;
using System.Collections;

public class ColorFinder : MonoBehaviour {

	public Texture2D text;

	// Use this for initialization
	void Start () 
	{
		float r = 76f/255;
		float g = 110f/255;
		float b = 187f/255;

		float best = 1000000;
		float bex = 0;
		float bey = 0;
		for(int x = 0; x<32; x++)
		{
			for(int y = 0; y<33; y++)
			{
				Color col = text.GetPixel(x,y);

				float rdif = Mathf.Abs(col.r-r);
				float gdif = Mathf.Abs(col.g-g);
				float bdif = Mathf.Abs(col.b-b);

				float total = rdif+gdif+bdif;
				if(total<best)
				{
					bex = x;
					bey = y;
					best = total;
				}
				//print(total);
			}
		}
			

		print(best);

		Debug.Log(bex+" "+bey);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
