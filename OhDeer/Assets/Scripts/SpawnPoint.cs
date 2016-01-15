using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour {


	[SerializeField]
	private GameObject [] m_viableRoadPieces;

	private GameObject [] m_OriginalViableRoadPieces;

	[SerializeField]
	private SpriteRenderer m_tunnelSpriteRenderer;

	private const float RADAR_CHECK_SIZE = 2.0f;

	[SerializeField]
	private Waypoint [] m_output;

	[SerializeField]
	private Waypoint [] m_input;

	[SerializeField]
	private WaypointOrientation m_orientation;

	[SerializeField]
	private GameObject m_carSpawner;

	private SpawnPoint m_linked = null;

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
		m_linked = null;
		m_tunnelSpriteRenderer.enabled = true;
	}
	public void Link(SpawnPoint target){
		m_linked = target;
		m_tunnelSpriteRenderer.enabled = false;
	}

	public bool IsLinked(){
		return m_linked != null;
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

    public void OneTimeSpawn()
    {
        foreach (Waypoint wp in m_input)
        {
            GameObject go = GameObject.Instantiate(m_carSpawner);
            go.transform.position = wp.transform.position;
            go.transform.rotation = wp.transform.rotation;
            go.transform.parent = transform;
            go.GetComponent<CarSpawner>().SetFirstTargetForSpawner(wp.gameObject, true);
        }
    }

    public void CreateCarSpawns(){
		foreach (Waypoint wp in m_input) {
			GameObject go = GameObject.Instantiate (m_carSpawner);
			go.transform.position = wp.transform.position;
			go.transform.rotation = wp.transform.rotation;
			go.transform.parent = transform;
			go.GetComponent<CarSpawner> ().SetFirstTargetForSpawner (wp.gameObject);
		}
	}

	public RoadPiece PlacePiece(RoadPiece previous){
		if (m_OriginalViableRoadPieces == null) {
			m_OriginalViableRoadPieces =  m_viableRoadPieces;
		}

		m_viableRoadPieces = m_OriginalViableRoadPieces;
		List<GameObject> instantiables = AvailablePieceThatFits ();

		if (instantiables != null) {
			bool pieceNotPlaced = true;
			while (pieceNotPlaced && instantiables.Count > 0) {
				//TODO, prevent double selection
				int choice = Random.Range(0,instantiables.Count);
				GameObject go = GameObject.Instantiate (instantiables[choice]);
				instantiables.RemoveAt (choice);

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
				Link (sp);
				sp.Link (this);


				foreach (Waypoint wp in m_output) {
					wp.SetNext (targetInput);
				}

				foreach (Waypoint wp in targetOutput) {
					wp.SetNext (m_input);
				}
				go.GetComponent<RoadPiece> ().Instantiate (previous);

				bool works = true;

				foreach (SpawnPoint targetSpawnPoints in rp.SpawnPoints()) {
					if (targetSpawnPoints.IsColliding ()) {
						if (targetSpawnPoints.SetConnections () == false) {
							works = false;
						}
					}
				}

				if (ZoneCheck (rp) == false) {
					works = false;
				}

				if (works == false) {
					rp.KillMe ();
				} else {
					pieceNotPlaced = false;
					return rp;
				}
			}
			return null;
		} else {
			return null;
		}
	}

	public bool ZoneCheck (RoadPiece rp){
		Collider2D [] cols = Physics2D.OverlapCircleAll (new Vector2(rp.transform.position.x,rp.transform.position.y + 10.2f), RADAR_CHECK_SIZE);
		rp.AddGizmo(new Vector3(rp.transform.position.x,rp.transform.position.y + 10.2f,0));

		RoadPiece TopRp = null;
		foreach (Collider2D cd in cols) {
			RoadPiece trp = cd.GetComponent<RoadPiece> ();
			if (trp == null && cd.transform.parent != null) {
				trp = cd.transform.parent.GetComponentInChildren<RoadPiece> ();
			}
			if (trp == null && cd.transform.parent != null) {
				trp = cd.transform.parent.GetComponent<RoadPiece> ();
			}

			if (trp != null) {
				TopRp = trp;
			}
		}

		if (TopRp != null) {
			if (TopRp.GetSpawnPoint (WaypointOrientation.Bottom) != null && rp.GetSpawnPoint (WaypointOrientation.Top) == null) {
				return false;
			} else if (TopRp.GetSpawnPoint (WaypointOrientation.Bottom) == null && rp.GetSpawnPoint (WaypointOrientation.Top) != null) {
				return false;
			}
		}

		cols = Physics2D.OverlapCircleAll (new Vector2(rp.transform.position.x-10.2f,rp.transform.position.y), RADAR_CHECK_SIZE);
		rp.AddGizmo(new Vector3(rp.transform.position.x-10.2f,rp.transform.position.y,0));
		RoadPiece LeftRp = null;
		foreach (Collider2D cd in cols) {
			RoadPiece trp = cd.GetComponent<RoadPiece> ();
			if (trp == null && cd.transform.parent != null) {
				trp = cd.transform.parent.GetComponentInChildren<RoadPiece> ();
			}
			if (trp == null && cd.transform.parent != null) {
				trp = cd.transform.parent.GetComponent<RoadPiece> ();
			}
			if (trp != null) {
				LeftRp = trp;
			}
		}

		if (LeftRp != null) {
			if (LeftRp.GetSpawnPoint (WaypointOrientation.Right) != null && rp.GetSpawnPoint (WaypointOrientation.Left) == null) {
				return false;
			} else if (LeftRp.GetSpawnPoint (WaypointOrientation.Right) == null && rp.GetSpawnPoint (WaypointOrientation.Left) != null) {
				return false;
			}
		}
		cols = Physics2D.OverlapCircleAll (new Vector2(rp.transform.position.x + 10.2f,rp.transform.position.y), RADAR_CHECK_SIZE);
		rp.AddGizmo(new Vector3(rp.transform.position.x+10.2f,rp.transform.position.y,0));
		RoadPiece RightRp = null;
		foreach (Collider2D cd in cols) {
			RoadPiece trp = cd.GetComponent<RoadPiece> ();
			if (trp == null && cd.transform.parent != null) {
				trp = cd.transform.parent.GetComponentInChildren<RoadPiece> ();
			}
			if (trp == null && cd.transform.parent != null) {
				trp = cd.transform.parent.GetComponent<RoadPiece> ();
			}
			if (trp != null) {
				RightRp = trp;
			}
		}

		if (RightRp != null) {
			if (RightRp.GetSpawnPoint (WaypointOrientation.Left) != null && rp.GetSpawnPoint (WaypointOrientation.Right) == null) {
				return false;
			} else if(RightRp.GetSpawnPoint (WaypointOrientation.Left)  ==  null && rp.GetSpawnPoint(WaypointOrientation.Right) != null){
				return false;
			}
		}
		cols = Physics2D.OverlapCircleAll (new Vector2(rp.transform.position.x,rp.transform.position.y -10.2f), RADAR_CHECK_SIZE);
		rp.AddGizmo(new Vector3(rp.transform.position.x,rp.transform.position.y-10.2f,0));
		RoadPiece BottomRp = null;
		foreach (Collider2D cd in cols) {
			RoadPiece trp = cd.GetComponent<RoadPiece> ();
			if (trp == null && cd.transform.parent != null ) {
				trp = cd.transform.parent.GetComponentInChildren<RoadPiece> ();
			}
			if (trp == null && cd.transform.parent != null) {
				trp = cd.transform.parent.GetComponent<RoadPiece> ();
			}
			if (trp != null) {
				BottomRp = trp;
			}
		}

		if (BottomRp != null) {
			if (BottomRp.GetSpawnPoint (WaypointOrientation.Top) != null && rp.GetSpawnPoint (WaypointOrientation.Bottom) == null) {
				return false;
			} else if (BottomRp.GetSpawnPoint (WaypointOrientation.Top) == null && rp.GetSpawnPoint (WaypointOrientation.Bottom) != null) {
				return false;
			}
		}


		return true;
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
		Link (sp);
		sp.Link (this);

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

	public List<GameObject> AvailablePieceThatFits(){
		Collider2D [] cols = Physics2D.OverlapCircleAll (new Vector2(transform.position.x,transform.position.y), 1.0f);
		// if there's an intersection of a piece that fits, return that piece
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
			return AvailablePieces;
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
	public RoadPiece Spawn (RoadPiece previous) {
		if (IsClear() ) {
			return PlacePiece(previous);
		} else {
			return null;
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
