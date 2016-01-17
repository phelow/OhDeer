using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	private IEnumerator m_playerKillingCoroutine = null;
	private const float DEATH_ANIMATION_TIME = 2.0f;

	[SerializeField]
	private Text m_scoreText;

	private int m_totalPoints = 0;

	public void GainPoints(int points)
	{
		m_totalPoints += points;
		m_scoreText.text = ""+m_totalPoints;
	}
	public int GetScore(){
		return m_totalPoints;
	}
	public void KillPlayer()
	{
		if (m_playerKillingCoroutine == null) {
			m_playerKillingCoroutine = KillPlayerCoroutine ();
			StartCoroutine (m_playerKillingCoroutine );
		}
	}

	private IEnumerator KillPlayerCoroutine()
	{
		PlayerPrefs.SetInt ("YourScore", m_totalPoints);
		Destroy (this.gameObject);
		ExplosionMaster.Explode (transform.position);
		//TODO: include code for animation here
		yield return new WaitForSeconds (DEATH_ANIMATION_TIME);
		SceneManager.LoadScene (Levels.GAME_OVER_LEVEL);
	}


}
