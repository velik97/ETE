using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	[Header("Health (tunning in player object)")]
	public float startHealth;
	public float currentHealth;

	[Header("Audio")]
	public float pitch;

	[Header("Sprites")]
	public Sprite[] sprites;

	private AudioSource audioSource;
	private SpriteRenderer spriteRenderer;

	private Vector3 pos;
	private Vector3 velocity;
	private Vector3 velocityBefore;
	private Vector3 velocityAfter;
	protected float healthPercent;
	protected int spriteCount;
	protected int spriteNum;

	private bool touched;
	private int i;

	private GameObject player;

	private Rigidbody2D rb;

	void Start() {
		rb = GameObject.FindWithTag ("Player").GetComponent<Rigidbody2D>();
		currentHealth = startHealth;
		audioSource = GetComponent <AudioSource> ();
		spriteRenderer = GetComponent <SpriteRenderer> ();
		touched = false;
		i = 5;
		spriteCount = sprites.Length - 1;
	}

	void FixedUpdate() {
		Vector3 prevPos = pos;
		pos = transform.position;
		velocity = pos - prevPos;
		if (touched) {
			velocityBefore = velocity;
			i = 0;
			touched = false;
		}
		if (i == 4) {
			velocityAfter = velocity;
			float damage = Vector3.Magnitude(velocityAfter - velocityBefore) * 5f;
			Damage(damage);
		}

		i ++;
	}
	
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Asteroid") {
			touched = true;
		}
	}

	public void TakeDamage(Vector3 bulletPos, Vector3 dir, float force, float damage){
		Damage(damage);
		rb.AddForceAtPosition((Vector2) dir * force, (Vector2) bulletPos);
	}
	

	private void Damage(float damage) {
		currentHealth -= damage;
		audioSource.pitch = pitch + damage * 0.05f + Random.Range (-0.2f, 0.2f);
		audioSource.volume = damage * 0.025f;
		if (audioSource.volume > 1f)
			audioSource.volume = 1f;
		audioSource.Play();
		CheckForSpriteChanging ();
	}

	public virtual void CheckForSpriteChanging () {
		healthPercent = currentHealth / startHealth;
		spriteNum = Mathf.RoundToInt (spriteCount * healthPercent);
		if (spriteNum > 0) {
			spriteRenderer.sprite = sprites[spriteCount - spriteNum];
		} else {
			spriteRenderer.sprite = sprites[spriteCount];
		}
	}

}





