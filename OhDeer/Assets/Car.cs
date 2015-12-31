using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

	[SerializeField]
	private float m_maximumSpeed;

	[SerializeField]
	private float m_acceleration;

	[SerializeField]
	private float m_breakPower;

	[SerializeField]
	private float m_turnRate;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Determine if there's a deer ahead and if it is to the left or the right

		// Determine if the deer is within breaking distance
	}

	void FixedUpdate() {
		
	}
}
