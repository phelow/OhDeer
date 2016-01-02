using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
	[SerializeField]
    private Transform target;

	[SerializeField]
	private Rigidbody2D targetRb;

	private Vector3 m_destination;
	void Start(){
		targetRb = target.GetComponent<Rigidbody2D> ();
	}

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
			Vector3 delta = target.position + new Vector3(targetRb.velocity.x,targetRb.velocity.y,-10.0f) - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -10.0f)); //(new Vector3(0.5, 0.5, point.z));

			m_destination = new Vector3(transform.position.x,transform.position.y, -10.0f) + delta * 2.0f;

			transform.position = Vector3.Lerp(transform.position, new Vector3(m_destination.x,m_destination.y,-10.0f), Time.fixedDeltaTime);
        }

    }
}