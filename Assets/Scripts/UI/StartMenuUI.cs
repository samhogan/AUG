using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour {

	public Canvas main;
	public Canvas controls;
	public Canvas loading;

	// Use this for initialization
	void Start () 
	{
		controls.enabled = false;
		loading.enabled = false;
	}
	

	public void playPress()
	{
		main.enabled = false;
		loading.enabled = true;
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

	public void exitPress()
	{
		Application.Quit();
	}

}
