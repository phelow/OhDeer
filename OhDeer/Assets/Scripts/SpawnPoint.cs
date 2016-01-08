using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public GameObject [] GetViableRoadPieces(){
		return m_viableRoadPieces;
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

	public bool PlacePiece(){
		GameObject inst = AvailablePieceThatFits ();

		if (inst != null) {
			GameObject go = GameObject.Instantiate (inst);
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

			//TODO: fix this to accomodate placing pieces with multiple connections

			RoadPiece rp = go.GetComponent<RoadPiece> ();
			SpawnPoint sp = rp.GetSpawnPoint (oppositeOrientation);
			//TODO: link up all of the road pieces

			Waypoint[] targetInput;
			targetInput = sp.getInput ();
			Waypoint[] targetOutput;
			targetOutput = sp.getOutput ();
			Link ();
			sp.Link ();


			foreach (Waypoint wp in m_output) {
				wp.SetNext (targetInput);
			}

			foreach (Waypoint wp in targetOutput) {
				wp.SetNext (m_input);
			}
			go.GetComponent<RoadPiece> ().Instantiate ();

			bool works = true;

			foreach(SpawnPoint targetSpawnPoints in rp.SpawnPoints())
			{
				if (targetSpawnPoints.IsColliding()) {
					works = targetSpawnPoints.SetConnections();
				}
			}
			if (works == false) {
				Destroy (this.gameObject);
				return false;
			}


			return true;
		} else {
			return false;
		}
	}

	public bool SetConnections(){
		Collider2D [] cols = Physics2D.OverlapCircleAll (new Vector2(transform.position.x,transform.position.y), 1.0f);
		RoadPiece rp = null;
		foreach (Collider2D cd in cols) {
			RoadPiece trp = cd.GetComponent<RoadPiece> ();
			if (trp != null) {
				rp = trp;
			}
		}

		if (rp == null) {
			return true;
		}

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

		//TODO: fix this to accomodate placing pieces with multiple connections

		SpawnPoint sp = rp.GetSpawnPoint (oppositeOrientation);
		if (sp == null) {
			return false;
		}
		if (sp.IsLinked ()) {
			return true;
		}
		Waypoint[] targetInput;
		targetInput = sp.getInput ();
		Waypoint[] targetOutput;
		targetOutput = sp.getOutput ();


		foreach (Waypoint wp in m_output) {
			wp.SetNext (targetInput);
		}

		foreach (Waypoint wp in targetOutput) {
			wp.SetNext (m_input);
		}
		Link ();
		sp.Link ();

		return true;
	}

	public bool IsColliding(){
		Collider2D [] cols = Physics2D.OverlapCircleAll (new Vector2(transform.position.x,transform.position.y), 1.0f);

		foreach (Collider2D cd in cols) {
			RoadPiece rp = cd.GetComponent<RoadPiece> ();
			if (rp != null) {
				return true;
			}
		}
		return false;
	}

	public GameObject AvailablePieceThatFits(){
		Collider2D [] cols = Physics2D.OverlapCircleAll (new Vector2(transform.position.x,transform.position.y), 1.0f);
		// if there's an intersection of a piece that fits, return that piece
		if (cols.Length > 1) {
			Debug.Log (cols.Length);
		}
		SpawnPoint[] spawnPoints = new SpawnPoint[cols.Length];

		List<GameObject> workingPieces;

		int index = 0;
		foreach (Collider2D c in cols) {
			SpawnPoint sp = c.GetComponent<SpawnPoint> ();
			if (sp == null) {
				return null;
			}
			spawnPoints [index] = sp;
			index++;
		}

		List<GameObject> AvailablePieces = new List<GameObject>();
		foreach (GameObject go in GetViableRoadPieces ()) {
			AvailablePieces.Add (go);
		}

		foreach (SpawnPoint sp in spawnPoints) {
			GameObject[] viablePieces = sp.GetViableRoadPieces ();

			List<GameObject> toRemove = new List<GameObject> ();

			foreach(GameObject go in AvailablePieces){
				bool contained = false;

				foreach(GameObject vp in viablePieces){
					if (vp == go) {
						contained = true;
					}
				}

				if (contained == false) {
					toRemove.Add (go);
				}
			}

			foreach (GameObject go in toRemove) {
				AvailablePieces.Remove (go);
			}
		}

		if (AvailablePieces.Count == 0) {
			return null;
		} else {
			return AvailablePieces [Random.Range (0, AvailablePieces.Count)];
		}
	}

	public bool IsClear()
	{
		Collider2D [] col = Physics2D.OverlapCircleAll (new Vector2(transform.position.x,transform.position.y),1.0f);

		foreach (Collider2D cl in col) {
			SpawnPoint sp = cl.GetComponent<SpawnPoint> ();
			if (sp == null) {
				return false;
			}
		}

		return true;
	}

	// Use this for initialization
	public bool Spawn () {
		if (IsClear() ) {
			return PlacePiece();
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
