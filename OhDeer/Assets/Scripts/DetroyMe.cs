using UnityEngine;
using System.Collections;

public class DetroyMe : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (DestroyMe ());
	}

	private IEnumerator DestroyMe(){
		yield return new WaitForSeconds(1.0f);
		Destroy(this.gameObject);
	}

}
