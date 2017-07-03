using UnityEngine;
using System.Collections;

public class BulletControll : MonoBehaviour {

	public float speed;
	public float damage;
	public float force;

	public GameObject glassDebris;
	public GameObject boardDebris;
	public GameObject asteroidDebris;
	public ParticleSystem trail;

	GameObject player;

	void Awake() {
		player = GameObject.FindWithTag("Player");
		if (player == null)
			Debug.LogError("There is no object with the 'player' tag");
	}

	void Start() {
		MoveAtPlayer();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Board" || other.gameObject.tag == "Left Engine" || other.gameObject.tag == "Right Engine"){
			MakeDamage(other.gameObject);
			DestroyBullet(boardDebris);
		}
		else if (other.gameObject.tag == "Glass"){
			MakeDamage(other.gameObject);
			DestroyBullet(glassDebris);
		}
		else if (other.gameObject.tag == "Asteroid"){
			DestroyBullet(asteroidDebris);
		}
	}
	

	public void MoveAtPlayer() {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.velocity = (Vector2) (transform.up * speed);
	}
	
	public void MakeDamage(GameObject shipPart) {
		shipPart.GetComponent<Health>().TakeDamage(transform.position, transform.up, force, damage);
	}
	
	public void DestroyBullet(GameObject debris) {
		if (debris != null){
			Instantiate(debris, transform.position, Quaternion.identity);
		}
		GetComponent <SpriteRenderer>().enabled = false;
		GetComponent <BoxCollider2D>().enabled = false;
		GetComponent <Rigidbody2D>().velocity = Vector2.zero;
		trail.Stop ();
		Destroy(this.gameObject, 2f);
	}
}