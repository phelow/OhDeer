using UnityEngine;
using System.Collections;

public class CarVision : MonoBehaviour {


	[SerializeField]
	private Car m_cv;

	private const float SPEED_RATIO = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}


	void OnTriggerExit2D(Collider2D other){
		m_cv.RemoveAvoiding (other.gameObject);
	}

	void OnTriggerStay2D(Collider2D other){
		if ((other.tag == "Player")) {
			m_cv.AddAvoiding (other.gameObject);
			m_cv.GivePoints ();
		} else if (other.tag == "Car" && Vector3.Distance (transform.position, other.transform.position) < m_cv.GetSpeed () * SPEED_RATIO) {
			if (other.GetComponent<Car>().GivesPoints ()) {
				m_cv.GivePoints ();
			}
			m_cv.AddAvoiding (other.gameObject);
		} else if (other.tag == "Car" && Vector3.Distance (transform.position, other.transform.position) > m_cv.GetSpeed () * SPEED_RATIO) {
			if (other.GetComponent<Car>().GivesPoints ()) {
				m_cv.GivePoints ();
			}
		}
	}
}
