using UnityEngine;
using System.Collections;

public class RocketControll : MonoBehaviour {

	public float forwardSpeed;
	public float turnSpeed;
	public float damage;
	public float force;
	public float lifeTime;
	public ParticleSystem trail;
	public ParticleSystem flame;

	public GameObject explotion;
	
	private GameObject player;
	private float timeFromStart;
	private Rigidbody2D rb;
	private bool dead;

	void Awake() {
		player = GameObject.FindWithTag("Player");
		if (player == null)
			Debug.LogError("There is no object with the 'player' tag");
		timeFromStart = Time.time;
		rb = GetComponent<Rigidbody2D>();
		dead = false;
	}

	void Start() {
		LookAtPlayer();
	}
	
	void FixedUpdate() {
		if (!dead) {
			if (Time.time - timeFromStart >= lifeTime) {
				DestroyRocket(explotion, true);
			} else {
				TurnToPlayer();
				MoveForward();
			}
		}
	}
	
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Board" || other.gameObject.tag == "Right Engine" || other.gameObject.tag == "Left Engine" || other.gameObject.tag == "Glass" || other.gameObject.tag == "Player"){
			MakeDamage(other.gameObject);
		}
		if (other.gameObject.tag != "PickUp")
			DestroyRocket(explotion, false);
	}
	

	public void MoveForward() {
		rb.velocity = Vector3.Normalize(transform.up) * forwardSpeed;
	}
	
	public void LookAtPlayer() {
		Vector3 relativePlayerPos = player.transform.position - transform.position;
		transform.up = Vector3.Normalize(relativePlayerPos);
	}
	
	public void TurnToPlayer() {
		Vector3 relativePlayerPos = player.transform.position - transform.position;
		transform.up = Vector3.Lerp(transform.up, relativePlayerPos, Time.deltaTime * turnSpeed);
	}
	
	public void MakeDamage(GameObject shipPart) {
		shipPart.GetComponent<Health>().TakeDamage (transform.position, transform.up, force, damage);
	}
	
	public void DestroyRocket(GameObject explotion, bool selfDeath) {
		dead = true;
		if (selfDeath) {
			Invoke ("Explode", 2f);
		} else {
			GetComponent <SpriteRenderer>().enabled = false;
			GetComponent <BoxCollider2D>().enabled = false;
			GetComponent <Rigidbody2D>().velocity = Vector3.zero;
			Explode ();
		}
		if (trail != null)
			trail.Stop ();
		if (flame != null)
			flame.Stop ();
		Destroy(this.gameObject, 2.1f);
	}

	public void Explode () {
		if (explotion != null)
			Instantiate(explotion, transform.position, Quaternion.identity);
	}
}








