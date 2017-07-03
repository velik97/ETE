using UnityEngine;
using System.Collections;

public class DestroyOnTime : MonoBehaviour {

	public float time;

	void Start () {
		Destroy (this.gameObject, time);
	}

}
