using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardInput cardboard;

	void Start () {
    // Make sure we never dim the screen
	  Screen.sleepTimeout = SleepTimeout.NeverSleep;
    cardboard = new CardboardInput();
    cardboard.OnClick += ModifySphere;
	}

  public void ModifySphere() {
    GameObject sphere = GameObject.Find("Sphere");
    sphere.renderer.material.color = new Color(Random.value, Random.value, Random.value);
  }

	// Update is called once per frame
	void Update () {
    cardboard.Update(Input.acceleration, Input.compass.rawVector);
	}
}
