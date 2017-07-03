using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	[Header("Min Params")]
	public float minSpeed;			//min ship's speed that effects camera zoom
	public float minCameraZoom;

	[Header("Max Params")]
	public float maxSpeed;			//max ship's speed that effects camera zoom
	public float maxCameraZoom;

	[Header("Smooth Params")]
	public float movingSmooth;
	public float zoomSmooth;
	public float timeBeforeZoomIn;

	private float zoomToReach;
	private float timeOffset;

	public GameObject player;

	private Vector3 offsetPos;
			
	void Awake () {
		if (minSpeed >= maxSpeed)
			Debug.LogError("minSpeed should be smaller than maxSpeed!");
		if (minCameraZoom >= maxCameraZoom)
			Debug.LogError("minCameraZoom should be smaller then maxCamersZoom!");
		offsetPos = transform.position;
		zoomToReach = minCameraZoom;
	}
	
	void FixedUpdate () {

		SmoothMoving();
		SmoothZoom();

	}

	private void SmoothMoving () {
		Vector3 playerPos = player.transform.position;

		transform.position = Vector3.Lerp(transform.position, playerPos + offsetPos, Time.deltaTime * movingSmooth);
	}

	private void SmoothZoom () {
		float playerVelocity = Vector3.Magnitude(player.GetComponent<Rigidbody2D>().velocity);
		bool changeZoom = false;

		if (playerVelocity < minSpeed)
			zoomToReach = minCameraZoom;
		else if (playerVelocity > minSpeed && playerVelocity < maxSpeed)
			zoomToReach = minCameraZoom + (maxCameraZoom - minCameraZoom) * ((playerVelocity - minSpeed) / (maxSpeed - minSpeed));
		else 
			zoomToReach = maxCameraZoom;

		if (zoomToReach > GetComponent<Camera>().orthographicSize){
			timeOffset = 0f;
			changeZoom = true;
		}
		else 
			timeOffset += Time.deltaTime;

		if (timeOffset >= timeBeforeZoomIn)
			changeZoom = true;

		if (changeZoom)
			GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoomToReach, Time.deltaTime * zoomSmooth);

	}


}
