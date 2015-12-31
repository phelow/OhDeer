using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

	[SerializeField]
	private float m_maximumSpeed;

	[SerializeField]
	private float m_acceleration;

	[SerializeField]
	private float m_breakPower;

	[SerializeField]
	private float m_turnRate;

	[SerializeField]
	private float m_fieldOfView;

	[SerializeField]
	private Rigidbody2D m_rigidbody;

	[SerializeField]
	private Transform m_transform;

	private float m_fieldOfViewAngle = 90;

	private enum currentState
	{
		MOVING_FORWARD,
		TURNING_LEFT,
		TURNING_RIGHT
	}

	private currentState m_curState = currentState.MOVING_FORWARD;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		// Determine if there's a deer ahead and if it is to the left or the right

		// Determine if the deer is within breaking distance
	}

	void OnTriggerStay2D(Collider2D other){
		Vector3 direction = other.transform.position - transform.position;
		float angle = Mathf.Atan2 (direction.x, direction.y)*Mathf.Rad2Deg - 90;
		Debug.Log (angle + " " + other.gameObject.name );
		float fov;


		if (other.tag == "Player") {
			fov = m_fieldOfView;

		} else {
			fov = m_fieldOfView/4;
		}
		if (angle >= 0 && angle < fov) {
			m_curState = currentState.TURNING_LEFT;
		} else if (angle < 0 && angle > -fov) {
			m_curState = currentState.TURNING_RIGHT;

		} else {
			m_curState = currentState.MOVING_FORWARD;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		m_curState = currentState.MOVING_FORWARD;
	}

	void FixedUpdate() {
		switch (m_curState) {
		case currentState.MOVING_FORWARD:
			if ((Vector3.Cross(new Vector3(m_rigidbody.velocity.x,m_rigidbody.velocity.y,0), m_transform.up)).magnitude < m_maximumSpeed) {
				//Don't turn
			}
			break;
		case currentState.TURNING_LEFT:
			m_rigidbody.AddTorque ( m_turnRate);
			m_rigidbody.velocity *= .95f;
			break;
		case currentState.TURNING_RIGHT:
			m_rigidbody.AddTorque ( -1 * m_turnRate);
			m_rigidbody.velocity *= .95f;
			break;
			
		}
		m_rigidbody.AddForce (m_transform.up * m_acceleration);
	}
}
