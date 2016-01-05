using UnityEngine;
using System.Collections;

public class CarVision : MonoBehaviour {


	[SerializeField]
	private Car m_cv;

	private const float SPEED_RATIO = 2.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}


	void OnTriggerExit2D(Collider2D other){
		m_cv.SetTurning(Car.currentState.PURSUING_WAYPOINT);
		if (other.tag == "Car") {
			m_cv.StopBreaking ();
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if ((other.tag == "Player")|| other.tag == "Car" && Vector3.Distance(transform.position,other.transform.position) < m_cv.GetSpeed() * SPEED_RATIO) {
			float dir = Car.LeftRightTest (other.transform.position, transform.position, transform.forward, transform.up);

			if (other.tag == "Car") {
				m_cv.StartBreaking ();
			}

			//right - positive, left - negative

			if (dir >= 0) {
				m_cv.SetTurning (Car.currentState.TURNING_LEFT);
			} else if (dir < 0) {
				m_cv.SetTurning (Car.currentState.TURNING_RIGHT);
			} 
		}
	}
}
