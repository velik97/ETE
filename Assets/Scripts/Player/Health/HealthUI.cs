using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

	public GameObject viewObject;
	private Image viewPanel;

	private Health health;

	private float currentLive;
	private float startLive;

	private float livePercent;

	void Start(){
		health = viewObject.GetComponent<Health>();
		viewPanel = GetComponent<Image>();
	}

	void Update(){
		startLive = health.startHealth;
		currentLive = health.currentHealth;
		livePercent = currentLive / startLive;
		if (currentLive > 0f)
			viewPanel.color = new Color(1f - livePercent, livePercent, 0f, 0.5f);
		else
			viewPanel.color = new Color(0f, 0f, 0f, 0.5f);
	}

}
