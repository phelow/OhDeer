using UnityEngine;
using System.Collections;

public class RoadPiece : MonoBehaviour {
	[SerializeField]
	private SpawnPoint [] m_spawnPoints;

	private const int GRID_LENGTH = 5;
	private const float ROAD_WIDTH = 10.22f;
	private const float ROAD_HEIGHT = 10.22f;

	private const float MAX_HEIGHT = GRID_LENGTH * ROAD_WIDTH;
	private const float MIN_HEIGHT = 0;

	public const float MAX_WIDTH = GRID_LENGTH * ROAD_HEIGHT;
	public const float MIN_WIDTH = 0;

	public void Instantiate()
	{
		if (transform.position.x < MAX_HEIGHT && transform.position.x >= 0 && transform.position.y < MAX_WIDTH && transform.position.y >= MIN_WIDTH) {
			//Instantiate road pieces at all of this road's spawn points
			foreach (SpawnPoint spawnPoint in m_spawnPoints) {
				spawnPoint.Spawn ();
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
}