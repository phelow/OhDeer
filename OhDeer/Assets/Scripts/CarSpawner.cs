using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour {
	[SerializeField]
	private float m_spawnTime;

	private static Player s_player;

	private static int s_nextEnemyScore = 1000;
	private static int s_incrementation = 1000;

	[SerializeField]
	private GameObject[] cars;
	[SerializeField]
	private GameObject hunter;

	private const float SPAWN_VARIANCE = 2.0f;

	private const float MIN_SPAWN_TIME = 1.0f;

	[SerializeField]
	private Transform m_spawnTransform;

	private GameObject m_firstTarget;

    private bool m_oneTime;

	// Use this for initialization
	void Start () {
		if(s_player == null){
			s_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		}
		StartCoroutine (SpawnCar ());
	}

	public void SetFirstTargetForSpawner(GameObject target, bool OneTime = false){
		m_firstTarget = target;
        m_oneTime = OneTime;
	}

	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator SpawnCar(){
        if (m_oneTime)
        {
            yield return new WaitForSeconds(1.0f);
        }

		while (true) {
			GameObject next = cars [Random.Range (0, cars.Length)];

			if (s_player.GetScore () > s_nextEnemyScore) {
				s_nextEnemyScore += s_incrementation + 100;
				s_incrementation = (int)(s_incrementation * .9f);
				next = hunter;
			}

			GameObject car = GameObject.Instantiate (next);
			car.transform.position = m_spawnTransform.position;
			car.transform.parent = transform;
			car.transform.rotation = Quaternion.LookRotation (m_firstTarget.transform.position - transform.position);
			car.GetComponent<Car> ().SetFirstTarget (m_firstTarget);

            if (m_oneTime)
            {
                car.transform.parent = null;
                Destroy(this.gameObject);
            }
			yield return new WaitForSeconds (Random.Range (Mathf.Max (MIN_SPAWN_TIME, m_spawnTime - SPAWN_VARIANCE), m_spawnTime + SPAWN_VARIANCE));

		}
	}
}
