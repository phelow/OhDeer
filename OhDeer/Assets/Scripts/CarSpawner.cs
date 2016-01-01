using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour {
	[SerializeField]
	private float m_spawnTime;

	[SerializeField]
	private GameObject[] cars;

	private const float SPAWN_VARIANCE = 1.0f;

	[SerializeField]
	private Transform m_spawnTransform;

	// Use this for initialization
	void Start () {
		StartCoroutine (SpawnCar ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator SpawnCar(){
		while (true) {
			yield return new WaitForSeconds (Random.Range (Mathf.Max (0, m_spawnTime - SPAWN_VARIANCE), m_spawnTime + SPAWN_VARIANCE));
			GameObject car = GameObject.Instantiate (cars [Random.Range (0, cars.Length)]);
			car.transform.position = m_spawnTransform.position;
			car.transform.rotation = m_spawnTransform.rotation;
		}
	}
}
