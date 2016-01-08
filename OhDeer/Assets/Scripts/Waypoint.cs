using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {
	[SerializeField]
	private Waypoint[] m_nextWaypoints;
	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawCube (transform.position, Vector3.one * .5f);
	}

	public void SetNext(Waypoint [] next)
	{
		Waypoint [] tWayPoints = new Waypoint[m_nextWaypoints.Length + next.Length];
		int i;
		for (i = 0; i < m_nextWaypoints.Length; i++) {
			tWayPoints [i] = m_nextWaypoints [i];
		}

		for (i = m_nextWaypoints.Length; i < m_nextWaypoints.Length+next.Length; i++) {
			tWayPoints [i] = next [i - m_nextWaypoints.Length];
		}
		m_nextWaypoints = tWayPoints;
	}

	public Waypoint GetNext(){
		if (m_nextWaypoints.Length == 0) {
			return null;
		} else {
			return m_nextWaypoints [Random.Range (0, m_nextWaypoints.Length)];
		}
	}
}
