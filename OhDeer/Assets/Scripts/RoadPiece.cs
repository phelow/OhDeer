using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadPiece : MonoBehaviour {
	[SerializeField]
	private SpawnPoint [] m_spawnPoints;

	private const int GRID_LENGTH = 4;
	private const float ROAD_WIDTH = 10.22f;
	private const float ROAD_HEIGHT = 10.22f;

	public const float MAX_HEIGHT = GRID_LENGTH * ROAD_WIDTH;
	public const float MIN_HEIGHT = 0;

	public const float MAX_WIDTH = GRID_LENGTH * ROAD_HEIGHT;
	public const float MIN_WIDTH = 0;

	private List<Vector3> gizmos = new List<Vector3>();

	public void AddGizmo(Vector3 vec){
		gizmos.Add (vec);
	}

	public SpawnPoint [] SpawnPoints(){
		return m_spawnPoints;
	}
	public void Instantiate()
	{
		if (transform.position.x < MAX_HEIGHT && transform.position.x >= 0 && transform.position.y < MAX_WIDTH && transform.position.y >= MIN_WIDTH) {
			//Instantiate road pieces at all of this road's spawn points
			foreach (SpawnPoint spawnPoint in m_spawnPoints) {
				
				if(!spawnPoint.Spawn ()){
					spawnPoint.CreateCarSpawns ();
				}
				
			}
		} else {
			foreach (SpawnPoint spawnPoint in m_spawnPoints) {
				spawnPoint.CreateCarSpawns ();
			}
		}
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