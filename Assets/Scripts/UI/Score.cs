using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	private GameObject player;
	private float maxPlayerPos;

	[Header("User Params")]
	public string name;
	public static string userName;

	[Header("Score Params")]
	public int scorePoint;
	public int coinPoint;

	private int score;
	private int coins;

	[Header("UI")]
	public Text scoreText;
	public Text coinsText;

	private bool uploadOnce;

	void Awake() {
		player = GameObject.FindWithTag("Player");
		if (player == null)
			Debug.LogError("There is no object with the 'player' tag");
		maxPlayerPos = player.transform.position.y;
		score = 0;
		coins = 0;
		uploadOnce = true;
		userName = name;
	}

	void Update () {
		if (!GameManager.playerIsDead) {
			if (player.transform.position.y > maxPlayerPos)
				maxPlayerPos = player.transform.position.y;
			score = Mathf.RoundToInt(maxPlayerPos * scorePoint / 50f) + coins * coinPoint;
			scoreText.text = Points("Score: ", score);
			coinsText.text = Points("Coins: ", coins);
		}
	}

	public string Points(string pointType, int points) {
		string ret;
		if (points < 1000)
			ret = pointType + points;
		else if (points < 1000000) {
			ret = pointType + (points / 1000) + "," ;
				if (points % 1000 >= 100) 
					ret += points % 1000;
			else
				ret += "0" + points % 1000;
		}
		else {
			ret = pointType + (points / 1000000) + "," + ((points % 1000000) / 1000) + "," + (points % 1000);
		}
		return ret;
	}
	

	public void AddCoin() {
		coins ++;
	}
}
