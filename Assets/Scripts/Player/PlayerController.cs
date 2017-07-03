using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	[Header("Controll")]
	public float forwardForce;
	public float rotationForce;

	public KeyCode rightButton;
	public KeyCode leftButton;

	public bool invert;
	private bool isInverted;

	[Header("Stop Time")]
	public float stopTime;
	private float stopTimeOffset;

	private Rigidbody2D rb;

	//Engines' lives
	private Health rightEngine;
	private Health leftEngine;

	//Input
	private float leftInput;
	private float rightInput;

	//Random
	private int lStepsToRand, rStepsToRand;
	private float leftRandom;
	private float rightRandom;

	//Forces
	private float leftForce;
	private float rightForce;

	//Invertation & Destabilization
	private float currentDestabilizationTime;
	private float maxDestabilizationTime;

	void Awake() {
		rightEngine = GameObject.FindWithTag("Right Engine").GetComponent<Health>();
		leftEngine = GameObject.FindWithTag("Left Engine").GetComponent<Health>();
		rb = GetComponent <Rigidbody2D> ();

		leftRandom = Random.Range(0.5f, 1f);
		rightRandom = Random.Range(0.5f, 1f);
		lStepsToRand= Random.Range(30, 100);
		rStepsToRand = Random.Range(30, 100);
		currentDestabilizationTime = 10000f;
		maxDestabilizationTime = 0f;
	}

	void FixedUpdate () {
		if (!GameManager.playerIsDead) {

			if (Input.touches.Length > 0) {
				foreach (Touch touch in Input.touches) {
					if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
						if (touch.position.x > Screen.width / 2) {
							rightInput = 1f;
						} else {
							leftInput = 1f;
						}
					}
				}
			} else {
				rightInput = 0f;
				leftInput = 0f;
			}

#if UNITY_EDITOR 
			leftInput = BoolToFloat (Input.GetKey(leftButton));
			rightInput = BoolToFloat (Input.GetKey(rightButton));
#endif

			isInverted = false;

			if (invert)
				isInverted = !isInverted;
			CheckForDestabilization();

			if (isInverted)
				Invert();

			Randomize();
			SetForces();
			Move (leftForce, rightForce);
			CheckForStop ();
		}
	}


	private void Move (float l, float r) {
		rb.AddRelativeForce(new Vector2(0f, (l + r)) * forwardForce);
		rb.AddTorque((r - l) * rotationForce);
	}

	private void SetCurrentConfig() {
		GameObject preSets = GameObject.FindWithTag("Settings");
		if (preSets != null) {
//			invert = preSets.GetComponent <OptionsSettings> ().invert;
		}
		else {
			invert = false;
			Debug.Log("There is no object with the 'Settings' tag!");
		}
	}

	private void Invert() {
		float xchange;
		xchange = rightInput;
		rightInput = leftInput;
		leftInput = xchange;
	}

	private void CheckForDestabilization() {
		currentDestabilizationTime += Time.deltaTime;
		if (currentDestabilizationTime <= maxDestabilizationTime)
			isInverted = !isInverted;
	}

	private void SetForces () {
		if (leftEngine.currentHealth > 0)
			leftForce = leftInput * (leftEngine.currentHealth / leftEngine.startHealth) + leftRandom;
		else 
			leftForce = 0f;
		
		if (rightEngine.currentHealth > 0)
			rightForce = rightInput * (rightEngine.currentHealth / rightEngine.startHealth) + rightRandom;
		else
			rightForce = 0f;
	}

	private void Randomize() {
		if (lStepsToRand == 0){
			leftRandom = Random.Range(0.5f, 1f);
			lStepsToRand= Random.Range(30, 100);
		}
		if (rStepsToRand == 0){
			rightRandom = Random.Range(0.5f, 1f);
			rStepsToRand= Random.Range(30, 100);
		}
		lStepsToRand--;
		rStepsToRand--;
		
		leftRandom = leftRandom * ((leftEngine.startHealth - leftEngine.currentHealth) / leftEngine.startHealth) * 0.5f;
		rightRandom = rightRandom * ((rightEngine.startHealth - rightEngine.currentHealth) / rightEngine.startHealth) * 0.5f;
	}

	private void CheckForStop () {
		if (Mathf.Abs(transform.rotation.eulerAngles.z - 180f) < 30f) {
			stopTimeOffset += Time.deltaTime;
		} else {
			stopTimeOffset = 0f;
		}

		if (stopTimeOffset >= stopTime) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				rb.AddForce (new Vector2 (0f, 10000f));
				rb.AddTorque (10000f);
			}
		}
	}

	public void Destabilizate(float time){
		currentDestabilizationTime = 0f;
		maxDestabilizationTime = time;
	}

	float BoolToFloat (bool bl) {
		if (bl) return 1f;
		return 0f;
	}
}
