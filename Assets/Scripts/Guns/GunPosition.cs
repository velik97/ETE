using UnityEngine;
using System.Collections;

public class GunPosition : MonoBehaviour {
	
	void Start () {
		SetPosition ();
	}

	private void SetPosition () {
		LayerMask asteroidsMask = LayerMask.GetMask ("Asteroids");

		Vector3 rightPos = transform.position + transform.right + transform.up * 2f;
		Vector3 leftPos = transform.position - transform.right + transform.up * 2f;

		RaycastHit2D rightHit = Physics2D.Raycast (rightPos, - transform.up, 10f, asteroidsMask);
		RaycastHit2D leftHit = Physics2D.Raycast (leftPos, - transform.up, 10f, asteroidsMask);

		Debug.DrawLine (rightPos, rightHit.point, Color.blue, 10f);
		Debug.DrawLine (leftPos, leftHit.point, Color.blue, 10f);

		Vector3 dir = rightHit.point - leftHit.point;

		transform.right = dir;
	}

}
