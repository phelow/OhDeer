using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour {
	[SerializeField]
	private Text m_highScore;

	[SerializeField]
	private Text m_yourScore;
	// Use this for initialization
	void Start () {
		StartCoroutine (HighScoreMenu());
	}

	public IEnumerator HighScoreMenu()
	{
		m_highScore.text = "High Score:" + PlayerPrefs.GetInt ("HighScore",0);
		m_yourScore.text = "Your Score:" + PlayerPrefs.GetInt ("YourScore");

		if (PlayerPrefs.GetInt ("HighScore") < PlayerPrefs.GetInt ("YourScore")) {
			PlayerPrefs.SetInt ("HighScore",PlayerPrefs.GetInt ("YourScore"));
		}
			
		yield return new WaitForSeconds (2.0f);
		SceneManager.LoadScene (Levels.MAIN_MENU);

	}
}