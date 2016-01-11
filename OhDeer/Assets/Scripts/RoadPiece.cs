using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadPiece : MonoBehaviour {
	[SerializeField]
	private SpawnPoint [] m_spawnPoints;

	private const int GRID_LENGTH = 5;
	private const float ROAD_WIDTH = 10.22f;
	private const float ROAD_HEIGHT = 10.22f;

	public const float MAX_HEIGHT = GRID_LENGTH * ROAD_WIDTH;
	public const float MIN_HEIGHT = 0;

	public const float MAX_WIDTH = GRID_LENGTH * ROAD_HEIGHT;
	public const float MIN_WIDTH = 0;

	private List<Vector3> gizmos = new List<Vector3>();

	RoadPiece m_previous;

	[SerializeField]
	private List<RoadPiece> next;
	private bool m_locked = false;
	[SerializeField]
	private bool m_allLinked = false;

	[SerializeField]
	private bool m_terminates = false;

	public void AddNext(RoadPiece rp){
		next.Add (rp);
	}

	public void AddGizmo(Vector3 vec){
		gizmos.Add (vec);
	}

	public SpawnPoint [] SpawnPoints(){
		return m_spawnPoints;
	}
	public void Instantiate(RoadPiece prev)
	{
		m_previous = prev;
		next = new List<RoadPiece> ();
		if (transform.position.x < MAX_HEIGHT && transform.position.x >= 0 && transform.position.y < MAX_WIDTH && transform.position.y >= MIN_WIDTH) {
			m_locked = true;
			//Instantiate road pieces at all of this road's spawn points
			foreach (SpawnPoint spawnPoint in m_spawnPoints) {
				
				AddNext(spawnPoint.Spawn (this));
				
			}
			StartCoroutine (Regenerate ());
		} else {
			foreach (SpawnPoint spawnPoint in m_spawnPoints) {
				if (spawnPoint.IsLinked () == false) {
					spawnPoint.CreateCarSpawns ();
				}
			}
			m_terminates = true;
		}
	}

	public bool Terminates(){
		if (m_terminates) {
			return true;
		}
		foreach (RoadPiece child in next) {
			if(child != null && child.Terminates()){
				m_terminates = true;
			}
		}
		return m_terminates;
	}

	private bool AllSpawnPointsLinked(){
		bool allLinked = true;
		foreach (SpawnPoint sp in m_spawnPoints) {
			if (sp.IsLinked () == false) {
				allLinked = false;
			}
		}
		return allLinked;
	}
	public void StartRegeneration(){
		StartCoroutine(Regenerate());
	}
	private IEnumerator Regenerate(){
		int generations = 0;
		while((m_terminates == false || m_allLinked == false) && generations < 10){
			yield return new WaitForSeconds (.01f + generations * Random.Range(.1f,0.5f));
			if (transform.position.x < MAX_HEIGHT && transform.position.x >= 0 && transform.position.y < MAX_WIDTH && transform.position.y >= MIN_WIDTH) {
				
			} else {
				m_terminates = true;
				break;
			}
			bool regenerate = true;
			foreach (RoadPiece child in next) {
				if (child != null && child.Terminates () == true) {
					regenerate = false;
				}
			}

			if (regenerate && generations <7) {
				foreach (RoadPiece rp in next) {
					if (rp != null) {
						rp.KillMe ();
					}
				}
				next.Clear ();
				//Instantiate ();
			}

			foreach (SpawnPoint sp in m_spawnPoints) {
				if (sp.IsLinked () == false) {
					AddNext (sp.Spawn (this));
				}
			}
			generations++;
		}
		foreach (SpawnPoint sp in m_spawnPoints) {
			if (sp.IsLinked () == false) {
				AddNext (sp.Spawn (this));
				m_allLinked = false;
			}
		}
		if (m_allLinked == false && m_previous != null) {
			m_previous.StartRegeneration ();
		}
	}

	public void KillMe(){
		if (next != null) {
			foreach (RoadPiece rp in next) {
				if (rp != null) {
					rp.KillMe ();
				}
			}
		}
		Destroy (this.gameObject);
	}


	public SpawnPoint GetSpawnPoint(SpawnPoint.WaypointOrientation WaypointOrientation){
		foreach (SpawnPoint spawnPoint in m_spawnPoints) {
			if (spawnPoint.GetOrientation() == WaypointOrientation) {
				return spawnPoint;
			}
		}
		return null;
	}

	public void OnDrawGizmos(){
		foreach (Vector3 gizmo in gizmos) {
			Gizmos.color = Color.green;
			Gizmos.DrawCube (gizmo, Vector3.one);
		}
	}
		
}