using UnityEngine;
using System.Collections;

public class CarVision : MonoBehaviour {

	private float m_fieldOfViewAngle = 90;
	[SerializeField]
	private float m_fieldOfView;

	[SerializeField]
	private Car m_cv;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void OnTriggerStay2D(Collider2D other){
		Vector3 direction = other.transform.position - transform.position;
		float angle = Mathf.Atan2 (direction.x, direction.y)*Mathf.Rad2Deg - 90;
		float fov;

		if (other.tag == "Player") {
			fov = m_fieldOfView;
		} else {
			fov = m_fieldOfView/4;
		}
		if (angle >= 0 && angle < fov) {
			m_cv.SetTurning(Car.currentState.TURNING_LEFT);
		} else if (angle < 0 && angle > -fov) {
			m_cv.SetTurning(Car.currentState.TURNING_RIGHT);

		} else {
			m_cv.SetTurning(Car.currentState.PURSUING_WAYPOINT);
		}
	}
}
