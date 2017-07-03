using UnityEngine;
using System.Collections;

public class Coin: PickUp {

	public override void PickUpJob () {
		base.PickUpJob();
		gameController.GetComponent<Score>().AddCoin();
	}
}
