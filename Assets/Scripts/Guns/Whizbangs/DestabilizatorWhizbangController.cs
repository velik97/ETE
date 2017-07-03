using UnityEngine;
using System.Collections;

public class DestabilizatorWhizbangController : MonoBehaviour {

	public float desactivationTime;
	public float startWidth;
	public float liveTime;

	GameObject player;
	LineRenderer lineRend;

	Ray ray;
	RaycastHit rayHit;

	private float time;

	void Awake() {
		player = GameObject.FindWithTag("Player");
		if (player == null)
			Debug.LogError("There is no object with the 'Player' tag!");
		lineRend = GetComponent <LineRenderer> ();
		lineRend.SetWidth(startWidth, startWidth);
		lineRend.SetPosition(0, transform.position);
		lineRend.SetPosition(1, player.transform.position);
		time = Time.time;
		player.GetComponent <PlayerController> ().Destabilizate(desactivationTime);
		ray = new Ray(transform.position, player.transform.position - transform.position);
		if (Physics.Raycast(ray, out rayHit)){
			if (rayHit.collider.tag == "Board" || rayHit.collider.tag == "Glass" || rayHit.collider.tag == "Right Engine" || rayHit.collider.tag == "Left Engine"){
				rayHit.collider.gameObject.GetComponent<Health>().TakeDamage(rayHit.point, ray.direction, 4000f, 0f);
			}
		}

	}

	void Update() {
		float width = startWidth * Mathf.Log10(-(9 / liveTime) * (Time.time - time) + 10);
		lineRend.SetWidth(width, width);
		if (width == 0)
			Destroy(this.gameObject);
	}
}
