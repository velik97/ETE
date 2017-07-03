using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	public float dist;
	private GameObject player;
	protected GameObject gameController;
	private bool doOnce;
	private AudioSource aud;

	void Awake() {
		player = GameObject.FindWithTag("Player");
		if (player == null)
			Debug.LogError("There is no object with the 'Player' tag!");
		gameController = GameObject.FindWithTag("GameController");
		if (gameController == null)
			Debug.LogError("There is no object with the 'GameController' tag");
		doOnce = true;
		aud = GetComponent <AudioSource> ();
	}

	void FixedUpdate(){
		if (Vector3.Magnitude(player.transform.position - transform.position) < dist)
			transform.position = Vector3.Lerp(transform.position, player.transform.position + (player.transform.position - transform.position) * 3f , 0.1f);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Board" || other.gameObject.tag == "Glass" || other.gameObject.tag == "Left Engine" || other.gameObject.tag == "Right Engine"){
			if (doOnce){
				PickUpJob();
				doOnce = false;
			}
		}

		Destroy(this.gameObject, aud.clip.length);
	}

	public virtual void PickUpJob() {
		aud.Play();
		GetComponent<SpriteRenderer>().enabled = false;
	}
}

