using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private GameObject board;
	private GameObject glass;

	public Text gameOverText;

	private Health boardHealth;
	private Health glassHealth;
	
	[HideInInspector] public static bool playerIsDead;

	public void Awake () {
		playerIsDead = false;
		board = GameObject.FindWithTag("Board");
		glass = GameObject.FindWithTag("Glass");

		boardHealth = board.GetComponent<Health>();
		glassHealth = glass.GetComponent<Health>();
		gameOverText.text = "";
	}

	public void Update () {
		if (boardHealth.currentHealth <= 0 || glassHealth.currentHealth <= 0)
			playerIsDead = true;
		if (playerIsDead)
			gameOverText.text = "Game Over";
		if (Input.GetKey(KeyCode.R))
			Restart();
	}

	public void Restart () {
		Application.LoadLevel(Application.loadedLevel);
	}
}
