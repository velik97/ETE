using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	[Header("Gun Features")]
	public float viewDist;
	public float viewAngle;
	public float turnSpeed;
	public float firstShotDelay;
	public bool timeIsInert;
	public float delayBetweenShots;
	public int maxWhizbangsNum;
	public float timeBeforeAwake;
	public GameObject whizbang;

	[Header("Transforms")]
	public Transform gun;
	public Transform holder;
	public Transform spawnPoint;
	public GameObject shotFlame;

	[Header("Shot Sounds")]
	public AudioClip[] shotSounds;
	
	private GameObject player;
	private AudioSource audioSource;
	private LineRenderer lineRenderer;
	private Vector3 forward;
	private RaycastHit2D forwardHit;
	private Vector3 relativePlayerPos;
	private RaycastHit2D hitToPlayer;
	private float awakeTimeOffset;
	private float shootTimeOffset;
	private bool firstShot;
	private bool movingRight; 			// when Idle
	private float angle;
	private bool followedPlayerInPreviousFrame;
	private bool willFollow;
	private int whizbangsCount;

	private void Awake () {
		player = GameObject.FindWithTag("Player");
		if (player == null)
			Debug.LogError("There is no object with the 'Player' tag!");
		awakeTimeOffset = timeBeforeAwake;
		movingRight = true;
		angle = 0f;
		willFollow = false;
		whizbangsCount = 0;
		audioSource = GetComponent <AudioSource> ();
		lineRenderer = GetComponent <LineRenderer> ();
	}

	private void Update () {
		relativePlayerPos = player.transform.position - spawnPoint.position;

		if (relativePlayerPos.sqrMagnitude < 250000f) {
			SetVariables ();
			ChekForFollowing ();

			forwardHit = Physics2D.Raycast (spawnPoint.position, transform.up, viewDist);

			if (willFollow) {
				FollowPlayer ();
				followedPlayerInPreviousFrame = true;
			}
			else {
				Idle ();
				followedPlayerInPreviousFrame = false;
			}

			if (forwardHit.transform != null) {
				if (forwardHit.transform.tag == "Player") {
					awakeTimeOffset = 0f;
					if (whizbangsCount < maxWhizbangsNum)
						ShootAtPlayer ();

				}
			}
		}

	}

	private void FixedUpdate () {
		DrawRays ();
	}

	private void SetVariables () {
		hitToPlayer = Physics2D.Raycast (spawnPoint.position, relativePlayerPos, viewDist);

		forward = (transform.position - holder.position).normalized;
		forwardHit = Physics2D.Raycast (spawnPoint.position, transform.up, viewDist);
	}

	private void ChekForFollowing () {

		if (!willFollow) {
			if (hitToPlayer.transform != null)
				if (hitToPlayer.transform.tag == "Player")
					willFollow = true;

			if (forwardHit.transform == null) 
				willFollow = false;
			else if (forwardHit.transform.tag != "Player") 
				willFollow = false;
		}

		if (willFollow) {
			willFollow = false;
			if (hitToPlayer.transform != null)
				if (hitToPlayer.transform.tag == "Player")
					willFollow = true;
			if (forwardHit.transform != null)
				if (forwardHit.transform.tag == "Player")
					willFollow = true;
			if (Vector3.Angle (relativePlayerPos, forward) >= viewAngle)
				willFollow = false;
		}
	}

	private void FollowPlayer () {
		if (hitToPlayer.transform != null)
			if (hitToPlayer.transform.tag == "Player")
				transform.up = Vector3.Lerp (transform.up, relativePlayerPos, Time.deltaTime * 1f);
		awakeTimeOffset = 0f;
	}

	private void ShootAtPlayer () {
		shootTimeOffset += Time.deltaTime;
		if (firstShot) {
			if (shootTimeOffset >= firstShotDelay) {
				Shot ();
				firstShot = false;
			}
		}
		else {
			if (shootTimeOffset >= firstShotDelay + delayBetweenShots) {
				Shot ();
				shootTimeOffset = firstShotDelay;
			}
		}
	}

	private void Shot () {
		GameObject newWhizbang = Instantiate (whizbang, spawnPoint.position ,Quaternion.identity) as GameObject;
		newWhizbang.transform.up = transform.up;
		if (shotSounds.Length != 0) {
			audioSource.clip = shotSounds[Random.Range(0, shotSounds.Length - 1)];
			audioSource.Play ();
		}
		whizbangsCount ++;
		if (shotFlame != null)
			Instantiate (shotFlame, spawnPoint.position, Quaternion.LookRotation(transform.up));
	}

	private void Idle () {
		if (followedPlayerInPreviousFrame) {
			angle = Vector3.Angle (forward, transform.up);
			if (transform.localRotation.z > 0)
				angle = - angle;
		}

		awakeTimeOffset += Time.deltaTime;
		if (awakeTimeOffset >= timeBeforeAwake) {
			if (movingRight) {
				angle = Mathf.Lerp (angle, - viewAngle - 5f, Time.deltaTime * turnSpeed);
				transform.up = TurnAt (forward, angle);
				if (angle + viewAngle <= 0)
					movingRight = false;
			} else {
				angle = Mathf.Lerp (angle, viewAngle + 5f, Time.deltaTime * turnSpeed);
				transform.up = TurnAt (forward, angle);
				if (angle - viewAngle >= 0)
					movingRight = true;
			}

				shootTimeOffset = 0;
				firstShot = true;
		}

		if (timeIsInert) {
			if (firstShot && shootTimeOffset >= 0)
				shootTimeOffset -= Time.deltaTime;
			if (!firstShot) {
				shootTimeOffset = 0;
				firstShot = true;
			}
		}

	}

	private void DrawRays () {
		lineRenderer.SetPosition(0, spawnPoint.position);
		if (forwardHit.transform == null) {
			lineRenderer.SetPosition (1, transform.position + transform.up * viewDist);
		}
		else {
			lineRenderer.SetPosition (1, forwardHit.point);
		}
		Debug.DrawRay (transform.position, forward * viewDist, Color.black);
		Debug.DrawRay (transform.position, TurnAt (forward, - viewAngle) * viewDist);
		Debug.DrawRay (transform.position, TurnAt (forward, viewAngle) * viewDist);
		if (relativePlayerPos.sqrMagnitude <= viewDist * viewDist)
			Debug.DrawRay (spawnPoint.position, relativePlayerPos , Color.yellow);
	}

	public Vector3 TurnAt (Vector3 turningVector, float angle) {
		angle = angle * Mathf.PI / 180f;
		Vector3 ret;
		float x = turningVector.x;
		float y = turningVector.y;
		ret = new Vector3 (Mathf.Cos (angle) * x + Mathf.Sin (angle) * y, - Mathf.Sin (angle) * x + Mathf.Cos (angle) * y, 0f);
		return ret;
	}

}
