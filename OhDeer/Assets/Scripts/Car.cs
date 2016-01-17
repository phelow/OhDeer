using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Car : MonoBehaviour {

	[SerializeField]
	private float m_maximumSpeed;

	[SerializeField]
	private float m_acceleration;

	[SerializeField]
	private float m_turnRate;


	[SerializeField]
	private Rigidbody2D m_rigidbody;

	[SerializeField]
	private Transform m_transform;

	[SerializeField]
	private int m_health;

	private const float TURN_TOLERANCE = 0.3f;

	private const float CHECK_FOR_DESTRUCTION_TIME = 1.0f;

	private const float HEALTH_DIVISOR = 1.4f;

	private const float DESTRUCTION_BUFFER = 20.0f;

	private const float MAX_WAYPOINT_COLLISION_DISTANCE = 3.0f;


	private bool m_targetingPlayer = false;

	private Vector3 m_playerLastSpotted;

	private static Player m_player;

	[SerializeField]
	private bool m_hunter;

	[SerializeField]
	private GameObject m_targetWaypoint;

	[SerializeField]
	private List<GameObject> m_avoiding;

	private bool m_givePoints = false;

	[SerializeField]
	private currentState m_curState = currentState.PURSUING_WAYPOINT;

	public enum currentState
	{
		PURSUING_WAYPOINT,
		AVOIDING_OBSTACLES,
		PANICKING
	}

	public void AddAvoiding(GameObject go){
		if (!m_avoiding.Contains (go)) {
			m_avoiding.Add (go);
		}
	}

	public void RemoveAvoiding(GameObject go){
		m_avoiding.Remove (go);
	}

	public void SetTurning(currentState cs){
		m_rigidbody.angularVelocity *= .9f;
		m_curState = cs;
	}

	public void SetFirstTarget(GameObject target){
		
		m_targetWaypoint = target;
	}

	// Use this for initialization
	void Start () {
		m_avoiding = new List<GameObject> ();
		if (m_player == null) {
			m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		}
		if (m_hunter == false) {
			StartCoroutine (CheckForDestruction ());
		}

	}

	// Update is called once per frame
	public IEnumerator CheckForDestruction () {
		while (true) {
			// Destroy this car when it is too far far from the camera
			if (m_transform.position.x > RoadPiece.MAX_WIDTH + DESTRUCTION_BUFFER || 
				m_transform.position.x < RoadPiece.MIN_WIDTH - DESTRUCTION_BUFFER ||
				m_transform.position.y > RoadPiece.MAX_WIDTH + DESTRUCTION_BUFFER ||
				m_transform.position.y < RoadPiece.MIN_WIDTH - DESTRUCTION_BUFFER ) {
				Destroy (this.gameObject);
			}

			yield return new WaitForSeconds (CHECK_FOR_DESTRUCTION_TIME);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (m_hunter) {

		}
		else if (other.gameObject == m_targetWaypoint && Vector3.Distance(other.transform.position,transform.position) < MAX_WAYPOINT_COLLISION_DISTANCE) {
			if (m_targetWaypoint != null) {
				Waypoint nextPoint = m_targetWaypoint.GetComponent<Waypoint> ().GetNext ();
				if (nextPoint != null) {
					GameObject next = nextPoint.gameObject;
					if (next != null) {
						m_targetWaypoint = next;
					} else {
						Destroy (this.gameObject);
					}
				} else {
					Destroy (this.gameObject);
				}
			}
		}
	}

	public void GivePoints(){
		m_givePoints = true;
	}

	public bool GivesPoints()
	{
		return m_givePoints;
	}

	public void TargetPlayer()
	{
		m_targetingPlayer = true;
	}

	public void StopTargetingPlayer()
	{
		m_playerLastSpotted = m_player.transform.position;
		m_targetingPlayer = false;
	}


	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.tag == "Player") {
			// Kill the player
			collision.gameObject.GetComponent<Player>().KillPlayer();
		} else {
			//TODO: inflict damage based off speed and award points accordingly
			if (m_givePoints && m_hunter == false) {
				m_player.GainPoints (100);
			}
			m_health--;
			if (m_health < 0) {
				Explode ();
			}
		}
	}

	public float GetSpeed(){
		return m_rigidbody.velocity.magnitude;
	}

	void Explode()
	{
		ExplosionMaster.Explode (transform.position);
		//TODO: put in explosion mechanics
		Destroy(this.gameObject);
	}

	public static float LeftRightTest(Vector3 targetPos, Vector3 carPos, Vector3 forward, Vector3 up){
		Vector3 heading = targetPos - carPos;

		Vector3 perpendicular = Vector3.Cross (forward, heading);
		return Vector3.Dot (perpendicular, up);
	}
	void FixedUpdate() {
		float dir;
		bool turning = false;

		if (m_targetingPlayer) {
			//determine if we are to the left or right of the center mass
			dir = LeftRightTest (m_player.transform.position, transform.position, transform.forward, transform.up);


			//apply torque accordingly
			m_rigidbody.AddTorque ((dir < 0 ? 1 : -1) * m_turnRate);
		} else if (m_hunter) {
			if (m_playerLastSpotted == null) {
				m_playerLastSpotted = new Vector3 (Random.Range (0, RoadPiece.MAX_WIDTH), Random.Range (0, RoadPiece.MAX_HEIGHT),0);
			}

			if (Vector3.Distance (transform.position, m_playerLastSpotted) < 5.0f) {
				m_playerLastSpotted =  new Vector3 (Mathf.Clamp(Random.Range(m_playerLastSpotted.x -3.0f,m_playerLastSpotted.x +3.0f),0, RoadPiece.MAX_WIDTH), Mathf.Clamp (Random.Range(m_playerLastSpotted.y -3.0f,m_playerLastSpotted.y +3.0f) ,0, RoadPiece.MAX_HEIGHT));
			}

			dir = LeftRightTest (m_playerLastSpotted, transform.position, transform.forward, transform.up);


			//apply torque accordingly
			m_rigidbody.AddTorque ((dir < 0 ? 1 : -1) * m_turnRate);
		}
		else if (m_avoiding.Count > 0) {
			//calculate center of mass
			Vector3 average = Vector3.zero;
			bool removeNull = false;
			foreach (GameObject go in m_avoiding) {
				if (go == null) {
					removeNull = true;
				} else {
					average += go.transform.position;
				}
			}

			if (removeNull) {
				RemoveAvoiding (null);
			}

			//determine if we are to the left or right of the center mass
			dir = LeftRightTest(average, transform.position,transform.forward,transform.up);


			//apply torque accordingly
			m_rigidbody.AddTorque((dir < 0 ? -1 : 1) * m_turnRate);
		}
		else
		{
			if (m_targetWaypoint != null) {
				dir = LeftRightTest (m_targetWaypoint.transform.position, transform.position, transform.forward, transform.up);

				if (dir > .1 || dir < -.1) {
					//apply torque accordingly
					turning = true;
					m_rigidbody.AddTorque ((dir < 0 ? 1 : -1) * m_turnRate);
				}
			}
		}
		// Apply force
		if (m_rigidbody.velocity.magnitude > m_maximumSpeed) {
			m_rigidbody.velocity *= .98f;
		}


		m_rigidbody.angularVelocity *= .9f;
		if (turning) {
			m_rigidbody.velocity *= .97f;
			m_rigidbody.AddForce (transform.up * m_acceleration * .5f);
		} else {
			m_rigidbody.AddForce (transform.up * m_acceleration);
		}

	}
}
