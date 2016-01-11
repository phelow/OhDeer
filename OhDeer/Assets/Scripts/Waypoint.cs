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
		int nElements = 0;
		foreach (Waypoint wp in m_nextWaypoints) {
			if (wp != null) {
				nElements++;
			}
		}
		foreach (Waypoint wp in next) {
			if (wp != null) {
				nElements++;
			}
		}

		Waypoint [] tempNext = new Waypoint[nElements] ;
		nElements = 0;
		foreach (Waypoint wp in m_nextWaypoints) {
			if (wp != null) {
				tempNext [nElements] = wp;
				nElements++;
			}
		}
		foreach (Waypoint wp in next) {
			if (wp != null) {
				tempNext [nElements] = wp;
				nElements++;
			}
		}
		m_nextWaypoints = tempNext;

		foreach (Waypoint wp in m_nextWaypoints) {
			if (wp == null) {
				Debug.LogError ("Null");
			}
		}

	}

	public Waypoint GetNext(){
		if (m_nextWaypoints.Length == 0) {
			return null;
		} else {
			return m_nextWaypoints [Random.Range (0, m_nextWaypoints.Length)];
		}
	}
}
