using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TitleController : MonoBehaviour {

	Text text;

	float r,g,b;

	// Use this for initialization
	void Start () 
	{
		text = gameObject.GetComponent<Text>();
		r = 1;
		g = 1;
		b = 1;
	}


	// Update is called once per frame
	void Update () {
	
		/*float val = Random.value/3;
		text.color = new Color(.5f+val,.5f+val,1);
		text.fontSize = Random.Range(142, 148);*/

		if(r > 1)
			r = 1;
		else if(r < 0)
			r = 0;
		if(g > 1)
			g = 1;
		else if(g < 0)
			g = 0;
		if(b > 1)
			b = 1;
		else if(b < 0)
			b = 0;

		r += Random.value*.05f - .025f;
		g += Random.value*.05f - .025f;
		b += Random.value*.05f - .025f;

		text.color = new Color(r, g, b);
	}
}
