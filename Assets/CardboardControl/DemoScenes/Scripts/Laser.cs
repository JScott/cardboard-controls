using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

  private float speed = 50f;
  private float timeLeft = 3f;
  private Vector3 direction;

  void Start() {
    direction = Camera.main.transform.forward;
  }

	void Update() {
	  Vector3 newPosition = transform.position;
    newPosition += direction * speed * Time.deltaTime;
    transform.position = newPosition;
    timeLeft -= Time.deltaTime;
    if (timeLeft <= 0) {
      Destroy(gameObject);
    }
	}
}
