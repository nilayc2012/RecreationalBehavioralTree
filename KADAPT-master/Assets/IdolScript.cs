using UnityEngine;
using System.Collections;

public class IdolScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        this.gameObject.SetActive(false);
    }
}
