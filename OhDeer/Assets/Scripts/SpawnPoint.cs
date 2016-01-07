using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {


	[SerializeField]
	private GameObject [] m_viableRoadPieces;

	[SerializeField]
	private Waypoint [] m_output;

	[SerializeField]
	private Waypoint [] m_input;

	[SerializeField]
	private WaypointOrientation m_orientation;

	[SerializeField]
	private GameObject m_carSpawner;

	private bool m_linked = false;

	//The waypoints orientation relative to it's parent roadpiece
	public enum WaypointOrientation
	{
		Left,
		Right,
		Top,
		Bottom
	}
	public void Unlink(){
		m_linked = false;
	}
	public void Link(){
		m_linked = true;
	}

	public bool IsLinked(){
		return m_linked;
	}

	public WaypointOrientation GetOrientation(){
		return m_orientation;
	}

	public Waypoint [] getInput()
	{
		return m_input;
	}

	public Waypoint [] getOutput()
	{
		return m_output;
	}

	public void CreateCarSpawns(){
		foreach (Waypoint wp in m_input) {
			GameObject go = GameObject.Instantiate (m_carSpawner);
			go.transform.position = wp.transform.position;
			go.transform.rotation = wp.transform.rotation;
			go.GetComponent<CarSpawner> ().SetFirstTargetForSpawner (wp.gameObject);
		}
	}

	public bool PlacePiece(int index){
		GameObject go = GameObject.Instantiate (m_viableRoadPieces [index]);
		go.transform.position = transform.position;

		WaypointOrientation oppositeOrientation = WaypointOrientation.Bottom;
		switch (m_orientation) {
		case WaypointOrientation.Bottom:
			oppositeOrientation = WaypointOrientation.Top;
			break;
		case WaypointOrientation.Left:
			oppositeOrientation = WaypointOrientation.Right;
			break;
		case WaypointOrientation.Right:
			oppositeOrientation = WaypointOrientation.Left;
			break;
		case WaypointOrientation.Top:
			oppositeOrientation = WaypointOrientation.Bottom;
			break;
		}
		RoadPiece rp = go.GetComponent<RoadPiece> ();
		SpawnPoint sp = rp.GetSpawnPoint (oppositeOrientation);
		//TODO: link up all of the road pieces

		Waypoint[] targetInput;
		targetInput = sp.getInput ();
		Waypoint[] targetOutput;
		targetOutput = sp.getOutput ();
		Link ();
		sp.Link ();

		if (rp.UnconnectedSpawnPointsOpen()) {
			foreach (Waypoint wp in m_output) {
				wp.SetNext (targetInput);
			}

			foreach (Waypoint wp in targetOutput) {
				wp.SetNext (m_input);
			}
			go.GetComponent<RoadPiece> ().Instantiate ();

			return true;
		} else {
			Unlink ();
			Destroy (go);
			return false;
		}
	}

	public bool IsClear()
	{
		Collider2D [] col = Physics2D.OverlapCircleAll (new Vector2(transform.position.x,transform.position.y), .1f);
		return col.Length <= 1;
	}

	// Use this for initialization
	public bool Spawn () {
		if (IsClear()) {
			int upperBound = m_viableRoadPieces.Length;
			while (upperBound-1 > 0) {
				int pick = Random.Range (0, upperBound-1);
				if (PlacePiece (pick)) {
					return true;
				}
				m_viableRoadPieces [pick] = m_viableRoadPieces [upperBound-1];
				upperBound--;
			}
			return false;
		} else {
			return false;
		}
	}

	void OnDrawGizmos(){
		if (m_linked) {
			Gizmos.color = Color.yellow;
		} else {
			Gizmos.color = Color.blue;

		}
		Gizmos.DrawSphere (transform.position, 1);
	}
}
