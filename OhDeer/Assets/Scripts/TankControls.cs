﻿using UnityEngine;
using System.Collections;

public class TankControls : MonoBehaviour {
	bool m_leftDown;
	bool m_rightDown;
	bool m_upDown;
	bool m_downDown;

	private const float ROTATION_AMOUNT = 3.0f;
	private const float FORWARD_MOVEMENT = 10.0f;
	private const float BACKWARDS_MOVEMENT = -4.0f;
    private const float TOP_ANIMATION_SPEED = 3.0f;

    [SerializeField]
    private Animator m_animator;

	[SerializeField]
	private Rigidbody2D m_rigidbody;

	[SerializeField]
	private Transform m_transform;

	void Start()
	{
		StartCoroutine (CheckBoundaries ());
	}

	private IEnumerator CheckBoundaries(){
		while (true) {
			//Teleport the player closer to the nearest road if they've gone to far away from it
			yield return new WaitForSeconds(2.0f);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			m_leftDown = true;
		} else {
			m_leftDown = false;
		}

		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			m_rightDown = true;
		} else {
			m_rightDown = false;
		}

		if (Input.GetKey (KeyCode.UpArrow)|| Input.GetKey(KeyCode.W)) {
			m_upDown = true;
		} else {
			m_upDown = false;
		}

		if (Input.GetKey (KeyCode.DownArrow)|| Input.GetKey(KeyCode.S)) {
			m_downDown = true;
		} else {
			m_downDown = false;
		}
	}

	void FixedUpdate() {
		// Move horizontally
		if (m_leftDown && m_rightDown) { 
			// do nothing
		} else if (m_leftDown) {
			// rotate to the left
			m_rigidbody.AddTorque(ROTATION_AMOUNT);
			m_rigidbody.velocity *= .9f;
		} else if (m_rightDown) {
			// rotate to the right
			m_rigidbody.AddTorque(-ROTATION_AMOUNT);
			m_rigidbody.velocity *= .9f;
		}

		// Move vertically
		if (m_upDown && m_downDown) {
			// do nothing
		} else if (m_upDown) {
			m_rigidbody.AddForce (m_transform.up * FORWARD_MOVEMENT);
		} else if (m_downDown) {
			m_rigidbody.AddForce (m_transform.up * BACKWARDS_MOVEMENT);
		}

        m_animator.speed = m_rigidbody.velocity.magnitude / TOP_ANIMATION_SPEED;
	}
}
