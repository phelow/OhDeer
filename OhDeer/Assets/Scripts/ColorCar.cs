using UnityEngine;
using System.Collections;

public class ColorCar : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer m_sr;
	// Use this for initialization
	void Start () {
		m_sr.color = new Color (Random.Range (0f, 1.0f), Random.Range (0f, 1.0f), Random.Range (0f, 1.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
