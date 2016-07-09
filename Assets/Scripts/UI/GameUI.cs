using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameUI : MonoBehaviour {

	public static GameUI ui;

	public Text seedText;
	public Text messageText;
	private float messageAlpha = 0;

	// Use this for initialization
	void Awake() 
	{
		ui = this;
	}
	
	// Update is called once per frame
	void Update () {
		if(messageAlpha > -.1)
		{
			messageAlpha -= Time.deltaTime;
			//if(messageAlpha < 1)
			messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, messageAlpha);
		}
	}

	public void exitPress()
	{
		Application.Quit();
	}

	public void setSeedText(int seed)
	{
		seedText.text = seed.ToString();
	}

	public void displayMessage(string text)
	{
		messageText.text = text;
		messageAlpha = 3;
		//messageText.
	}
}
