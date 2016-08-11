using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private Transform defaultPos;
	private bool attachToHolder = false;

	public GameObject cameraHolder;

	// Use this for initialization
	void Start () {

		defaultPos = this.transform;
	}
	
	// Update is called once per frame
	void Update () {

		if (attachToHolder) {

			this.transform.position = cameraHolder.transform.position;
		}
	}

	public void ResetCameraPosition () {

		attachToHolder = false;
		this.transform.position = defaultPos.position;
	}

	public void EnableCameraMoveControls () {

		attachToHolder = true;
	}
}
