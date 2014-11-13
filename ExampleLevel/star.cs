using UnityEngine;
using System.Collections;

public class star : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
	  transform.Rotate(0, 100 * Time.deltaTime, 0);
	}
}
