using UnityEngine;
using System.Collections;

public class DontDestroyThis : MonoBehaviour {
	
	void Awake () {
		DontDestroyOnLoad(this.gameObject);
	}

}
