using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	private Transform m_target;
	// Use this for initialization
	void Start () {
		m_target = transform.parent;
		transform.parent = null;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector2 (m_target.position.x, m_target.position.y - .1f);
	}
}
