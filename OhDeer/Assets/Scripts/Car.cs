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

	[SerializeField]
	private int m_health;

	private float m_fieldOfViewAngle = 90;

	private const float MAXIMUM_CAMERA_DISTANCE = 20.0f;

	private const float CHECK_FOR_DESTRUCTION_TIME = 1.0f;

	private const float HEALTH_DIVISOR = 1.4f;

	private static Player m_player;

	private enum currentState
	{
		MOVING_FORWARD,
		TURNING_LEFT,
		TURNING_RIGHT
	}

	private currentState m_curState = currentState.MOVING_FORWARD;

	// Use this for initialization
	void Start () {
		if (m_player == null) {
			m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		}

	}
	
	// Update is called once per frame
	public IEnumerator CheckForDestruction () {
		while (true) {
			// Destroy this car when it is too far far from the camera
			if (m_transform.position.x > MAXIMUM_CAMERA_DISTANCE || m_transform.position.x < -MAXIMUM_CAMERA_DISTANCE || m_transform.position.y > MAXIMUM_CAMERA_DISTANCE || m_transform.position.y < -MAXIMUM_CAMERA_DISTANCE) {
				Destroy (this.gameObject);
			}

			yield return new WaitForSeconds (CHECK_FOR_DESTRUCTION_TIME);
		}
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

	void OnCollisionEnter2D(Collision2D collision){


		if (collision.gameObject.tag == "Player") {
			// Kill the player
			collision.gameObject.GetComponent<Player>().KillPlayer();
		} else {
			//TODO: inflict damage based off speed and award points accordingly
			m_player.GainPoints(100);

			m_health--;
			if (m_health < 0) {
				Explode ();
			}
		}
	}

	void Explode()
	{
		//TODO: put in explosion mechanics
		Destroy(this.gameObject);
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
			m_rigidbody.AddForce (m_transform.up * m_acceleration);
			break;
		case currentState.TURNING_RIGHT:
			m_rigidbody.AddTorque ( -1 * m_turnRate);
			m_rigidbody.velocity *= .95f;
			m_rigidbody.AddForce (m_transform.up * m_acceleration);
			break;
			
		}
		m_rigidbody.AddForce (m_transform.up * m_acceleration);
	}
}
