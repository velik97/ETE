using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	private float shakeTime;
	private float timeOffset = 0f;
	private float shakeForce;
	private Vector2 direction;
	private float timePercent;
	private Vector3 currentPos;

	public void Shake (Vector2 dir, float force, float time) {
		Debug.Log ("Shake");
		shakeForce = force;
		shakeTime = time;
		dir.Normalize ();
		direction = dir;

		InvokeRepeating ("DoOneMove", 0f, 0.05f);
		Invoke ("StopShake", time);
	}

	public void DoOneMove () {
		timePercent = (shakeTime - timeOffset) / (shakeTime);
		float currentShakeForce = Random.value * shakeForce * timePercent;
		Vector2 currentShakeMove = direction * currentShakeForce; 
		currentPos = transform.position;
		transform.position += (Vector3)currentShakeMove;
		Invoke ("MoveBack", 0.045f);
		timeOffset += 0.05f;
	}

	public void MoveBack () {
		transform.position = currentPos;
	}

	public void StopShake () {
		CancelInvoke ("DoOneMove");
		timeOffset = 0f;
	}

}
