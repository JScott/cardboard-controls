using UnityEngine;
using System.Collections;

public class MovingCharacterController : MonoBehaviour {
  // For a full explanation of the API, look at ControlsDemoManager.cs
  // This example will assume knowledge of the API to code a moving first-person character

  public float speed = 17f;
  public float reticleMaxLength = 2f;
  public GameObject laserPrefab;

  private static CardboardControl cardboard;
  private bool moving = false;
  private float reticleTimer = 0f;
  private bool evenLaser = false;

  void Start() {
    cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
    cardboard.trigger.OnDown += ToggleMove;
    cardboard.trigger.OnUp += ToggleMove;
    cardboard.trigger.OnClick += Interact;
  }

  void Interact(object sender) {
    cardboard.reticle.Show();
    reticleTimer = reticleMaxLength;
    FireLaser();
  }

  void ToggleMove(object sender) {
    moving = !moving;
  }

  void Update() {
    // If you don't need as much control over what happens when moving is toggled,
    // you can replace this with cardboard.trigger.IsHeld() and remove ToggleMove()
    if (moving) {
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

  void FireLaser() {
    GetComponent<AudioSource>().Play();
    Vector3 position = transform.position;
    position -= Vector3.up * 0.5f;
    Vector3 offset = cardboard.gaze.Right() * 2f;
    position += evenLaser ? offset : -offset;
    evenLaser = !evenLaser;
    Vector3 rotation = cardboard.gaze.Rotation().eulerAngles;
    rotation.x += 90f;
    Instantiate(laserPrefab, position, Quaternion.Euler(rotation));
  }
}
