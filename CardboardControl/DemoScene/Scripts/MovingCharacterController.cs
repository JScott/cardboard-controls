using UnityEngine;
using System.Collections;

public class MovingCharacterController : MonoBehaviour {
  // For a full explanation of the API, look at ExampleCharacterController.cs
  // This example will assume knowledge of the API to code a moving first-person character

  public float speed = 17f;
  public float reticleMaxLength = 2f;
  public GameObject laserPrefab;

  private static CardboardControl cardboard;
  private float reticleTimer = 0f;
  private bool evenLaser = false;

  void Start() {
    cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
    cardboard.trigger.OnClick += Interact;
  }

  void Interact(object sender) {
    cardboard.reticle.Show();
    reticleTimer = reticleMaxLength;
    FireLaser();
  }
	
	void Update() {
    // This is the key: moving forward when the trigger is held
	  if (cardboard.trigger.IsHeld()) {
      Vector3 movement = Camera.main.transform.forward;
      transform.position += movement * speed * Time.deltaTime;
    }

    // We don't need the reticle unless the player is actively firing lasers
    if (reticleTimer < 0f) {
      reticleTimer = 0f;
      cardboard.reticle.Hide();
    }
    else if (reticleTimer > 0f) {
      reticleTimer -= Time.deltaTime;
    }
	}

  // If you need more control around movement, create a ToggleMove() method
  // which toggles a boolean that gets checked in Update instead of trigger.IsHeld()
  // and then tie that method to both trigger.OnUp and trigger.OnDown

  void FireLaser() {
    Vector3 position = transform.position;
    position -= Vector3.up * 0.5f;
    Vector3 offset = Camera.main.transform.right * 2f;
    position += evenLaser ? offset : -offset;
    evenLaser = !evenLaser;
    Vector3 rotation = Camera.main.transform.rotation.eulerAngles;
    rotation.x += 90f;
    Instantiate(laserPrefab, position, Quaternion.Euler(rotation));
  }
}
