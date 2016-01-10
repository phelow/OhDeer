/// <summary>
/// Starting point. This is where all of the roads start from
/// </summary>
using UnityEngine;
using System.Collections;

public class StartingPoint : MonoBehaviour {
	[SerializeField]
	RoadPiece m_rp;
	void Start () {
		m_rp.Instantiate (null);
	}
}
