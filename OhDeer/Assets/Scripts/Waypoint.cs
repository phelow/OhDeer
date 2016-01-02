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
		m_nextWaypoints = next;
	}

	public Waypoint GetNext(){
		return m_nextWaypoints [Random.Range (0, m_nextWaypoints.Length)];
	}
}
