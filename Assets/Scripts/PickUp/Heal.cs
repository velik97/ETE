using UnityEngine;
using System.Collections;

public class Heal : PickUp {

	[Range(0f,100f)] 
	public float addLivePercent;

	private GameObject[] shipPartsObj = new GameObject[4];
	private Health[] shipparts = new Health[4];
	private float[] startlive = new float[4];

	public override void PickUpJob () {
		base.PickUpJob();

		shipPartsObj[0] = GameObject.FindWithTag("Board");
		shipPartsObj[1] = GameObject.FindWithTag("Glass");
		shipPartsObj[2] = GameObject.FindWithTag("Right Engine");
		shipPartsObj[3] = GameObject.FindWithTag("Left Engine");

		for (int i = 0; i < 4 ; i++) {
			shipparts[i] = shipPartsObj[i].GetComponent<Health>();
			startlive[i] = shipparts[i].startHealth;
			shipparts[i].currentHealth += shipparts[i].startHealth * addLivePercent / 100f;
			if (shipparts[i].currentHealth > startlive[i])
				shipparts[i].currentHealth = startlive[i];
			shipparts[i].CheckForSpriteChanging ();
		}
	}

}
