using UnityEngine;
using System.Collections;

public class ExplosionMaster : MonoBehaviour {
	[SerializeField]
	private GameObject m_explosion;

	private static ExplosionMaster instance;

	void Start(){
		instance = this;
	}


	// Use this for initialization
	public static void Explode(Vector3 explode){
		instance.StartCoroutine (instance.Explosion (explode));
	}

	private IEnumerator Explosion(Vector3 explode){
		int nExplosions = Random.Range (1, 10);
		GameObject go2 = GameObject.Instantiate (instance.m_explosion);
		go2.transform.position = explode;
		for (int i = 0; i < nExplosions; i++) {
			yield return new WaitForSeconds (Random.Range (0.0f, .5f));
			GameObject go = GameObject.Instantiate (instance.m_explosion);
			go.transform.position = new Vector3 (explode.x + Random.Range (-1.0f, 1.0f), explode.y + Random.Range (-1.0f, 1.0f), explode.z);
		}
	}
}
