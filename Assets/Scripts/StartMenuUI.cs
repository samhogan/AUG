using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour {

	public Canvas main;
	public Canvas controls;

	// Use this for initialization
	void Start () 
	{
		controls.enabled = false;
	}
	

	public void playPress()
	{
		SceneManager.LoadScene("GameBackup2", LoadSceneMode.Single);
	}

	public void contPress()
	{
		main.enabled = false;
		controls.enabled = true;
	}

	public void backPress()
	{
		main.enabled = true;
		controls.enabled = false;
	}

}
