using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	//TODO: implement a main menu

	// Use this for initialization
	void Start () {
		SceneManager.LoadScene (Levels.GAMEPLAY);
	}
}
