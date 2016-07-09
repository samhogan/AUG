using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPS : MonoBehaviour {

	Text text;

	// Use this for initialization
	void Start () {
		text = gameObject.GetComponent<Text>();
	}
	
	float deltaTime = 0.0f;

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		text.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

	}
}
