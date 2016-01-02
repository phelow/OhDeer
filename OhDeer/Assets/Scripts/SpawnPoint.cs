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

	//The waypoints orientation relative to it's parent roadpiece
	public enum WaypointOrientation
	{
		Left,
		Right,
		Top,
		Bottom
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
	// Use this for initialization
	public void Spawn () {
		Collider2D cols = Physics2D.OverlapCircle (new Vector2(transform.position.x,transform.position.y), .1f);
		if (cols == null) {
			GameObject go = GameObject.Instantiate (m_viableRoadPieces [Random.Range (0, m_viableRoadPieces.Length)]);
			go.transform.position = transform.position;
			go.GetComponent<RoadPiece> ().Instantiate ();

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

			SpawnPoint sp = go.GetComponent<RoadPiece> ().GetSpawnPoint (oppositeOrientation);
			//TODO: link up all of the road pieces
			Waypoint [] targetInput;
			targetInput = sp.getInput();
			Waypoint[] targetOutput;
			targetOutput = sp.getOutput();

			foreach (Waypoint wp in m_output) {
				wp.SetNext (targetInput);
			}

			foreach (Waypoint wp in targetOutput) {
				wp.SetNext (m_input);
			}
		}



	}

	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (transform.position, 1);
	}

}
