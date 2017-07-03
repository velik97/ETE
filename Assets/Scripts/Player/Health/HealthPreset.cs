using UnityEngine;
using System.Collections;

public class HealthPreset : MonoBehaviour {

	[Header("Health")]
	public float boardHealth;
	public float glassHealth;
	public float enginesHealth;

	private GameObject board;
	private GameObject glass;
	private GameObject rightEngine;
	private GameObject leftEngine;

	private Health boardHealthScript;
	private Health glassHealthScript;
	private Health rightEngineHealthScript;
	private Health leftEngineHealthScript;

	public void Awake() {
		board = GameObject.FindWithTag("Board");
		glass = GameObject.FindWithTag("Glass");
		rightEngine = GameObject.FindWithTag("Right Engine");
		leftEngine = GameObject.FindWithTag("Left Engine");

		boardHealthScript = board.GetComponent<Health>();
		glassHealthScript = glass.GetComponent<Health>();
		rightEngineHealthScript = rightEngine.GetComponent<Health>();
		leftEngineHealthScript = leftEngine.GetComponent<Health>();

		boardHealthScript.startHealth = boardHealth;
		glassHealthScript.startHealth = glassHealth;
		rightEngineHealthScript.startHealth = enginesHealth;
		leftEngineHealthScript.startHealth = enginesHealth;

	}

}